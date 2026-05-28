# SQLDaosPackage

[![NuGet](https://img.shields.io/nuget/v/JuanCarlosHS.SQLDaosPackage.svg)](https://www.nuget.org/packages/JuanCarlosHS.SQLDaosPackage)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

C# / .NET 9 NuGet library that implements the DAO design pattern over MySQL.

## NuGet package

Available on NuGet as `JuanCarlosHS.SQLDaosPackage`:

https://www.nuget.org/packages/JuanCarlosHS.SQLDaosPackage

```bash
dotnet add package JuanCarlosHS.SQLDaosPackage
```

## Public API at a glance

- **DAO interfaces** — `ISingleDao<T>`, `ITwoForeignDao<T>`, `IThreeForeignDao<T>`, all extending the root `IDao<T>`.
- **MySQL implementations** — abstract bases consumers subclass: `MySQLSingleDao<T>`, `MySQLTwoForeignDao<T>`, `MySQLThreeForeignDao<T>`.
- **Entity markers** — `IEntity`, `ITwoForeignEntity`, `IThreeForeignEntity` constrain each DAO to a compatible entity shape.
- **Column attributes** — role markers (`[Identificator]`, `[Identifier]`, `[FirstForeignId]`, `[SecondForeignId]`, `[ThirdForeignId]`) plus column-type markers (`[Text]`, `[Integer]`, `[SmallInteger]`, `[BigInteger]`, `[Timestamp]`, `[PreciseTimestamp]`, `[Date]`, `[Time]`, `[Flag]`) and `[NotPersisted]`. Full table in the [Code README](SQLDaosPackage.Code/README.md#entity-attributes).
- **Cross-cutting** — `MySQLRetryPolicy` (exponential backoff over curated transient errors) and the `IDataInjector` / `DataInjector` pair for `LOAD DATA` bulk loads.

## Repository layout

- `SQLDaosPackage.Code/` — the shipped NuGet package. Its [README](SQLDaosPackage.Code/README.md) is the detailed API reference and usage guide.
- `SQLDaosPackage.Test/` — NUnit integration tests that run against a real MySQL instance.

## Running tests + coverage + docs

A single Docker Compose file spins up a MySQL container with the test schema, runs `dotnet test` with coverage, generates the HTML coverage report with ReportGenerator, builds the Doxygen API docs, and serves everything through Apache.

```bash
docker compose up --build
```

When the build finishes, open:

- http://localhost:5000/ — landing page with links to both views.
- http://localhost:5000/test/ — test coverage report.
- http://localhost:5000/docs/ — Doxygen API documentation.

## License

MIT — see [LICENSE](LICENSE).
