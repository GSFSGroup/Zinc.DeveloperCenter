# ADR-0001: Event Sourcing

Date: 2021-04-01

## Status

Accepted

## Context

Unlike many of our existing microapplications, the earnings service has specific, well-defined business rules and intense audit requirements.

### Traditional 3NF relational design
| Pros                                | Cons                                                         |
|-------------------------------------|--------------------------------------------------------------|
| Familiar architecture for most devs | Audit concerns are not baked in.                             |
| Tools optimized for this style      | Complex object graphs are difficult to persist.              |
|                                     | Requires precise SQL migrations for each additional features |
|                                     | Data loss inherent in update, delete ops                     |
|                                     | Optimized for either reads or writes, but not both           |

### Document-centric design
| Pros                                                       | Cons                                            |
|------------------------------------------------------------|-------------------------------------------------|
| Easy to get started                                        | Audit data stored separately.                   |
| Easily maps to REST & HTTP                                 | Lots of duplication in audit data.              |
| Audit can be automated by storing every document revision. | Breaks down as domain becomes complex           |
| Easily searcheable                                         | Document schema versioning can become difficult |
| Complex object graphs are easy to persist                  |                                                 |


### Event sourced design
| Pros                                                  | Cons                                                                 |
|-------------------------------------------------------|----------------------------------------------------------------------|
| Optimized for reads and writes                        | Completely foreign to many devs                                      |
| Read models map well to UX query concept              | Set validation across aggregates is difficult (unique email problem) |
| Full audit baked in to design                         | Unforgiving when business rules are fluid                            |
| Audit data is minimal, changes only                   | Event versioning is difficult                                        |
| Simple to add new features                            | Data duplication in read models makes people nervous                 |
| Almost zero risk of data loss from bad migrations     | Difficult to scrub data for compliance w/ new privacy laws           |
| Maps well to accounting ledgers, other mature domains |                                                                      |
| Easy to test business logic with given / when / then  |                                                                      |


## Decision

For the reasons below, we chose event sourcing.

### Key Considerations
* Event sourcing is a good fit for the intense audit requirements of this domain. 
* The business rules are solid enough to fit event sourcing.
* The new features we anticipate in earnings, such as revisions, would require large, complicated schema changes without an event sourced domain.
* Privacy law compliance is not a concern. There is not PII in earnings.

## Consequences

The concepts, data flow, and technical implementation of event sourcing are foreign to many business application developers.

> Command Query Separation (CQS) is an object oriented pattern whereby a method is either a command or a query.
> * Commands change the state of a system but do not return a value.
> * Queries return results and do not change the observable state of the system.

> Command Query Responsibility Segregation (CQRS) extends CQS by splitting the model. This can be achieved by simply segregating the interfaces on a single object. However, CQRS is typically implemented by splitting the object models and persistence in two: a read-side and write-side.

> Event sourcing is a system where the state of an object is stored as a stream of events. The current state is achieved by replaying those stored events. 
> * Every change to the state of a model is captured in an event.
> * These events are stored in the sequence they were applied.
> * These events are stored for the lifetime of the application.

### Write-Side Model

This is the "domain" of the DDD application. We still have aggregate roots, entities, repositories, and domain events. The implementation of some of these components differs greatly from other architectures.
#### Aggregates

The entire aggregate - the aggregate root and all entities and value objects - is stored as a stream of events. Since every change to the state is captured in an event, we don't need to store anything else.

##### Implementation

In an event sourced domain, each aggregate root should inherit `EventSourcedAggregate`. It contains all of the necessary internal methods and interface implementations to support an event sourced aggregate root. Additionally, a protected parameterless constructor is required for creating an instance when reading from the database. Nothing else is required.

```csharp
    public class EarningsCurve : EventSourcedAggregate
    {
        protected EarningsCurve()
        { }
    }
```

Of course, the example above does nothing. In the example below, we add a constructor that will create a curve and add a single, blank development period.

```csharp
    public class EarningsCurve : EventSourcedAggregate
    {
        private readonly List<decimal?> factors = new List<decimal?>();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="name">The name of the curve.</param>
        /// <param name="description">A description of the curve. This parameter can be null.</param>
        public EarningsCurve(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainException(400, $"'{nameof(name)}' is required.");
            }

            var curveId = Guid.NewGuid();
            RaiseEvent(new EarningsCurveCreated(curveId, name, description));
            RaiseEvent(new DevelopmentPeriodAdded(curveId, 1, null));
        }

        protected EarningsCurve()
        { }

        public Guid CurveId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public IEnumerable<DevelopmentPeriod> DevelopmentPeriods => factors.Select((factor, index) => new DevelopmentPeriod(index + 1, factor));


        private void Apply(EarningsCurveCreated evt)
        {
            Key = evt.CurveId;
            CurveId = evt.CurveId;
            Name = evt.Name;
            Description = evt.Description;
        }

        private void Apply(DevelopmentPeriodAdded evt)
        {
            factors.Insert(evt.PeriodNumber - 1, evt.Factor);
        }
    }
```

An event sourced aggregate can have three types of members. 
* The public methods, including public constructors, contain all the branching logic, throw exceptions when necessary, and raise events. They may also query other services through double-dispatch. They do _not_ set any properties or fields or manipulate collections. The must not change the state in any way execpt by raising events.
* The private apply methods only set properties and fields or manipulate collections. They do not contain branching logic, throw exceptions or raise events.
* Properties and fields store the current state. These don't need to be public since all queries are performed against read models, not the write model. 

> **Rules**:
> * An inheritance heirarchy at the aggregate root level is not supported.
> * The `Apply` methods must be non-public and accept exactly 1 parameter.
> * Each type of domain event raised by an aggregate must have a corresponding `Apply` method in that aggregate.
> * A domain event must be raised by only one aggregate. You should not have a `NameChanged` event raised by an `EarningsCurve` and some other type of aggregate.
> * Mutable `static` and `AsyncLocal` fields and properties are forbidden because the shared state does not exist within the aggregate's events.

When an event is raised using the `RaiseEvents` method of the base class, the appropriate `Apply` method is called.

In the previous code example, `EarningsCurveCreated` is applied, followed by  `DevelopmentPeriodAdded`. No properties or fields are changed outside of the `Apply` methods.

#### Repositories, Persistence, and Table Schema

In the previous example, when the aggregate is saved to the database, the repository will serialize and write the two events: `EarningsCurveCreated` followed by `DevelopmentPeriodAdded`.

When the aggregate is loaded from the database, the repository will create a new instance of `EarningsCurve` using the protected constructor, deserialize the two events, call `Apply(EarningsCurveCreated)` and then `Apply(DevelopmentPeriodAdded)`. The state of this new instance will be identical to the one we saved.

| Use                 | Name           | Data Type        | Nullable | Notes                                                 |
|---------------------|----------------|------------------|----------|-------------------------------------------------------|
| _DB only_           | sid            | int              | not null | PK, identity, makes the database efficient            |
| _Unique Constraint_ | tenant_id      | string           | not null | Multi-tenant by default                               |
| _Unique Constraint_ | aggregate_id   | guid             | not null |                                                       |
| _Unique Constraint_ | version        | int              | not null | The event's order within the aggregate                |
| _Event Sourcing_    | type           | string           | not null | The event's type's full name                          |
| _Event Sourcing_    | payload        | jsonb            | not null | The serialized event                                  |
| _Audit & Metadata_  | correlation_id | guid             | not null | Correlation id of the activity that caused this event |
| _Audit & Metadata_  | created_on     | date time offset | not null | Timestamp of when the event occurred                  |
| _Audit & Metadata_  | created_by     | string           | not null | The login (email) of the user who caused this event   |
| _Concurrency_       | etag           | string           | not null | etag of the activity that caused this event           |

> It's not necessary to create any additional tables for storing entities. There's no need for the complex foreign keys, indexes and unique constraints that go in to a traditional 3rd normal form database design. The write model schema is incredibly simple.


##### Reading an Aggregate

To read any aggregate from the database, the repository follows these steps:
1. Create a new instance of the aggregate root using the protected parameterless constructor.
1. Query the database for the aggregate's events sorted by the order they occurred (the `version` column).
1. For each event returned from the database, pass the event to the non-public `Apply` method.
1. Set the `Version` property on the aggregate to the last event's `version`.

##### Saving an Aggregate

1. An aggregate's entire state is stored as a stream of serialized events in the order they occurred on that aggregate. This order is determined by the `version` field. The first event is version 1, the second is version 2, and so on.
1. To save an aggregate's new state, we insert new events in to the database. It's strictly an append-only model.
1. In addition to the standard etags, the `version` column is used to detect concurrency violations.
1. In addition to publishing events through the Outbox to RabbitMQ, we also publish events directly through MediatR to update the read models. 

Here's an example of a concurrency violation: 
1. Alice and Bob both load an earnings curve at `version` 4. That is, the earnings curve has 4 events applied, with `version`s 1, 2, 3, and 4.
1. Alice corrects a typo in the description of the curve.
1. Bob adds another development period to the end of the curve.
1. Alice inserts her new `EarningsCurveDescriptionChanged` event with a `version` of 5, but hasn't yet committed the transaction.
1. Bob inserts his new `DevelopmentPeriodAdded` event with a `version` of 5, but hasn't yet committed the transaction.
1. Bob commits his transaction.
1. When Alice commits her transaction, she gets a unique constraint violation. Bob has already committed a row with the same `tenant_id`, `aggregate_id` and `version`. Alice's transaction rolls back.

##### Advanced: Automatic Concurrency Conflict Resolution

> **Warning:** Earnings does not support automatic concurrency conflict resolution at this time because it conflicts with the transaction boundaries set in our mediator pipeline. Adding conflict resolvers is only possible after replacing the transaction behavior with a true unit of work implementation. 

In the example above, Bob and Alice both attempted to apply an event at `version` 5. Bob committed first, so he won. Alice must retry her entire operation.

However, like in the example above, it's possible the two operations don't actually conflict in terms of business rules. Alice's decision to fix the description's typo is not impacted by Bob adding development periods. There is no real-world conflict here, so let's not burden Alice with redoing her change.

Instead of forcing Alice to deal with the error and make her description change again, we can use a conflict resolver to automatically determine that it's safe to allow Alice's description change anyway.

> **Note**: Automatic conflict resolution elevates concurrency issues to business concerns. Some work can happen in parallel. Other work cannot. This is a business decision. If you have difficulty getting decisions from your SMEs or project sponsors, you can wait until concurrency conflicts start to cause user frustration. Automatic conflict resolution can be added at any point in the development process.

The conflict detector interface is injected in to the repository and may look like this:
```csharp
public interface IDetectConflicts<TAggregate>
{
    public bool HasConflict(IEnumerable<IDomainEvent> committedEvents, IEnumerable<IDomainEvent> newEvents);
}
```

1. The database throws a unique constraint violation just as before, but this time it is caught in the repository's `Save` method.
1. The repository begins a new database transaction.
1. The repository loads the events committed since Alice loaded the curve. Alice loaded `version` 4 of the curve, so the resolver loads Bob's `DevelopmentPeriodAdded` event at `version` 5. 
1. The repository asks the conflict detector it compare Bob's `DevelopmentPeriodAdded` event with Alice's `EarningsCurveDescriptionChanged` event and decide if a true conflict occurred. If so, the original unique constraint violation is thrown and Alice must redo her work.
1. If no conflict is detected, Alice's `EarningsCurveDescriptionChanged` event is renumbered as `version` 6 and inserted in to the table.
1. The database transaction is committed. If another unique constraint violation occurs, this process may be repeated, or you may give up and choose to force Alice to try again, perhaps later when this curve isn't being worked on so heavily.

### Read-Side Model

> **Note**: Many implementations of CQRS with event sourcing introduce eventual consistency between the read model and write model. This is typically done for improved scalability, but introduces some complexity in the user interface design. This implementation doesn't do that. Instead, the read model and write model are updated in the same database transaction.

Here's an example read model that maintains a list of earnings curves:
```csharp
public class UxEarningsCurveListQueryReadModelHandler :
    ReadModelHandlerBase,
    INotificationHandler<EarningsCurveCreated>,
    INotificationHandler<EarningsCurveRenamed>,
    INotificationHandler<EarningsCurveDescriptionUpdated>
{
    public UxEarningsCurveListQueryReadModelHandler(IActivityContext activityContext)
        : base(activityContext)
    {
    }

    public async Task Handle(EarningsCurveCreated notification, CancellationToken cancellationToken)
    {
        // Insert a row in ux.earnings_curves_list
    }

    public async Task Handle(EarningsCurveRenamed notification, CancellationToken cancellationToken)
    {
        // Update the name on a row in ux.earnings_curves_list
    }

    public async Task Handle(EarningsCurveDescriptionUpdated notification, CancellationToken cancellationToken)
    {
        // Update the description on a row in ux.earnings_curves_list
    }

    internal override async Task ClearData()
    {
        // Delete all rows in ux.earnings_curves_list
    }
}
```

When the repository saves an aggregate, new events are published to MediatR as notifications and handled by any read model handlers that are interested in that event type.
```csharp
public abstract class DomainEventBase : IDomainEvent, INotification 
{ }

public class EarningsCurveNameChanged : DomainEventBase
{ ... }
```

##### Adding and Rebuilding Read Model

As an application is developed over time, new features are added, typically requiring a new read model. In traditional applications, this can often mean launching the feature without any historical data and only using the new feature going forward.

For an event sourced application, the entire history is always available. We have the ability to add a new read model and play the entire history of events through that read model to bring it up to the current point in time.

Additionally, we also support rebuilding a read models after breaking changes and bug fixes. For this reason, a read model handler requires the `ClearData()` method to reset the read model.

Rebuilding a read model is done with a simple data migration. This migration is forward-only, since it is a destructive process.

```csharp
/// <inheritdoc />
[Migration(2021041601)]
public class Migration2021041601RebuildUxTimeline : ForwardOnlyMigration
{
    /// <inheritdoc />
    public override void Up()
    {
        this.RebuildReadModel<UXTimelineQueryReadModelHandler>();
    }
}
```

It will perform the following steps:
1. Clear any existing data in the read model using the read model handler's `ClearData()` method.
1. For the set of event types handled by this read model, query all of those events from the write-model tables in the order they occurred. This includes metadata such as the event's `created_on` timestamp and access token data.
1. For each of those events, instantiate the read model with an `IActivityContext` forged from the event metadata, and pass the event to the appropriate `Handle` method on the read model.
