# ADR-0017: Accounting Rules for Rounding Money

Date: 2021-03-15

## Status

Accepted

## Context

There are several standard rounding rules available for rounding floating point numbers, especially money. We need to carefully choose the correct one to ensure consistent handling throughout our tech stack.

### IEEE 754

[IEEE 754](https://en.wikipedia.org/wiki/IEEE_754#Rounding_rules) sets the IEEE standard for floating point arithmetic. It defines 5 rounding rules.

* Round to nearest, ties to even. _This is the default._
* Round to nearest, ties away from zero.
* Round to toward zero.
* Round to negative infinity.
* Round to positive infinity.

| Original number | Nearest, to even | Nearest, away from zero | Toward zero | Toward -∞ | Toward +∞ |
|-----------------|------------------|-------------------------|-------------|-----------|-----------|
| $0.035          | $0.04            | $0.04                   | $0.03       | $0.03     | $0.04     |
| $0.028          | $0.03            | $0.03                   | $0.02       | $0.02     | $0.03     |
| $0.025          | $0.02            | $0.03                   | $0.02       | $0.02     | $0.03     |
| $0.021          | $0.02            | $0.02                   | $0.02       | $0.02     | $0.03     |
| -$0.021         | -$0.02           | -$0.02                  | -$0.02      | -$0.03    | -$0.02    |
| -$0.025         | -$0.02           | -$0.03                  | -$0.02      | -$0.03    | -$0.02    |
| -$0.028         | -$0.03           | -$0.03                  | -$0.02      | -$0.03    | -$0.02    |
| -$0.035         | -$0.04           | -$0.04                  | -$0.03      | -$0.04    | -$0.03    |

### C#

`Math.Round` has an overload that accepts [a `MidpointRounding` enum value](https://docs.microsoft.com/en-us/dotnet/api/system.midpointrounding). If not specified, the default is `MidpointRounding.ToEven`. This conforms to IEEE 754.

### Python

[Python's `round()` function](https://docs.python.org/3/library/functions.html#round) is an implementation of _Round to nearest, ties to even_. There is no built-in function to specify an alternative rounding method. These alternatives can be implemented using `floor()`, `ceil()`, `trunc()`, and `copysign()`.

### JavaScript

The rounding algorithm of [JavaScript's `Math.round()`](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Math/round) does *NOT* conform to any of the 5 rounding rules in IEEE 754, does not accept a decimals parameter, and only returns an integer.

`Number.toFixed()` uses a different algorithm than `Math.round()`, but still doesn't conform to any of the 5 rounding rules in IEEE 754. It only returns a string, not a Number.

As expected, JavaScript is the strangest by a [sheppey](https://en.wikipedia.org/wiki/List_of_humorous_units_of_measurement#Length).

If math must be done in JavaScript using Big.js start by running `npm i big.js` and then `npm i --D @types/big.js`
Then set these two line itmes must be done for setting rounding mode and forcing precision:

```javascript
(<any>Big).strict = true; // looks like the type declarations don't support the strict mode directly
Big.RM = 1; // rounding mode, round away from 0 at equidistant
```

### Microsoft SQL Server

[Microsoft SQL Server's `ROUND()`](https://docs.microsoft.com/en-us/sql/t-sql/functions/round-transact-sql?view=sql-server-ver15) function is equivalent to C#'s `AwayFromZero`. SQL Server also supports [`CEILING()`](https://docs.microsoft.com/en-us/sql/t-sql/functions/ceiling-transact-sql?view=sql-server-ver15) and [`FLOOR()`](https://docs.microsoft.com/en-us/sql/t-sql/functions/floor-transact-sql?view=sql-server-ver15) which are equivalent to +∞ and -∞ respectively, but they only round to whole numbers. There is no overload that accepts a decimals parameter like C#'s `Math.Round`.

### PostgreSQL

The rounding algorithm used by [PostgreSQL's `ROUND()`](https://www.postgresql.org/docs/8.1/functions-math.html) function is undocumented. Interally, it uses C's [rint()](https://doxygen.postgresql.org/float_8c.html#a3c7923cced10d99582e20866897cebbc) under the covers, which depends on [the current rounding mode](https://en.cppreference.com/w/c/numeric/math/rint) of the system.

## Decision

I asked Cherie Goosen in Accounting which of the 5 IEEE rounding rules we should use. Both she and Mike Hurley chose *Round to nearest, ties away from zero*.

## Consequences

* In Microsoft SQL Server, we can use the built-in `ROUND()` function.
* In C#, always call `Math.Round()` and specify `MidpointRounding.AwayFromZero`.
* In Python and JavaScript, we will have to find a library or implement our own function.
* In PostgreSQL, we will have to implement our own function.
