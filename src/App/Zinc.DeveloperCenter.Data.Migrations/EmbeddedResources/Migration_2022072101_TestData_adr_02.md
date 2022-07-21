# ADR-0002: Organize Solution into Layers

Date: 2018-10-08

## Status

Accepted

## Context
Development work of any size and consequence requires a strategy for organization. Specifically, we need to address the following challenges:

1. It's often been said that the only constant is change. When a new technology - or simply a newer version of an existing one - comes along one of the barriers to adoption is often the amount of work required to retrofit and test existing code. We need to organize our work in a manner that helps to minimize the "blast radius" of technology changes and/or updates.
2. Maintaining code that is poorly organized is much more difficult and error-prone. We need to organize our code in a manner that's consice and easy to understand.
3. Automated code tests (unit tests, integration tests, etc.) are a critical feature of world-class code. We need to make it possible to define the discreet units, easily mock dependencies, and design with testing in mind.

## Decision
We will organize the solution into layers as follows:

### Domain
If the microservice is suited for Domain Driven Design (DDD), the domain objects (Entities, Aggregate Roots, Value Objects, Repository interfaces, and so forth) will be captured in true Plain Ol' Class Object (POCO) fashion in this layer. Here we will encapsulare the business logic as modeled for the domain and nothing else. This layer will not take a reference to any other layer in most cases.

### Data
Persistence concerns, if any, should be captured in this layer. This includes Repository implementations (if applicable), O/RM code, and so forth. If a domain layer exists, this layer will usually take a reference to it in order to persist the domain objects. Otherwise, this layer will generally take a reference to the layer where the models or data transfer objects (DTO's) exist.

### Application
This layer is the outward facing interface layer that defines the service or application. Commands and queries issued to the application or service are defined and fulfilled here. This layer will usually take a reference to the data and/or domain layers (if applicable). Nearly every service or application will include this layer.

### Presentation (Console, Web, etc.)
When users and/or other services need to interact with the service or application being built, they will do so via a presentation layer. Usually, this will be in the form of a console application, a web application, or an API. This layer usually takes a reference to the application layer. Nearly every service or application will include this layer.

### Other
The layers presented above are by far the most common. However, there will be the need to create other layers occasionally. For example, let's say you're writing a microservice that stores a file. You may choose to write that service with some modularity in mind and support multiple storage mechanisms such as AWS S3 and NFS. As such, you might create interfaces in the application or domain layers that are fulfilled by implementations in new layers. For instance, you might create a `storage.implementation.aws` and `storage.implementations.nfs` layers.

## Consequences
* Applications and services may take slightly longer to create due to the additional "yak shaving" required to set up the individual projects within the solution.
* It will be easier to identify where the code is that is responsible for different things. For example, if a service defines a web API then it is easier to isolate that code from the business logic which is defined in another layer altogether.
* Build and test times will increase slightly.
* Upgrading and/or replacing technology will be safer and easier since, in many cases, those changes only impact one or two layers. For example, upgrading from Entity Framework X.1 to EF X.2 is unlikely to touch more than the data layer and, perhaps, the application or presentation layer.
* Automated testing will be easier since the code that requires high coverage - for example the domain and application layer - are isolated from the ones that require less coverage such as the presentation layer.
