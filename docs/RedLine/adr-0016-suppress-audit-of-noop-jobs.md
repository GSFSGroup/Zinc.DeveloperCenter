# ADR-0016: Suppress audit of NoOp jobs

Date: 2021-03-04

## Status

Proposed

## Context

All of our apps have the dispatch job, which runs frequently but usually does no work. We have other app-specific jobs that also run frequently and usually do no work. With the new template, these jobs go through the mediator pipeline, which means they are audited. This can lead to a lot of noise in the audit log for jobs that do nothing.

## Decision

We will suppress the job's audit messages when a job runs but doesn't perform any real work. 

1. Jobs that fail should throw an exception, which WILL be audited.
1. Jobs that perform work and succeed should return `JobResult.OperationSucceeded`, which WILL be audited.
1. Jobs that don't perform any work should return `JobResult.NoWorkPerformed`, which will NOT be audited.

## Consequences

* There is no mechanism for a job to return data beyond the two values above. All of our jobs are triggered by simple recurring schedules. There are no scenarios in our current apps where consuming data in a job result is necessary.
* This change to the audit behavior does not affect the logging behavior.
* Any mediator commands or queries performed inside a job will be audited as usual.
