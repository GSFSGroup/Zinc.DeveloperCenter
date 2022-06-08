# ADR-0021: Storing Timestamps in Postgres

Date: 2021-08-20

## Status

Proposed

## Context

When storing & retrieving dates in/from Postgres, it's easy to fail to account for the aspects of .NET, Postgres, or Ngsql and improperly handle a date. This may only result in the date being displayed wrong. However, data loss is possible when dates are stored improperly. A solution may appear to work, but fail in other cases.

An ideal solution would have the following characteristics:
1. Dates are sent and retrieved from Postgres as-is without depending on Postgres to convert them.
2. Doesn't require extra work each time Postgres is read or updated. The developer should not have to diligently manipulate a date each time it's read or stored.
3. Dates can be converted to an arbitrary locale easily as needed. For example, it may be impractical to localize data downloads/exports in browser.
4. Different representations of the same moment (i.e. DateTime.UtcNow & DateTime.Now) should save to Postgres as the same value or error out to avoid corruption.

## Background
To fully understand this ADR and the options presented, some background on Postgres, .NET, and Ngsql is necessary.

### Postgres Date Types
Postgres date types don't store time zone (or offset) information. `timestamp` stores a timestamp without modifying it. `timestamptz` automatically converts dates to UTC before storing.

> For timestamp with time zone, the internally stored value is always in UTC (Universal Coordinated Time, traditionally known as Greenwich Mean Time, GMT). An input value that has an explicit time zone specified is converted to UTC using the appropriate offset for that time zone. If no time zone is stated in the input string, then it is assumed to be in the time zone indicated by the system's TimeZone parameter, and is converted to UTC using the offset for the timezone zone.
> https://www.postgresql.org/docs/10/datatype-datetime.html#DATATYPE-DATETIME-INPUT

> A common mistake is for users to think that the PostgreSQL `timestamp with timezone` type stores the timezone in the database. This is not the case: only the timestamp is stored. There is no single PostgreSQL type that stores both a date/time and a timezone, similar to DateTimeOffset 
> https://www.npgsql.org/doc/types/datetime.html

Internally both are 64-bit signed integers. `timestamp` is the number of microseconds from 2000-01-01 00:00:00.000000. `timestamptz` is the number of microseconds from 2000-01-01 00:00:00.000000 UTC.

### Postgres Time Zone
By default Postgres will use the time zone of it's host. The time zone can also be specified in the connection string. The time zone setting affects how `timestamptz` is returned.

> When a timestamp with time zone value is output, it is always converted from UTC to the current timezone zone, and displayed as local time in that zone.
> https://www.postgresql.org/docs/10/datatype-datetime.html#DATATYPE-DATETIME-INPUT

### .NET Date Types
Neither `DateTime` or `DateTimeOffset` map to Postgres date types well. They contain information that can't be represented in Postgres. `DateTime` can not represent time zone's other than local and UTC. And most problematic, different representations of the same moment in time are not equal.    
https://blog.nodatime.org/2011/08/what-wrong-with-datetime-anyway.html

Additionally, .NET and Postgres don't use the same time zone format, so mapping between them is impractical.

> The real problem, as I said before, is that PostgreSQL uses IANA time zone names (e.g. America/New_York), while .NET TimeZoneInfo uses Windows time zone names (e.g. Eastern Standard Time). This means that converting PostgreSQL's timezone names to something usable by Npgsql is non-trivial.
> https://github.com/npgsql/npgsql/issues/1469#issuecomment-283888285

### Npgsql
Npgsql complicates things further by using the Postgres wire (binary) protocol. The `timestamptz` rules mentioned above only apply to how Postgres handles a date received via the text protocol.

Npgsql tries to replicate the Postgres behavior. But Npgsql doesn't know anything about the database schema when a query is compiled. Consequently, it doesn't know the destination column type. It only knows the C# datatype and the Npgsql parameter type.

This creates two problems.

#### Problem 1
When writing a `DateTime` to Postgres all time zone information is lost. Since Npgsql doesn't know if the destination of the will be `timestamp` or `timestamptz`, so for `DateTime` is assumes `timestamp` and all time zone information is ignored and so the local time will be saved with adjusting to UTC. 

##### A Note About Dapper

You can coerce Npgsql to send a `timestamptz` and account for the time zone information. But that requires you to explicitly set the parameter type to `NpgsqlDbType.TimestampTz` rather than let Npgsql infer it for you. And if you are using Dapper, you have to ditch Dapper's automatic mapping and have explicitly define all of the query parameters.

```
DateTime: 8/10/2021 10:30:59 AM Kind: Local
 => timestamptz(UTC)              (DateTime)8/10/2021 5:30:59 AM (Kind = Local)
 => timestamp(UTC)                (DateTime)8/10/2021 10:30:59 AM (Kind = Unspecified) 
```

#### Problem 2
Npgsql will ALWAYS return a `System.DateTime` for `timestamp` and `timestamptz` by default.

```
DateTimeOffset: 8/10/2021 3:30:59 PM +00:00
 => timestamptz(UTC)              (DateTime)8/10/2021 10:30:59 AM (Kind = Local)
 => timestamp(UTC)                (DateTime)8/10/2021 3:30:59 PM (Kind = Unspecified)
```

And for `timestamptz` the returned values are localized to the C# application's time zone. This can cause a major problem. The value read out might not save back to the database cleanly due to problem #1. 

##### A Note About Dapper

The returned `DateTime` could be cast to a `DateTimeOffset` to avoid running into problem #1. But this presents a challenge when using Dapper to automatically map the query result. If the object uses a constructor, Dapper will fail to find a valid constructor, because it's looking for a `DateTime` when the constructor takes a `DateTimeOffset`. This can be worked around by providing an empty constructor and property setters. But this creates extra work for the developer.

https://www.npgsql.org/doc/types/datetime.html

## Solutions Considered

### Existing Approach
The existing approach is to use `timestamptz` columns and `System.DateTimeOffset` .NET types.

This approach can work, but the developer must be aware of several landmines:
1. If the developer uses `DateTime` by accident, data will be lost.
2. It depends on `timestamptz`. So other clients that use the text protocol or operate in a different time zone may get different results.
3. Read values are converted to local time by Npgsql. Always getting back a `DateTime` creates challenges and risks when using Npgsql and Dapper.
4. Arbitrary time zones can't easy be represented. `DateTimeOffset` has limited time zone support which isn't compatible with browsers.

### NodaTime
`NodaTime` is a alternative date and time API for .NET. It addresses the limitation mentioned of the built-in .NET date types. 

* NodaTime types (`Instant`, `ZoneDateTime`, `LocalDateTime`) can be used in place of `DateTime` and `DateTimeOffset`. 
* NodaTime supports IANA time zones that match browser localization.
* There is a first-class Npgsql extension for NodaTime.

Using NodaTime we can ensure that we always store/retrieve UTC without Postgres or Npgsql converting the values. Any `DateTime` or `DateTimeOffset` can be safely converted to an NodaTime type. And dates can to converted IANA time zones that match browser localization.

## Decision

### Decision 1: Postgres Server Time Zone
The Postgres server always operate in UTC. Not doing so can cause a bunch of additional issue, not fully captured in this document. And hard-coding to avoids all of this.

At the time of this writing, the docker-compose file does not set the TZ environment variable. So Postgres runs in local time, which will vary across environments. We should set Postgres to UTC at the server level or in the connection string.

### Decision 2: Use NodaTime
We should use `NodaTime` in place of `DateTime` and `DateTimeOffset`. When possible it's preferable to use `timestamp` Postgres columns, since that avoids Postgres converting timestamp.

Existing applications do not need to be migrated, `timestamptz` is safe to use as long as the Postgres connection is operating in UTC.

Finally, we should remove the `DateTime` and `DateTimeOffset` type handlers from Npgsql. This will ensure the developer doesn't accidentally use the wrong type.

## Example
1. Add NodaTime NuGet package to relevant application projects. Most likely Domain and Application projects will need it. Probably tests will as well.
    ```
    dotnet add package NodaTime
    ```
2. Add NodaTime & Npgsql.NodaTime NuGet packages in the RedLine.Data project.
    ```
    dotnet add package NodaTime
    dotnet add package Npgsql.NodaTime
    ```
3. Enable Npgsql.NodaTime and remove type handlers during setup.
    ```
    NpgsqlConnection.GlobalTypeMapper.UseNodaTime();
    ```
4. Replace Dapper type handlers.
    ```
    SqlMapper.RemoveTypeMap(typeof(DateTime));
    SqlMapper.RemoveTypeMap(typeof(DateTimeOffset));
    SqlMapper.AddTypeMap(typeof(Instant), DbType.DateTime);
    SqlMapper.AddTypeMap(typeof(Instant), DbType.DateTimeOffset);
    SqlMapper.AddTypeMap(typeof(Instant), DbType.DateTime2);
    ```
5. JSON serializer support
    
   Refer to this commit: https://github.com/GSFSGroup/Zinc.DeveloperCenter/commit/834882702f8d7777da7be64e4325b4a5d00f7e04

    * Add NuGet references:
        * `NodaTime` in `Directory.Build.props`
        * `NodaTime.Serialization.JsonNet` in `RedLine.Data`
    * Import changes from the template to these files:
        * App/Zinc.DeveloperCenter.Host.Web/Startup.cs
        * RedLine/RedLine.Data/Outbox/OutboxRecord.cs
        * RedLine/RedLine.Data/Serialization/NodaTimePatternMatchingConverter.cs
        * RedLine/RedLine.Data/Serialization/RedLineNewtonsoftSerializerSettings.cs
        * RedLine/RedLine.Data/ServiceCollectionExtensions.cs
