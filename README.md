# SQLDaosPackage

C# / .NET 9 NuGet library that implements the DAO design pattern over MySQL.

## NuGet package

Available on NuGet as `JuanCarlosHS.SQLDaosPackage`:

https://www.nuget.org/packages/JuanCarlosHS.SQLDaosPackage

```bash
dotnet add package JuanCarlosHS.SQLDaosPackage
```

## Repository layout

- `SQLDaosPackage.Code/` — the shipped NuGet package. See its own [README](SQLDaosPackage.Code/README.md) for the architecture and build instructions.
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
