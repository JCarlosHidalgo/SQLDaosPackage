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

## Running the test suite + coverage report

The canonical way to run the tests is via Docker Compose: it spins up a MySQL container with the test schema already applied, runs `dotnet test` with coverage, generates the HTML report with ReportGenerator, and serves it through Apache.

```bash
docker compose up --build
```

When the build finishes, open http://localhost:8000 to browse the coverage report.

## Building the Doxygen API documentation

A second compose file builds the Doxygen documentation inside a container and serves the generated HTML through Apache.

```bash
docker compose -f compose.docs.yaml up --build
```

Open http://localhost:8001 to browse the API documentation.

## License

MIT — see [LICENSE](LICENSE).
