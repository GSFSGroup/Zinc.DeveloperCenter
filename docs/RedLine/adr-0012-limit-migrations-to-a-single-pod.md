# ADR-0012: Limit migrations to a single pod

Date: 2020-06-19

## Status

Accepted

## Context

Originally, this was Data Catalog ADR-0009.

When deploying, Kubernetes spins up a single pod and waits for the pod's health check to report healthy before spinning up additional pods. Our current health check doesn't include the status of migrations, so additional pods may spin up before the first pod has completed all migrations. Additionally, Elastic doesn't participate in transactions of any kind, including those created by Fluent Migrator. All together, this means that multiple pods may be applying migrations to Elastic simultaneously, potentially even out of order.

### Options

1. Add a health check the state of migrations. If migrations take too long to run, Kubernetes may consider the pod unhealthy, kill it, and start a new one.
1. Use the Kubernetes init containers to apply migrations before the application pods start. https://kubernetes.io/docs/concepts/workloads/pods/init-containers/
While this may be the best option, it would require significant and disruptive changes for local development, CI/CD, and infrastructure monitoring.
1. Block any other instance of the application from applying migrations while the first pod is still applying migrations.  
 
## Decision

Use database locks to coordinate instances of the application. In PostgreSQL, the best option I found is session advisory locking. https://www.postgresql.org/docs/9.4/explicit-locking.html.

* Advisory locks are managed by the application, not the database engine. Unlike table or row locks, they don't apply to a specific database object.
* Advisory locks are scoped to a database and identified by a 64-bit signed integer (`long` in C#).
* The lock can be explicitly released by the application. 
* When the connection that created a lock is closed, the lock is automatically released.

To avoid interfering with Fluent Migrator, we open a new database connection just for locking and create our advisory lock before the migrations begin. Once the migrations complete, the lock is released and the connection is closed.

Advisory locks are not released when a connection is returned to the connection pool. For this reason, we will disable pooling on the connection that manages the lock.

## Consequences

* Additional pods will not fully start until migrations have been applied.
* Advisory locks are not released when a connection is returned to the connection pool. Only dropping the physical connection will release the lock.
* If a hard crash occurs during the migrations, it may take some time before PostgreSQL recognizes the connection has dropped and releases the lock automatically. However, a simple unhandled exception should gracefully release the lock during the `finally` block.
