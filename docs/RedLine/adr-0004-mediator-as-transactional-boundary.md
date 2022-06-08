# ADR-0004: Mediator as transactional boundary

Date: 2020-04-20

## Status

Accepted

## Context

As we dug in to ADR-0003 to bring Outbox to all contexts, not just NServiceBus, we discovered that our transaction boundary was only enforced on web requests.

Internally, NServiceBus wraps each message context in a `ReadCommitted` `TransactionScope`. https://docs.particular.net/transports/transactions

This means our current transaction boundaries are as follows:
* For web, the entire HTTP request is wrapped in a single transaction.
* For NServiceBus, the handling of a message is wrapped in a single transaction.
* For Hangfire jobs, there is no transactional boundary defined.

Our transaction boundaries are inconsistent or non-existent.

Transaction isolation options:
* `ReadCommitted`: Unable to read dirty (uncommitted) data.
* `Snapshot`: Same as read-committed, plus repeatable reads and some rudimentary optimistic concurrency checking.
* `Serializable`: Same as Snapshot, plus prevents phantom reads. Easily leads to deadlocks.

https://www.postgresql.org/docs/current/transaction-iso.html

It has been confirmed experimentally that opening and completing a transaction scope without performing a transaction of any type will not raise an error, so this code is safe 
in stateless services and services that use non ACID compliant storage.

## Decision

Move the transaction boundary to the mediator pipeline. It should be registered after metrics, logging and authentication, but before audit, since audit may use a database connection to write to the outbox.

The transaction isolation will be `Snapshot`, known as `REPEATABLE READ` in PostgreSQL.

## Consequences

1. Transactions are consistent everywhere the mediator is used, regardless of context.
2. "Npgsql.PostgresException (0x80004005): 40001: could not serialize access due to concurrent update" will need to be converted in to concurrency exceptions.
3. In cases where database work should be done on a failure, wrap that code in a `Suppress` `TransactionScope`. This has not changed from the current state  of our code, but it is worth calling out here.
4. The choice of Snapshot Isolation requires a bit of mental mapping when working in DBMSs other than SQL Server with the canonical example here being 'REPEATABLE READ' in both PostgreSQL and MySql.
    * it should be noted that `REPEATABLE READ` itself is not one of the options available for TransactionScope isolation level.
    * the Database vendors added the `Repeatable Read` to their implementation of Snapshot to conform to Microsofts nomenclature.
