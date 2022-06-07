# ADR-0010: Integration Testing from the Web Layer
Date: 2020-05-11

## Status

Accepted

## Context
Integration testing is an integral part of an automated testing suite. Integration tests are tests that run with volatile dependencies, e.g. a database, filesystem, that can potentially interfere with other through those dependencies. This is in stark contrast to unit tests which more often than not can be run completely standalone.

We've put in a lot of effort over the past several months to build out the Redline platform. When we originally started the decision was made to integration test from the Application layer down. As most of our orchestration and/or process handling logic has been "pushed down" with the help of [Mediatr](https://github.com/jbogard/MediatR), our controllers are often very thin. Previously, it has been somewhat difficult to run reliable tests from the web layer directly. The architecture of previous versions of ASP.NET MVC made this very difficult.

Given the recent changes with ASP.NET Core, and tooling that has been developed as a result, it may be a good time to look at testing from a "higher" level.

## Proposal
Given better testing faculties in ASP.NET Core, **we propose that we move integration testing to the Web layer instead of the Application layer.**

Microsoft implemented a [Test Server](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1) that gets with ASP.NET Core. This can spin-up your web app completely in memory and run tests against a running web host, much like we do when manually testing. This is in stark contrast to previous versions, where a web server would have to be setup to achieve the same results.

Jeremy Miller, a stalwart of the .NET OSS community, has created a library on top of the ASP.NET Core testing functionality called [Alba](https://jasperfx.github.io/alba/). Alba improves on some of the functionality built-in to the ASP.NET Core tools to make it easier to write and maintain HTTP-driven tests.

**We would leverage Alba as well as we move our testing strategy to the Web layer.**

Our tests would look similar to the following:

```C#
[Fact]
public async Task ListGreetings()
{
    var bearerToken = IdentityMother.GenerateJWTToken(fixture.IntegrationTestSettings);
    var correlationId = Guid.NewGuid();
    var result = await fixture.TestHost.Scenario(_ =>
    {
        _.WithRequestHeader(HeaderNames.Authorization, $"Bearer {bearerToken}");
        _.WithRequestHeader(HeaderKeyConstants.RedlineCorrelationId, correlationId.ToString());

        _.Get.Url(ListGreetingsEndpoint);
        _.StatusCodeShouldBe(200);
    });
}
```

We can also use the same DTOs that we currently use for request/response when sending a request to an endpoint. From a development perspective, the cost in time of writing an integration test should not be negatively impacted. 

## Decision

Moving forward, we will use the TestServer approach when integration testing.

Overall, we feel the positives of following this approach outweigh the negatives (which are listed below).

## Consequences
Below are some of the advantages and disadvantages with the aforementioned approach.

### Advantages
- Setting up the Test Server has been easier than wiring up the dependencies from each project. We're using the Startup class from the Web project, so it's just creating a web host from that.
- A lot of the mocking currently used in our tests should be reduced dramatically. Making it easier to read and maintain tests as well as giving the team more confidence functionality works as intended.
- We'd be able to verify all of our IoC registrations work as intended and would get immediate feedback instead of need to manually run the application.
- We'd be able to reliably test HTTP-centric concerns that may potentially leave us exposed to issues, e.g. headers, content-types, etc..
- We can still use the tooling we have setup for testing without issue, i.e. Respawn, Shouldly. Alba also works with all major testing frameworks, including xUnit.
- We'd be able to verify URLs and HTTP methods are correct on our routes. Reducing the chances of bad URLs making it to Production.

### Disadvantages
- As with all types of integration testing, it can be harder to deduce what's causing problems in a test failure since there are more dependencies included in the test run. This difficulty may increase since we're running a full web host.
- It may or may not be more difficult to swap out dependencies in the Web Host when we need to assert against specific values of a certain dependency. More research will have to be done in this area.
- Additional testing will be needed for other layers should the need arise, e.g. if we decide to add a CLI.

