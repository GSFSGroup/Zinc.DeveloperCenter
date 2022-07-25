# ADR-0003: Implement the Outbox Pattern for Web Requests

Date: 2020-04-16

## Status

Accepted

## Context

When processing a business operation, it is often a requirement that we will need to communicate with an external system. For example, when placing an order in an e-commerce system, we'll need to collect the customer's order information, send e-mails, notify shipping vendors, etc.

Due to the distributed nature of the system we want to insure that:
* Events are published and Data is persisted within the same transaction scope so that
    * When data persistance fails messages are not published.
    * When there is an error raised by the domain, incomplete or invalid data is not published or persisted.

NServiceBus doesn't have this particular functionality in the context of web requests, it's only when handling messages. In order to ensure we have this level of consistency for all operations, we will need to implement this pattern ourselves.

## Options

1. Attempt to extend NServiceBus functionality so it can be used in the context of Web requests and Hangfire Jobs. The NServiceBus implementation of the outbox pattern is heavily tied to the NServiceBus messaging pipeline, which is not available during web requests or hangfire jobs.
2. Implement our own outbox for web and HangFire contexts.
3. Implement our own outbox for use everywhere, regardless of context.

## Decision

We will use our own outbox implementation everywhere, regardless of context.

We will use an implementation similar to the one described here: http://www.kamilgrzybek.com/design/the-outbox-pattern/.

Following diagram shows the sequence of calls for saving events in the outbox table, in case of an aggregate creation.

![Outbox](./adr-0003-outbox-insert.svg)

Processing of the events in outbox table is to be done by another process or thread. Details of processing will be solidified in a future ADR.

## Consequences

1. Since we will be using our Outbox implementation everywhere, we will _not_ enable NServiceBus Outbox. It is not necessary.
2. Consumers of messages, particularly domain events, should expect at-least-once delivery. Some message duplication is possible. At-least-once delivery is implicit in our current messaging system as well, but worth calling out here.
3. We no longer need to hold domain events in memory. These can be written to the outbox table as soon as the domain emits them. ADR-004 addresses the Unit of Work / Transaction concerns.
4. Sending of failure audit messages require some extra care, since they are raised within the scope of a unit of work / transaction that will be rolled back. We can either bypass the Outbox and send these directly to RabbitMQ, or we will need to use a `Suppress` `TransactionScope` to write them to the Outbox. There should be a standard, easy to use method that handles creating this scope and writing these messages to the outbox.
5. The background job that polls for messages must use `READ COMMITTED` transaction isolation or better, or risk sending messages before the domain state changes have been written to the database. Failing to use at least `READ COMMITTED` will lead to a distributed race condition.
6. The polling should first update a batch of unsent messages to claim them in some way so that other pods don't also attempt to send those messages. This will minimize the chance of message duplication at the consumer(s). This claim needs to expire in the event of a pod failure, so that another pod will eventually send them.
7. Sent messages will need to be cleaned up, either as part of the sending process (after Rabbit acknowledges the messages, to avoid message loss), or later as a separate background job.
8. A health check is required for monitoring the oldest message in the outbox table. This check should use some sort of caching to prevent overloading the database or running up the cloud bill from too-frequent polling.

Related ADRs: 
* ADR-0004 addresses the TransactionScope mediator behavior changes for Unit of Work / transactional boundary.
