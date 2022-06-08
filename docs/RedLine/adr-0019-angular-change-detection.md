# ADR-0019: Use Change-Detection OnPush as Default

Date: 2021-04-30

## Status

Proposed

## Context

In order to improve performance and component resource consumption should we default new components change detection strategy to "OnPush". During Change Detection Cycle, Angular looks for all the bindings, re-executes all the expressions, compares it with the previous values and if a change is detected, it propagates the change to the DOM Elements. In Angular if "changeDetection" is not specified then it will default to "Default". We can configure the Change Detection Strategy for the component inside the decorator. We can also set the default strategy in angular.json using schematics.

### Angular

All Angular apps are made up of a hierarchical tree of components. At runtime, Angular creates a separate change detector class for every component in the tree, which then eventually forms a hierarchy of change detectors similar to the hierarchy tree of components. Whenever change detection is triggered, Angular walks down this tree of change detectors to determine if any of them have reported changes. The change detection cycle is always performed once for every detected change and starts from the root change detector and goes all the way down in a sequential fashion. This sequential design choice is nice because it updates the model in a predictable way since we know component data can only come from its parent.

A model in Angular can change as a result of any of the following:

- DOM events (click, hover, etc.)
- AJAX requests
- Timers (setTimer(), setInterval())

##### Change Detection Strategies

###### "Default"

This Change Detection Strategy works by checking if the value of template expressions have changed. This is done for all components. By default, Angular does not do deep object comparison to detect changes, it only takes into account properties used by the template.

`@Component({`

 `selector: 'app-book',`

 `templateUrl: './book.component.html',`

 `styleUrls: ['./book.component.scss'],`

`})`

Benefits of Default Change Detection

- This implementation is compatible with direct object mutation because angular will compare expressions of the object with each property and a change will be detected and the template will update to the new value.

Drawbacks of Default Change Detection

- The default strategy is less performant since there are extra render cycles involved for the child component even if there is no impact.

###### "OnPush"

During this Change Detection Strategy, the Child Component is not always dirty checked, if the parent element is updating the values that are not passed as @Input properties to the child component, then the child component should not be dirty checked. The component's change detection is triggered by either a change to a @Input object change or when a @Output event within the component is emitted.

`@Component({`

 `selector: 'app-book',`

 `templateUrl: './book.component.html',`

 `styleUrls: ['./book.component.scss'],`

 `changeDetection: ChangeDetectionStrategy.OnPush`

`})`

Pros:

- Faster Component Re-rendering
- All newly created components created using CLI commands will default to "OnPush" Change Detection.
- No Unnecessary Dirty Check in the Child Components - These components will not be dirty checked and re-rendered unless their input values change or output events are fired which in turn will use less  browser resources.

- Pages that have many components will be more performant.

Cons:

- Be Cautious about Object Mutation: The objects that are received as @Input property should not be mutated. When @Input objects are received from the parent Component, it is received as reference. If the original object is mutated from the parent element, the reference is not updated in the Child Component, so @Input is not receiving a new reference and will not be updated since the object in the @Input property is still the same.

- Objects that are being passed to components using "OnPush" change detection should not be mutated and instead need to be replaced with a new object that reflects any changes. This poses a new concern while debugging client side bugs.

- In order to manually trigger change detection inject 'ChangeDetectorRef' within the component constructor and trigger change detection with the following call.

  `this.changeDetectorRef.detectChanges();`

## Decision

- We decided that existing components in our solutions will not be refactored at this time.

- Make the default Change Detection strategy "OnPush".

  `"@schematics/angular:component"{`

   `"changeDetection": "OnPush"`

  `}`

## Consequences

- Since we are not updating existing components on pages made up of multiple UI components if we notice any runtime performance issues we may need to refactor those UI components. 
  - This is not referring to loading the page performance. We are referring to re-rendering based on changes or selections. Without utilizing "OnPush" we might see a increase in time taken and number of resources consumed.

