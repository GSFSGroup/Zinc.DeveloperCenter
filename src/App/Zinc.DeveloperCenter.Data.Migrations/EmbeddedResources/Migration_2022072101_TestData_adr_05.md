# ADR-0002: Select an Infinite Scroll Library

Date: 2021-04-11

## Status

Pending

## Context

Infinite scroll is a very popular way of paging through large amounts of data and is used on virtually every website, including ours.

While we have infinite scroll implemented on several pages in our application, most of these are grid based and comes out-of-the-box with our grid library, agGrid.

We needed an infinite scroll library that could be used in the absence of a grid, hence the need for this ADR.

## Options

### [ngx-infinite-scroll](https://github.com/orizens/ngx-infinite-scroll)

ngx-infinite-scroll is a popular Angular library for implementing infinite scroll. It has a simple, easy-to-use API for implementing infinite scroll with a variety of content.

The library is also being used by the likes of Apple, Google, Amazon, Microsoft, etc.

### [Material Angular CDK (Component Dev Kit)](https://material.angular.io/cdk/scrolling/overview)

Infinite scrolling is also an option built-in to the Angular CDK, which is bundled with the Angular Material library currently in use.

The scrolling functionality built-in to the Angular CDK is very robust, with a variety of scrolling methods. For example, it supports a [virtual scrolling feature](https://material.angular.io/cdk/scrolling/overview) which only renders content that fits on the screen. This can potentially increase performance for larger dataset but does come at the expense of a more complex API.

## Decision

Due to time constraints, sprint commitments, and an interest in [KISS](https://en.wikipedia.org/wiki/KISS_principle), we decided to go with the **ngx-infinite-scroll** library.

It was **super easy** to get started with and will serve our needs, i.e. infinitely scrolling arbitrary content, just fine.

In regards to the scrolling built-in to Angular CDK, it supports advanced functionality, i.e. the virtual scrolling feature mentioned above. To reiterate, a big part of this decision was time constraints and an adherence to the "done is the engine of more" philosophy.

With that being said, we, the team, would like to keep the scrolling in Angular CDK in our "back pocket". We may run into limitations with the current infinite scroll library, where the CDK scrolling might be a better fit. In the event we run into those limitations, or time is allotted to perform further research on CDK scrolling, expect another ADR to follow.

## Consequences

- We may run into limitations with ngx-infinite-scroll on sufficiently large datasets. That is, page performance may be negatively affected if there are thousands or tens of thousands elements being rendered. What "sufficiently large" is in the context of earnings, still remains to be seen. Also, this could be mitigated with UX changes, i.e. only return the last 200 records and have a tool tip instructing the user to go to another page to see more.
- It looks like **ngx-infinite-scroll** has [built-in analytics](https://github.com/orizens/ngx-infinite-scroll/issues/352#issuecomment-742009046), which is how the package maintainers know the aforementioned companies are using it. The analytics are (a) GDPR compliant and (b) can be shut off, which has been done.
