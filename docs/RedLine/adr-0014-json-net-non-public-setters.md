# ADR-0014: Support writing to non-public setters by default in Json.NET

Date: 2020-07-28

## Status

Accepted

## Context

The contract resolvers provided out of the box by [Json.NET](https://github.com/JamesNK/Newtonsoft.Json) do not support writing to non-public property setters.  This is not an issue with data transfer objects since they generally have public setters, but we have run into scenarios where objects following the object-oriented principle of encapsulation do not have public setters.  An example would be deserialzing aggregates from a jsonb column in PostgreSQL.  This has been solved in more than one way in our GitHub repositories.  With a recent need to deserialize objects with non-public property setters in web integration tests, it's an opportunity to standardize the approach.

Additional information regarding contract resolvers, caching, and performance can be found here:

* [ContractResolver](https://www.newtonsoft.com/json/help/html/ContractResolver.htm)
* [Caching in DefaultContractResolver and CamelCasePropertyNamesContractResolver](https://stackoverflow.com/a/33558665)
* [CamelCaseNamingStrategy vs CamelCasePropertyNamesContractResolver](https://stackoverflow.com/a/57778704)

### Options

1. Have a static default settings in `RedLineJsonSerializationSettings` that supports non-public property setters.
   * Pros
     * Default settings are already in place, just need to modify them to support non-public property setters.
     * Since caching is done per instance of the `DefaultContractResolver` (or derived types) in [Json.NET](https://github.com/JamesNK/Newtonsoft.Json), having a static defaults follows the performance recommandation for resuing contract resolvers.
     * Consistent behaviour with our most commonly used object-relational mapper Dapper, so spends less time investigating why a property is deserialized in one case but not another.
   * Cons
     * Using a NuGet package to support non-public setters.  The implementation is straightfoward if we did not want to take this dependency.

2. Have a [`ContractResolver`](https://www.newtonsoft.com/json/help/html/ContractResolver.htm) that specifically supports non-public property setters and use as needed.
   * Cons
     * Non-consistent behaviour depending on which contract resolver is used.
     * Possiblility of not resusing the contract resolver and thus not benefiting from caching, there is already an example in Krypton.Forms.

## Decision

Modify the default settings in `RedLineJsonSerializationSettings` that supports non-public property setters.

## Consequences

* Continue to use default settings provided in `RedLineJsonSerializationSettings`.
* Less time developers investigating why properties are not being populated.
* Additional dependency to [JsonNet.ContractResolvers](https://github.com/danielwertheim/jsonnet-contractresolvers)
* A developer is able to override the default settings:
  * Use the overloaded `JsonConvert.DeserializeObject` that takes `JsonSerializerSettings`
  * Use an `ActionFilter` or `JsonResult` to specify custom `JsonSerializerSettings`
