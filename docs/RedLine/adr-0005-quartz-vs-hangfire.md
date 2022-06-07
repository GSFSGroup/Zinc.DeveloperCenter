# ADR-0005: Quartz vs. Hangfire
Date: 2020-14-23

## Status

Accepted

## Context
There are two popular and regularly used scheduling frameworks in the .Net ecosystem Quartz and Hangfire and there is Timed HostedService, an alternative approach
that is useful in some scenarios.  We need to choose one for the purpose of scheduling domain event publishing. 

The key features of all three schedulers have been layed out and aligned below as you see Quartz and Hangfire are largely at parity with one another when you include
their full software ecosystems. The one differentiator is scheduling resolution which Quartz.net has an edge on.

Where Timed-HostedService shines is in its initial simplicity; it's an asynchronous class that runs in the background of a web service and it uses .Net's existing Timer
mechanism to manage its delays.  The down side of this simplicity is the burden of managing concurrency. Timer does not wait for work to complete
before starting the next timed sequence. So, if the operation completed on each timed cycle takes longer than the timer a pattern will emerge where increasing numbers of
operations are happening simultaneously and potentially interfering with one another.  There are workarounds for this issue that could add to development, testing
and maintenance time required.

### Runtime Environments
|Features:                                                                                                      |Hangfire |Quartz.net          |Timed-HostedService |
|---------------------------------------------------------------------------------------------------------------|---------|--------------------|--------------------|
|Can run embedded within another free standing application?                                                     |        x|                   x|                   x|
|Can run as a stand-alone program (within its own .NET virtual machine instance), to be used via .NET Remoting? |        x|  with CrystalQuartz|                    |
|Can run as a stand-alone program (within its own .NET virtual machine instance), to be used via Web Frontend?  |         |                   x|                    |
|Can be instantiated as a cluster of stand-alone programs (with load-balance and fail-over capabilities)        |        x|                   x|                    |

### Job Scheduling
|Features:                                            |Hangfire      |Quartz.net |Timed-HostedService |
|-----------------------------------------------------|--------------|-----------|--------------------|
|Cron Tab format?                                     |             x|          x|                    |
|Extended Cron format time of day to the millisecond? |              |          x|                    |
|Time delay to the millisecond?                       |              |          x|                   x|
|Can exclude holidays from registered calender?       |  can be coded|          x|                    |
|Support for recurring schedules                      |             x|          x|                   x|

### Execution
|Features:                     |Hangfire |Quartz.net |Timed-HostedService |
|------------------------------|---------|-----------|--------------------|
|Support for automatic retries |        x|          x|                    |

### Persistence
|Features:                                       |Hangfire |Quartz.net          |Timed-HostedService |
|------------------------------------------------|---------|--------------------|--------------------|
|Support for non-volatile job storage via SQL.   |        x|                   x|                    |
|Support for non-volatile job storage via Redis. |        x|Quartz.RedisJobStore|                    |

### Clustering
|Features:                         |Hangfire |Quartz.net |Timed-HostedService |
|----------------------------------|---------|-----------|--------------------|
|Support for Fail-over clustering? |        x|           |                    |
|Support for load balancing?       |        x|           |                    |
|Uses for distributed locks?       |         |          x|                    |

## Decision
The decision has been made to choose the use of Quartz.net for scheduling publications.  

## Consequences
Going with Quartz.Net brings us several important things
1. High scheduling resolution will allow us to poll at the correct interval for each service
2. A simple mechanism to prevent multiple of the same jobs from running concurrently in the same process. 
3. A single simple scheduling mechanism that we can use for any scheduling activity that we need in the future.

