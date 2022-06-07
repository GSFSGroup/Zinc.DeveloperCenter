# ADR-0015: Selection of a UI Automation Tool

Date: 2020-12-18

## Status

Pending

## Context

UI automation testing is a technique where testing is performed via browser automation. Instead of a QA tester or developer clicking through the site to ensure it works, those actions would be automated via a tool.

As opposed to more Javascript-based testing, this provides more coverage as you're simulating the actual browser. There are things that can be tested that are difficult or impossible to do via a Javascript testing framework.

There are drawbacks however. UI tests can be difficult to maintain. As you're spinning up a browser for the tests, there are more issues that can occur during test runs. They also tend to be magnitude of orders slower than others tests.

Given their drawbacks however, they're an essential piece in comprehensive test suite.

This ADR serves to highlight some of the research that was done for various tools.

The goal is to find a tool suitable for our use that can be used in our deployment pipeline to ensure the quality of production deployments.

## Options

### Browserstack/SauceLabs

I'm creating a combined summary of both the advantages and disadvantages for SauceLabs and Browserstack. For all intents and purposes, they essentially provide the same service and feature set.

SauceLabs and Browserstack are cloud-based, continuous testing tools, specifically focused on UI automation. They provide a wide range of features such as, viewing test runs, live browser testing, visual testing, etc.

In the context of what we're trying to accomplish in RedLine, we'd essentially write our tests in a UI framework of choice (Nightwatch was chosen for these particular samples for its ease-of-use). We'd then hook into either SauceLabs or Browserstack via configuration changes, and our test runs would be recorded in the cloud. This also gives us the added benefit of being able to run tests locally, since the tests are created programatically.

### Pros

- Both tools support a wide variety of UI testing frameworks.
- Both tools have extensive supporting features, such as being able to live test on certain machines/devices/browsers, visual testing, mobile testing, etc.
- Both tools have good documentation and were very easy to get started with.
- If we did choose one, and decided we wanted to switch, the change is a very simple one. UI tests were originally written with Nightwatch, for SauceLabs. With a few configuration changes, those exact same tests were running in Browserstack in around 30 minutes.jet
- Both tools are widely used in the community and have support options available. Getting help when we need it should not be a problem.
- As we're essentially writing the UI tests programatically, our UI tests are very portable. We can run them wherever we need to since they are NodeJS tests. If in the future we decided we wanted neither SauceLabs or Browserstack, we could faciliate that.

### Cons

- There's no support as far as drawing up "test cases/plans". We'd have to own what we want to do as far as testing in the code.
- Selenium tests can be brittle and have a high maintenance cost. This is also "code", so it's subject to all the other things we typically want to see in our code bases, i.e. linting, build checks/gates, etc.
- The framework chosen for the sample, NightwatchJS, works great, but doesn't have some of the things we might want. Namely Typescript support. There were also some inconsistencies I noticed with Safari that I weren't able to tell if (a) they were from the framework or (b) from the machine they were run on.


### Katalon

Katalon is an end-to-end test automation platform. It has a variety of tools and services to generate UI automation tests.

There an IDE studio for generating test cases in an easy-to-use format. There's recording functionality for recording tests and UI elements to be used in the tests. It provides a codeless experience that makes the tool easy for anyone to use, but there's functionality where customization can be done using code.

There are also a wide variety of integration points with tools we commonly use, i.e. JIRA, Slack, etc.

### Pros
- Tests are quick to write and easy to replace.
- Tests are easy to modify.
- Well structured.
- Generates xpath alternatives.
- The selenium extensions used are open sourced.
- Generated code is clean and readable.

### Cons
- Uses groovy as a DSL language which may have a learning curve for some developers and for some custom tests.
- If we run the tests in our own container java is required.
- Advanced features may require a studio enterprise license.


### TestSigma

TestSigma is a cloud-based automation tool for UI automated tests.

It uses "Natural Language" tooling to enable the creation of tests in a simple and intuitive manner. It also supports test cases, where we could create different testing groups across our applications. This would allow us to potentially re-use different tests across different areas of the application.

It also allows generating test data to increase coverage across different test cases and conditions.
​
### Pros:
​
- Use of Natural Language makes it easy to write the test steps.
   ```
    1:Navigate to ``link``
    2:Wait for 1 seconds
    3:Verify that the element `ui identifier`  is displayed
    4:Click on ui `identifier`
   ```
- Capturing the UI identifier is easy.
- Can easily record the steps using chrome extension.
- Provided good test hierarchy and organization.
   - Create reusable Test groups
   - A requirement is attached to the test case.
   - Test cases can be grouped in test suites
   - Test Plan can be setup for test suites
- Provides API testing
- Can hook in local machine for testing.
- Can write custom functions in Selenium.
- Write data driven tests.
​
​
### Cons:
- Does not provide the way to automate key press event with a set of custom pair of keys (example ALT+a). But we can write the custom function and can add as the test step.
```
  eg:
   public TestStepResult createCustomSteps() {
​
         //your custom code starts here
​
         Actions actions = new Actions(driver);
         actions.keyDown(Keys.ALT);
         actions.sendKeys("a");
         actions.perform();
​
         //your custom code ends here
​
        TestStepResult result= new TestStepResult();
        result.setStatus(ResultConstants.SUCCESS);
        result.setMessage("Successfully Executed");
        return result;
    }
```

- Cannot export the test cases. We can only export the UI identifiers.

### Selenium IDE

Selenium is the go-to tool for UI automation in the IT industry. It's battle-tested, stable, has a wide variety of support for different languages, browsers, operating systems, etc.

You would be hard pressed to find anyone in the industry who hasn't either heard of or used the tool.

There can be issues writing and running the Selenium tests. They can be brittle at runtime and are often difficult and time-consuming to maintain. This is something inherent with most UI automation frameworks though.

Selenium falls into the "...no one got fired for buying IBM..." class of software and it would be hard to go wrong choosing the framework.

### Pros

- Super easy to get started with. We probably had tests going in ~30 minutes, from downloading the software to actually writing the tests.
- It's Selenium...it probably has the biggest community, selection of languages, integrations, etc. It's the "...gorilla in the jungle, holding the banana". It has everything.
- We have ultimate flexibility in moving across different tools and services when it comes to UI automation testing. Virtually everything supports Selenium.

### Cons

- I don't know very many people who've actually used Selenium and enjoyed it. It's a tool of necessity.
- Tests can be more brittle than other frameworks, based on experience.

### Playwright

Playwright is a Node.js library to automate UI testing across Chromium, Firefox, and Webkit based browsers.

It was recently created by Microsoft (and the team who made Puppeteer) and provides a single API for testing a wide variety of scenarios against a wide variety of browsers. It can test across multiple pages, auto-wait for elements, intercept and stub network activity and mock requests, supports selectors for web components, among a host of other features.

It has support for a wide variety of languages (Javascript, Typescript, Python, C#, Go at the time of this writing), comes with its own Docker container, and integrations for virtually of CI providers.

### Pros

- Super easy to get started with. Had tests converted from another Node.js framework in ~30 minutes. Had it running in Docker and CircleCI another 30 minutes ~ 1 hour after that.
- Docker support out-of-the-box. Was able to run the suite against Chrome, Firefox, and Safari in a container.
- Given the Docker support, will be easier for Windows developers to test locally against Safari, should the need arise.
- Very good APIs and good documentation.
- Supports Typescript out-of-the-box. This was difficult to find with other Node.js frameworks.
- Supports multiple languages.
- If we run it in Docker and on CirlceCI like we do our other tests...it's free...?

### Cons

- Relatively new, doesn't have the support with other providers (tools, services, etc.) that the other frameworks have.
- It's support for running tests in parallel is very limited. There's a GitHub issue saying they're looking into it with a workaround.
- It was made by Microsoft (no offense), i.e. https://killedbymicrosoft.info/. That list doesn't include the software either.

## Decision

We will move forward with Selenium IDE. It's the staple of UI automation tools.

It has the biggest community, documentation, and ecosystem. It's also virtually supported by all UI services and was rather easy to get started with via the IDE.

For the backing service, we will be using SauceLabs. It has the same feature set, community support, etc. as Browserstack while having the added bonus of being less expensive.
