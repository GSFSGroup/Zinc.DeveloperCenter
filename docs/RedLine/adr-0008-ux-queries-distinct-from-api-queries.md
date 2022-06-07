# ADR-0008: UX Queries distinct from Api queries

Date: 2020-04-13

## Status

Accepted

## Context
We have endpoints created for individual screens in the UI, called UX endpoints. The idea is having a clear path from the screen a user looks at, to the endpoint it hits, to the handler that retrieves that data.

Confusion arose because:
* It is not clear when a UX endpoint should be used instead of a normal Api endpoint.
* The aggregate Repositories get overloaded with methods which were not originally intended for the UX query's needs.
    * UX queries can also become coupled because of the those overloaded methods.
* Organization of the code does not immediately imply the use or existence of UX endpoints or queries.
    * How do I know that a method on the repository is intended for an UX endpoint?

## Proposal

If we explicitly define UX capabilities throughout the app, it will be easier to understand its intended use, and maintain existing UX endpoints.

The UX scheme should propagate throughout the request's lifetime, and relate back to the calling screen. The ClientApp should call a UX endpoint, which calls a UX query, which returns UX data.

### The ClientApp should call a UX endpoint

The `UX*Controller` should clearly indicate that it only supports endpoints for UX. This means replacing "api" in its base route with "ux", e.g. `[Route("ux/v{version:apiVersion}/{tenantId}")]`. The ClientApp's core `backendService` will support a new `query()` method which has identical behavior to `get()`, save calling a UX endpoint.

### The UX endpoint should call a UX query

This step is the same as currently performed.

### The UX query should return UX data

The `UX*Handler` should read data using a [query object pattern](https://stackoverflow.com/a/17838073) to isolate it from other queries and domain functionality. This is a clear strategy to separate our reads from operations which might write data. To use the pattern, a new `IUXRepository` will exist in the `Data` layer of the project which accepts instances of `IUXDataQuery` to retrieve data from the database specific to the UX query. The new instance should follow naming conventions, e.g. `UX*DataQuery`.

## Solution organization

The solution should be organized, so it is clear when a UX endpoint is being used.

### The ClientApp layer

The ClientApp should organize all screens as siblings.  If the screen needs to call endpoints for data, the screen will have its own service, not a shared service.  The service will call `backendService.query()` for all queries, unless domain operations are needed.

```
/app
../modules
../screens
..../screen-name
....../screen-name.component.ts
....../screen-name.service.ts
../shared
```

The screen's name will be prepended with UX once it touches the [Web Api layer](#the-web-api-layer).  The method names on the service should match the name of the action in the [Web Api layer](#the-web-api-layer).

*`screen-name.service.ts`*
```typescript
allSamples(): Observable<Sample[]> {
    return this.backendService.query<Sample[]>('samples');
}
```

### The Web Api layer

The `UX*Controller` will be named according to the [screen](#the-clientapp-layer). The action methods in the controller will be named according to the calling screen's service.

*`UXScreenNameController.cs`*
```c#
public async Task<IActionResult> AllSamples()
{
}
```

### The Application layer

The `UX*Query` and `UX*Handler` will be organized according to the [endpoint](#the-web-api-layer)'s `UX*Controller`. The `UX*Query` and `UX*Handler` will be named according to the calling [endpoint](#the-web-api-layer)'s name.

```
Application.Queries/
../UXScreenName
../UXScreenName/UXAllSamplesQuery.cs
```

### The Data layer

The `UX*DataQuery` object will be organized according to the [endpoint](#the-web-api-layer)'s `UX*Controller`. The `UX*DataQuery` will be named according to the calling [endpoint](#the-web-api-layer)'s name.

```
Data.UXDataQueries/
../UXScreenName
..../UXAllSamplesDataQuery.cs
```

## Decision

We will use distinct layers for UX queries and Api queries.

## Consequences

* There will be a clear distinction between sql queries for UX endpoints vs Api endpoints.
* The url of the api can indicate whether or not the underlying implementation is a UX query.
* The Aggregate `IRepository` will not be littered with `UXDataQuery`s.
* There is another explicit layer of moving parts to learn and understand the solution in code.
* A clear naming distinction provides a path to follow a screen all the way down to the sql being run for that screen.
* There is potential for enforcing Read before Write if UX endpoints do not return ETag headers.
