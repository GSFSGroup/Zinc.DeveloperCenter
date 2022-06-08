# ADR-0018: Catching errors client-side

Date: 2021-05-17

## Status

Accepted

## Context

The client app needs to handle errors. Historically, the template uses a [HttpInterceptor](https://angular.io/api/common/http/HttpInterceptor) to handle errors from the server. The interceptor captures errors, performs some action such as displaying a snackbar notification, and returns an [`EMPTY` observable](https://rxjs.dev/api/index/const/EMPTY). The template also uses an [ErrorHandler](https://angular.io/api/core/ErrorHandler) to capture any unhandled client-side errors and display a snackbar notification.

Because the `HttpInterceptor` swallowed all server errors, eventually a process was added that allowed screens to register to handle specific server errors. The `HttpInterceptor` would continue to handle most errors, while a specific screen would handle its own, e.g. `412` concurrency issues. This allowed the screens to display custom dialogs in certain circumstances without triggering the generic snackbar notification.

On some complex screens, multiple components may need to register to handle different errors. Because the `HttpInterceptor` does not know which components called which endpoints, it has no way of allowing specific components to register for specific errors. This resulted in some components needing to handle common errors and creating unmaintainable code.

The existing snackbar notification is not user-friendly.

## Decision

Errors will not be swallowed by an `HttpInterceptor` at all. Every component has the opportunity to respond to any error it desires. The `ErrorHandler` will remain as a catch-all which dispays a friendlier snackbar notification, and logs the actual error. Convenience operators for observables will be used to handle errors in an observable pipe chain.

### Operators

- `catchServerError` will create a side-effect and not propagate the error through the pipe.
- `mapServerError` will transform the error into a new valid observable and recover the pipe.
- `suppressServerError` will stop propagating errors through the pipe without any side-effects.
- `tapServerError` will create a side-effect and propagate the error through the pipe.

## Consequences

- `HttpInterceptor` is deleted.
- Default snackbar notification is more user-friendly.
- Inconsistency in snackbar notifications until every app uses this approach to error handling.
- Components can catch their own errors without having them swallowed.
- Screens no longer register to handle specific errors.
- Errors can be recovered with `mapServerError`.
- Errors can be handled in services or directly in components.
- The standard [`catchError` operator](https://rxjs.dev/api/operators/catchError) can be used.
- Reliance on DataDog for errors from users.
    - Adblockers easily prevent the logs from going to DataDog.

### Sample operator usage

Note, each server error operator can take either a single http status, or an array of statuses. They can be combined, and will run in order when an error is thrown.

#### *`catchServerError(status: number | number[], handle: (error: HttpErrorResponse) => void)`*

`catchServerError` will create a side-effect and not propagate the error through the pipe. Use it to respond to a specific http error status, and prevent the subsequent operators or subscription from being called.

```typescript
this.backendService.query('greetings')
    .pipe(
        catchServerError(403, () => this.sharedServices.showUnauthorizedError('view greetings')),
        catchServerError(400, error => this.sharedServices.showError(error.message)),
        map(greetings => new TransformedModel(greetings))) // never called if an error is caught
    .subscribe(model => this.doSomething()); // never called if an error is caught
```

#### *`mapServerError(status: number | number[], handle: (error: HttpErrorResponse) => Observable<any>)`*

`mapServerError` will transform the error into a new valid observable and recover the pipe. Use it to respond to a specific http error status, and pass a default observable to subsequent operators or subscriptions.

```typescript
this.backendService.query('greetings')
    .pipe(
        mapServerError(404, () => of([])), // nothing found? default to an empty array
        map(greetings => new TransformedModel(greetings))) // called with an empty array
    .subscribe(model => this.doSomething());
```

#### *`suppressServerError(status: number | number[])`*

`suppressServerError` will stop propagating errors through the pipe without any side-effects. Use it to ignore a specific http error status, e.g. if you are debouncing a duplicate-name check and expect 404 for non-duplicate names, or for known not-implemented API features.

```typescript
this.backendService.query('greetings/check-name')
    .pipe(
        suppressServerError([404, 501]))
    .subscribe(model => this.duplicateName = true); // never called on 404 or 501
```

#### *`tapServerError(status: number | number[], handle: (error: HttpErrorResponse) => void)`*

`tapServerError` will create a side-effect and propagate the error through the pipe. Use it to do something with an error, but still let other error handlers take action. Can be used to separate concerns on the same error.

```typescript
this.backendService.query('greetings')
    .pipe(
        tapServerError(412, error => this.mergeChangesMode = true), // do something to let the app know it needs to try and merge conflicts
        catchServerError(412, () => this.sharedServices.showLatestVersionError(`You don't have the latest version.`))) // still called
    .subscribe(model => this.doSomething()); // never called on concurrency exceptions
```
