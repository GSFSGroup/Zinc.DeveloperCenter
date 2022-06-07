# ADR-0006: Domain Event Versioning

Date: 2020-04-23

## Status

Accepted

## Context

Domain events are published to the service bus as state changes occur in the domain aggregates. Services that are interested in those changes subscribe to the bus for those events and are notified accordingly. As time moves on, changes to the domain might necessitate changes in the domain events as well. 

Subscribers depend on events from other services, but they only get those changes at build time (either through packages or source copy). If an event changes in a way that is not backward-compatible, it might cause downstream errors in subscribers, until they are updated and rebuilt. Therefore, versioning is necessary to manage changes to domain events and to ensure backward compatibility.

We have considered a few concerns:

* Versioning strategy.
* Should we use interfaces or classes for the event objects?
* How do we share the event classes or interfaces with subscribers?

---

### Versioning strategy

We have considered several versioning strategies:

#### 1. Including a `Version` property in event objects.

With this strategy, each domain event object will have a `Version` property. Every time the event class/interface changes, the version is to be bumped.

Pros:
* Easy to implement. Only one class/interface with a version property.

Cons:
* Prone to incorrect versioning, if a developer forgets to update version.
* Harder to see the history of the version changes.
* Prone to potentially dangerous changes like changing a member name.

#### 2. `Version` in the namespace.

With this strategy, each domain event class/interface will be in a versioned namespace. Example source below shows a glimpse of this strategy.

```csharp
namespace Zinc.DeveloperCenter.Domain.Events.V1
{
    public class GreetingMessageChanged
    { ... }
}

namespace Zinc.DeveloperCenter.Domain.Events.V2
{
    public class GreetingMessageChanged : V1.GreetingMessageChanged
    { ... }
}

// Next namespace depends on whether we want to point to the latest version.
namespace Zinc.DeveloperCenter.Domain.Events
{
    public class GreetingMessageChanged : V2.GreetingMessageChanged
    { }
}
```

Pros:
* Easy to see history of changes to event classes/interfaces.
* Easy to employ policies to ensure version changes recorded and changes to members are done gracefully. For instance, open/closed principle: "No direct change to event class/interface allowed. You can only extend them."

Cons:
* Verbosity: A new namespace for each version causes generation of extra source files (in case of folders as namespaces) or verbose source files (in case of all namespaces defined in the same source file)
* Navigating to the latest version, when you don't know what is latest, is harder.

#### 3. `Version` in the class/interface name.

With this strategy, we will add a version number to the name of the class. See example source code below shows how this could be employed.

```csharp
namespace Zinc.DeveloperCenter.Domain.Events
{
    public class GreetingMessageChangedV1
    { ... }

    public class GreetingMessageChangedV2 : GreetingMessageChangedV1
    { ... }

    // Next namespace depends on whether we want to point to the latest version.
    public class GreetingMessageChanged : GreetingMessageChangedV2
    { }
}
```

Pros:
* More concise than the versioned namespace strategy.
* Easy to employ policies for change.

Cons:
* A little more verbose than option 1, but much less than option 2.

---

### Interfaces or Classes

NServiceBus, our tool of choice for messaging bus, allows us to use interfaces or classes for message contracts. That gives us an option to choose one or the other.

#### Interfaces

Pros:
* Provides separation from the event classes.

Cons:
* It could be a little more verbose, since we might need to write event class to implement the interface. This could be alleviated by changing the `PublishEvents(IDomainEvent @event)` call signature to `PublishEvent<T>(T @event)` or `PublishEvent<T>(Action<T> ctor)`.

#### Classes

Pros:
* Easier to write and less verbose.

Cons:
* Doesn't provide much separation, which could make it harder to distribute via either packaging or source copy.

---

### Sharing Event Classes or Interfaces with Subscribers

Tooling becomes important when we have to work with many messages from disparate sources. There are two options that we are considering for this purpose:

#### 1. Source or binary packages (Nuget, etc.)

Pros:
* A very common way to share message contracts. Needs little explanation at time of onboarding.

Cons:
* Language specific. Each language has their own packaging system.
* Requires setup with third-party providers for package repository.

### 2. Source copy

In this option, we plan to have all events of a domain in a specific folder. Then, we can copy the folder into source code to get the latest changes. A simple tool could be created and added to build tasks to help with this process.

Pros:
* Language agnostic.
* It doesn't require package repository, since it uses source code.

Cons:
* No standard tools. It requires a little training at time of onboarding.

## Decision

Our proposed decision is to use interfaces to define the contract for the events. Also, we think it is best to use the version number in the name of the interface. We have decided to have the class for the event to implement the latest versioned interface. Example source code below shows how it would look.

```csharp
namespace Zinc.DeveloperCenter.Domain.Events
{
    public interface IGreetingMessageChangedV1
    {
        // Original interface.
    }

    public interface IGreetingMessageChangedV2 : IGreetingMessageChangedV1
    {
        // Changes in V2
    }

    public class GreetingMessageChanged : IGreetingMessageChangedV2
    {
        // V1 members

        // V2 members
    }
}
```

We will follow Open/Closed Principle. No changes to the interface is allowed. For any change, we will add a new versioned interface and extend from the previous version. Example source code below shows a glimpse of how the code will look when there are further changes to the previous one.

```csharp
namespace Zinc.DeveloperCenter.Domain.Events
{
    public interface IGreetingMessageChangedV1
    {
        // Original interface.
    }

    public interface IGreetingMessageChangedV2 : IGreetingMessageChangedV1
    {
        // Changes in V2
    }

    public interface IGreetingMessageChangedV3 : IGreetingMessageChangedV2
    {
        // Further changes in V3
    }

    // Note that this now implements V3 instead of V2.
    public class GreetingMessageChanged : IGreetingMessageChangedV3
    {
        // V1 members

        // V2 members

        // V3 members
    }
}
```


## Consequences

* Backward compatibility is ensured through development practices.
* Keeping the interfaces in the same file makes it easy to navigate and find changes to events.
* Picking a strategy for distribution is made easier with interfaces, since they are concise and contained.
* More discussion is needed for distribution, since we couldn't reach a concensus.  
