# ADR-0001: Devloper Center GitHub API

Date: 2022-06-13

## Overview

During consideration of which GitHub API to use for Developer Center, we outlined the requirements needed of our selected API. We also compared the pros and cons of both existing GitHub APIs.

### Requirements
* Fast query times for repo and file access.
* Scaleability as the number of RedLine repos and ADRs grows.

## Status
Accepted.

## Context
Two options for GitHub APIs currently exist: REST and GraphQL. 
We thus detailed each option with a list of pros and cons.

### Option 1. REST

#### Pros
* The REST framework is considered the industry standard for API functionality. The team is likely more familiar with this framework. The format of REST queries will be very easy for the team to learn and read.

* REST contains specific error messaging, which will make debugging easier. Specifically, REST provides descriptive error messages for invalid, incorrect, and unsupported queries. GraphQL simply returns nothing when a query does not work.

* We have a proof of concept for pulling the data we need utilizing this API. This data includes repo titles and ADR files.

* There is a much greater amount of existing documentation for the REST API. This will make the process of building the DC much easier if we select this API.

#### Cons
* The REST API is considered v3, while the GraphQL is considered v4. It may be in our best interest to utilize the newest technology.

#### Conclusion
Selected due to simplicity and REST use in other parts of the RedLine application. A proof of concept also exists for this API, so we feel more comfortable using it.

### Option 2. GraphQL

#### Pros
* GraphQL is designed to access multiple resources simultaneously. In situations where REST would make multiple queries, GraphQL can accomplish the same result with one query.

* The GraphQL API is generally considered to be faster than the REST API. While we will not have a large number of repos, we will possibly have a large number of ADRs. Thus, speed is a small bonus for the GraphQL API.

* Many companies, such as Facebook, are migrating their REST APIs to GraphQL APIs. This suggests that GraphQL is a better choice than REST.

#### Cons
* Complexity is a factor with the GraphQL API. Given that it is more concise by nature, there are more rules that will be needed to followed. The learning curve will also be steeper. However, while using GraphQL may be more complex, it is likely not far too complex to understand for the scope of this app.

* There is less documentation and tooling for GraphQL. This is due to the fact that the REST API has been around longer and applied more in industry.

* We have not performed any testing for this API, nor is there a proof of concept to show that this API successfully performs the data retrieval we need.

#### Conclusion
Not selected due to complexity and lack of proof of concept.

## Decision
The REST API was selected. In general, we are more comfortable with this API given its simplicity over GraphQL and its use in other parts of the RedLine application. The plethora of existing documentation for the REST API was also a big factor, as there is much more support for this API over GraphQL.

## Consequences
