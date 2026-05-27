# SQLDaosPackage
## _SQL databases DAO's package_

A C# / .NET 9 NuGet package that implements the DAO design pattern in the simplest way possible. Currently only the MySQL backend is implemented.

Available on NuGet:

https://www.nuget.org/packages/JuanCarlosHS.SQLDaosPackage

```bash
dotnet add package JuanCarlosHS.SQLDaosPackage
```

## Architecture

### DAO interface hierarchy

The library categorizes DAOs by how many foreign-key relationships their underlying table has:

- `IDao<T>` (root) — `Create` / `ReadAll` / `Update` plus async variants.
- `ISingleDao<T> : IDao<T>` — adds `Read(Guid id)` / `Delete(Guid id)`. Used for tables that have their own primary key (zero or one outgoing foreign key). The primary-key column must be named exactly `Id`.
- `ITwoForeignDao<T> : IDao<T>` — adds `Read(Guid id1, Guid id2)` / `Delete(Guid id1, Guid id2)`. Used for many-to-many join tables that have no own identifier, just two foreign keys.

### MySQL implementation layer

`MySQLBaseDao<T>` is the abstract base that implements `IDao<T>`. `MySQLSingleDao<T>` and `MySQLTwoForeignDao<T>` extend it and implement the corresponding sub-interface. **Consumers subclass `MySQLSingleDao<T>` or `MySQLTwoForeignDao<T>`** — they never touch `MySQLBaseDao` directly.

A consumer subclass must:

1. In its constructor, assign `_connection` (a `MySqlConnection`) and `_tableName`. For `MySQLTwoForeignDao<T>`, also assign `_firstForeignKey` and `_secondForeignKey`.
2. Override these abstract methods on the base:
   - `MapReaderToEntity()` — read one row from `_mySqlReader` into a `T`.
   - `MapReaderToEntitiesList()` — iterate `_mySqlReader` and produce `List<T>`; this is what `ReadAll()` returns. Optionally override `MapReaderToEntitiesListAsync()` to use `ReadAsync` per row.
   - `CreateCommandIntoStringBuilder(T)` / `UpdateCommandIntoStringBuilder(T)` — build the `INSERT` / `UPDATE` SQL text. The base then runs it via `GetCommandByText(_sb)`.

`MySQLBaseDao` also exposes `GetCommandStoredProcedure(name)` so subclasses can add stored-procedure methods on top of the base CRUD surface.

### Minimal usage example

A consumer DAO for a `User` table (`Id`, `UserName`, `Role`) looks like this:

```csharp
using System.Text;
using MySql.Data.MySqlClient;
using SQLDaosPackage.Daos.MySQL;

public class UserDao : MySQLSingleDao<User>
{
    public UserDao(MySqlConnection connection)
    {
        _connection = connection;
        _tableName = "User";
    }

    protected override User MapReaderToEntity() => new User
    {
        Id = _mySqlReader!.GetGuid("Id"),
        UserName = _mySqlReader.GetString("UserName"),
        Role = _mySqlReader.GetString("Role")
    };

    protected override List<User> MapReaderToEntitiesList()
    {
        _entitiesList = new List<User>();
        while (_mySqlReader!.Read())
        {
            _entitiesList.Add(MapReaderToEntity());
        }
        _mySqlReader.Close();
        return _entitiesList;
    }

    protected override StringBuilder CreateCommandIntoStringBuilder(User entity)
    {
        _sb = new StringBuilder();
        _sb.Append("INSERT INTO ").Append(_tableName).Append(" (Id,UserName,Role) VALUES ('")
            .Append(entity.Id).Append("','")
            .Append(entity.UserName).Append("','")
            .Append(entity.Role).Append("');");
        return _sb;
    }

    protected override StringBuilder UpdateCommandIntoStringBuilder(User entity)
    {
        _sb = new StringBuilder();
        _sb.Append("UPDATE ").Append(_tableName)
            .Append(" SET UserName = '").Append(entity.UserName).Append("', ")
            .Append("Role = '").Append(entity.Role).Append("' ")
            .Append("WHERE Id = '").Append(entity.Id).Append("';");
        return _sb;
    }
}

// Consumption — the connection does not need to be opened first;
// the DAO routes every call through MySQLRetryPolicy.EnsureOpen.
var dao = new UserDao(new MySqlConnection(connectionString));
await dao.CreateAsync(new User { Id = Guid.NewGuid(), UserName = "ada", Role = "admin" });
List<User> users = await dao.ReadAllAsync();
```

For a `MySQLTwoForeignDao<T>` subclass, also assign `_firstForeignKey` and `_secondForeignKey` in the constructor (the column names of the two FKs on the join table).

### Retry policy and connection lifecycle

Every MySQL operation is routed through `MySQLRetryPolicy`:

- `EnsureOpen` / `EnsureOpenAsync` — called at the top of every DAO method so consumers can register the connection without eager-opening it. A `Broken` connection is closed first, then reopened.
- `ExecuteAsync` — wraps async operations with exponential backoff (3 attempts, 100 ms initial delay, doubling). Retries only on the curated transient error numbers (lock-wait, deadlocks, network / server-gone-away). Operations passed a non-null `MySqlTransaction` are not retried, since the transaction would already be invalid.

### Injector layer

Independent of the DAO hierarchy. `IDataInjector` / `DataInjector` run an arbitrary MySQL command, intended for `LOAD DATA` bulk loads from `.csv`. Consumers subclass `DataInjector` and set `_injectionCommand` in their constructor. `InjectDataAsync` also goes through `MySQLRetryPolicy`.

## Building with dotnet

Requires the .NET 9 SDK.

```bash
# Restore dependencies
dotnet restore SQLDaosPackage.Code.csproj

# Build the library
dotnet build SQLDaosPackage.Code.csproj -c Release

# Produce the NuGet package (.nupkg lands in bin/Release/)
dotnet pack SQLDaosPackage.Code.csproj -c Release
```

Version, author, and package metadata are defined in `SQLDaosPackage.Code.csproj`.
