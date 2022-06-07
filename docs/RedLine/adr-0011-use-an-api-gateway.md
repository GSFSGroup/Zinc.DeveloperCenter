# ADR-0011: Use an API Gateway

Date: 2020-05-20

## Status

Accepted

## Context
How do the clients of a microservices-based application access the individual services?

### Forces
* The granularity of APIs provided by microservices is often different than what a client needs. Microservices typically provide fine-grained APIs, which means that clients need to interact with multiple services. For example, as described above, a client needing the details for a product needs to fetch data from numerous services.
* Network performance is different for different types of clients. For example, a mobile network is typically much slower and has much higher latency than a non-mobile network. And, of course, any WAN is much slower than a LAN. This means that a native mobile client uses a network that has very difference performance characteristics than a LAN used by a server-side web application. The server-side web application can make multiple requests to backend services without impacting the user experience where as a mobile client can only make a few.
* The number of service instances and their locations (host+port) changes dynamically
* Partitioning into services can change over time and should be hidden from clients
* Services might use a diverse set of protocols, some of which might not be web friendly

## Decision
Implement an API gateway that is the single entry point for all clients. The API gateway handles requests in one of two ways. Some requests are simply proxied/routed to the appropriate service. It handles other requests by fanning out to multiple services.

Rather than provide a one-size-fits-all style API, the API gateway can expose a different API for each client. For example, the Netflix API gateway runs client-specific adapter code that provides each client with an API that’s best suited to its requirements.

The API gateway might also implement security, e.g. verify that the client is authorized to perform the request.

### Solutions
These are the solutions that were considered:

* Amazon API Gateway
* Apigee
* Tyk.io
* Kong

Ultimately, we made the decision to use Kong for the following reasons:

* Kong is a well-known, open source (Apache 2.0) solution
* Unlike other vendors, Kong provides a free tier
* Solutions such as the AWS API Gateway are bound to a cloud provider
* AWS API Gateway makes heavy use of Lambda and EC2 which drives considerably higher cloud costs
* Apigee is far too complex for simple use cases (a minimum of 9 nodes and you must run Cassandra, Zookeeper, Postres, and other infrastructure first)
* Tyk.io is relatively new and immature, but worthy of consideration in the future
* Kong provides a Kubernetes ingress controller making it the easiest to implement by far
* Datadog provides a Kong integration out of the box
* Kong integrates with Okta (using the enterprise tier)
* Kong has an extensive library of plug-in solutions for things like JWT management
* Kong is built on NGINX, one of the most popular open source HTTP proxy servers

It should be noted that Mulesoft and Dell Boomi were briefly reviewed, but rejected early in the decision making process due to cost, implementation complexity, and concerns about Infrastructure as Code (IaC) feasibility.

## Consequences
Using an API gateway has the following benefits:

* Insulates the clients from how the application is partitioned into microservices
* Insulates the clients from the problem of determining the locations of service instances
* Provides the optimal API for each client
* Reduces the number of requests/roundtrips. For example, the API gateway enables clients to retrieve data from multiple services with a single round-trip. Fewer requests also means less overhead and improves the user experience. An API gateway is essential for mobile applications.
* Simplifies the client by moving logic for calling multiple services from the client to API gateway
* Translates from a “standard” public web-friendly API protocol to whatever protocols are used internally

The API gateway pattern has some drawbacks:

* Increased complexity - the API gateway is yet another moving part that must be developed, deployed and managed
* Increased response time due to the additional network hop through the API gateway - however, for most applications the cost of an extra roundtrip is insignificant.
