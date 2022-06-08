# ADR-0013: Add nightly builds

Date: 2020-07-13

## Status

Accepted

## Context

We've had issues where we leave a microapp in a fully working state, move on to other work, and when we return,we find that the build has broken. This can be due any number of non-code dependencies, including infrastructure and parameter store configuration changes. Because these applications are currenlty only built when the code changes, it can be difficult to identify what non-code change broke the build. We may have to search through months of changes to find the one specific change that broke the build. 

### Options

With more frequent builds, we can shorten the timeframe to search, thereby reducing the number of suspect changes.

CircleCI supports scheduled builds. Nightly seems reasonable.

## Decision

Going forward, each application will trigger a build of the `master` branch at 8 AM UTC (2 or 3 AM central, depending on the time of year). 

Here are the workflows for nightly builds and code-change triggered builds:

```
workflows:
  version: 2
  nightly:
    triggers:
      - schedule:
          cron: "0 8 * * *"
          filters:
            branches:
              only:
                - master
    jobs:
      - build:
          context: default-context
          run-tests-csharp: true
          run-tests-spa-unit: true
          run-tests-spa-integration: false
          run-push-images: false
  build_workflow:
    jobs:
      - build:
          context: default-context
          docker-image-repo: quay.io
          run-tests-csharp: true
          run-tests-spa-unit: true
          run-tests-spa-integration: false
          run-push-images: true
```

## Consequences

* Because we don't have enough CircleCI workers, most of these builds will queue up.
* Because new images would clutter our deployment pipeline, we won't publish images from nightly builds. This is controlled with the `run-push-images` parameters in the `.circleci/config.yml` file.
* Only the master branch will be built each night.
* Because both workflows trigger the `build` action, GitHub will label them `ci/circleci: build` and `ci/circleci: build-2`. So far, `ci/circleci: build-2` appears to be the code-change triggered build. 
