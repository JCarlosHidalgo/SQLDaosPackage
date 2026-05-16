using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;

using MySql.Data.MySqlClient;

using SQLDaosPackage.Daos.MySQL;

using Test.MySQL.Utils;

namespace Test.MySQL.Daos;

[TestFixture]
public class RetryPolicyTest
{
    private static readonly ConstructorInfo MySqlExceptionCtor =
        typeof(MySqlException).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: new[] { typeof(string), typeof(int) },
            modifiers: null)!;

    private static MySqlException TransientException(int number) =>
        (MySqlException)MySqlExceptionCtor.Invoke(new object[] { "transient", number });

    private static MySqlException NonTransientException(int number) =>
        (MySqlException)MySqlExceptionCtor.Invoke(new object[] { "non-transient", number });

    [Test]
    public async Task Returns_value_without_retry_on_first_success()
    {
        int attempts = 0;
        int result = await MySQLRetryPolicy.ExecuteAsync<int>(connection: null, () =>
        {
            attempts++;
            return Task.FromResult(42);
        });

        Assert.That(result, Is.EqualTo(42));
        Assert.That(attempts, Is.EqualTo(1));
    }

    [Test]
    public async Task Retries_on_transient_error_and_succeeds()
    {
        int attempts = 0;
        int result = await MySQLRetryPolicy.ExecuteAsync<int>(connection: null, () =>
        {
            attempts++;
            if (attempts == 1) throw TransientException(2006);
            return Task.FromResult(7);
        });

        Assert.That(result, Is.EqualTo(7));
        Assert.That(attempts, Is.EqualTo(2));
    }

    [Test]
    public void Propagates_after_max_attempts_on_persistent_transient_error()
    {
        int attempts = 0;

        MySqlException? thrown = Assert.ThrowsAsync<MySqlException>(async () =>
        {
            await MySQLRetryPolicy.ExecuteAsync<int>(connection: null, () =>
            {
                attempts++;
                throw TransientException(2013);
            });
        });

        Assert.That(thrown, Is.Not.Null);
        Assert.That(thrown!.Number, Is.EqualTo(2013));
        Assert.That(attempts, Is.EqualTo(3));
    }

    [Test]
    public void Does_not_retry_on_non_transient_error()
    {
        int attempts = 0;

        MySqlException? thrown = Assert.ThrowsAsync<MySqlException>(async () =>
        {
            await MySQLRetryPolicy.ExecuteAsync<int>(connection: null, () =>
            {
                attempts++;
                throw NonTransientException(1062);
            });
        });

        Assert.That(thrown, Is.Not.Null);
        Assert.That(thrown!.Number, Is.EqualTo(1062));
        Assert.That(attempts, Is.EqualTo(1));
    }

    [Test]
    public void Does_not_retry_when_inside_transaction()
    {
        int attempts = 0;
        MySqlTransaction? fakeTransaction =
            (MySqlTransaction?)RuntimeHelpers.GetUninitializedObject(typeof(MySqlTransaction));

        MySqlException? thrown = Assert.ThrowsAsync<MySqlException>(async () =>
        {
            await MySQLRetryPolicy.ExecuteAsync<int>(
                connection: null,
                () =>
                {
                    attempts++;
                    throw TransientException(2006);
                },
                transaction: fakeTransaction);
        });

        Assert.That(thrown, Is.Not.Null);
        Assert.That(attempts, Is.EqualTo(1));
    }

    [Test]
    public async Task Void_overload_runs_operation_and_retries_on_transient_error()
    {
        int attempts = 0;
        await MySQLRetryPolicy.ExecuteAsync(connection: null, () =>
        {
            attempts++;
            if (attempts == 1) throw TransientException(1205);
            return Task.CompletedTask;
        });

        Assert.That(attempts, Is.EqualTo(2));
    }

    [Test]
    public async Task Retries_each_documented_transient_code()
    {
        int[] transientCodes = new[] { 1042, 1043, 1205, 1213, 2002, 2003, 2006, 2013, 2055 };

        foreach (int code in transientCodes)
        {
            int attempts = 0;
            int result = await MySQLRetryPolicy.ExecuteAsync<int>(connection: null, () =>
            {
                attempts++;
                if (attempts == 1) throw TransientException(code);
                return Task.FromResult(code);
            });

            Assert.That(result, Is.EqualTo(code), $"failed for code {code}");
            Assert.That(attempts, Is.EqualTo(2), $"attempts mismatch for code {code}");
        }
    }

    [Test]
    public async Task EnsureOpen_returns_when_connection_already_open()
    {
        MySqlConnection? conn = new MySQLConnectionUtils().GetConnection();
        Assert.That(conn, Is.Not.Null, "test requires a live DB connection");
        Assert.That(conn!.State, Is.EqualTo(ConnectionState.Open));

        int attempts = 0;
        int result = await MySQLRetryPolicy.ExecuteAsync<int>(conn, () =>
        {
            attempts++;
            if (attempts == 1) throw TransientException(2006);
            return Task.FromResult(11);
        });

        Assert.That(result, Is.EqualTo(11));
        Assert.That(attempts, Is.EqualTo(2));
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));

        conn.Dispose();
    }

    [Test]
    public async Task EnsureOpen_reopens_closed_connection_between_attempts()
    {
        MySqlConnection? conn = new MySQLConnectionUtils().GetConnection();
        Assert.That(conn, Is.Not.Null, "test requires a live DB connection");
        await conn!.CloseAsync();
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));

        int attempts = 0;
        int result = await MySQLRetryPolicy.ExecuteAsync<int>(conn, () =>
        {
            attempts++;
            if (attempts == 1) throw TransientException(2006);
            return Task.FromResult(99);
        });

        Assert.That(result, Is.EqualTo(99));
        Assert.That(attempts, Is.EqualTo(2));
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));

        conn.Dispose();
    }

    [Test]
    public async Task EnsureOpen_swallows_MySqlException_from_OpenAsync()
    {
        MySqlConnection conn = new MySqlConnection(
            "server=127.0.0.1;port=1;uid=invalid;pwd=invalid;database=none;Connection Timeout=1");

        int attempts = 0;
        int result = await MySQLRetryPolicy.ExecuteAsync<int>(conn, () =>
        {
            attempts++;
            if (attempts == 1) throw TransientException(2006);
            return Task.FromResult(7);
        });

        Assert.That(result, Is.EqualTo(7));
        Assert.That(attempts, Is.EqualTo(2));

        await conn.DisposeAsync();
    }

    [Test]
    public async Task EnsureOpen_handles_broken_connection_state()
    {
        MySqlConnection? conn = new MySQLConnectionUtils().GetConnection();
        Assert.That(conn, Is.Not.Null, "test requires a live DB connection");

        FieldInfo? stateField =
            typeof(MySqlConnection).GetField("connectionState",
                BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(stateField, Is.Not.Null, "connectionState field not found on MySqlConnection");
        stateField!.SetValue(conn, ConnectionState.Broken);
        Assert.That(conn!.State, Is.EqualTo(ConnectionState.Broken));

        int attempts = 0;
        int result = await MySQLRetryPolicy.ExecuteAsync<int>(conn, () =>
        {
            attempts++;
            if (attempts == 1) throw TransientException(2006);
            return Task.FromResult(123);
        });

        Assert.That(result, Is.EqualTo(123));
        Assert.That(attempts, Is.EqualTo(2));
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));

        conn.Dispose();
    }

    [Test]
    public void EnsureOpen_sync_is_noop_on_null_connection()
    {
        Assert.DoesNotThrow(() => MySQLRetryPolicy.EnsureOpen(null));
    }

    [Test]
    public void EnsureOpen_sync_opens_closed_connection()
    {
        MySqlConnection? conn = new MySQLConnectionUtils().GetConnection();
        Assert.That(conn, Is.Not.Null, "test requires a live DB connection");
        conn!.Close();
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));

        MySQLRetryPolicy.EnsureOpen(conn);

        Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));
        conn.Dispose();
    }

    [Test]
    public void EnsureOpen_sync_is_noop_when_already_open()
    {
        MySqlConnection? conn = new MySQLConnectionUtils().GetConnection();
        Assert.That(conn, Is.Not.Null, "test requires a live DB connection");
        Assert.That(conn!.State, Is.EqualTo(ConnectionState.Open));

        MySQLRetryPolicy.EnsureOpen(conn);

        Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));
        conn.Dispose();
    }

    [Test]
    public void EnsureOpen_sync_swallows_MySqlException_from_Open()
    {
        MySqlConnection conn = new MySqlConnection(
            "server=127.0.0.1;port=1;uid=invalid;pwd=invalid;database=none;Connection Timeout=1");

        Assert.DoesNotThrow(() => MySQLRetryPolicy.EnsureOpen(conn));

        conn.Dispose();
    }

    [Test]
    public void EnsureOpen_sync_handles_broken_connection_state()
    {
        MySqlConnection? conn = new MySQLConnectionUtils().GetConnection();
        Assert.That(conn, Is.Not.Null, "test requires a live DB connection");

        FieldInfo? stateField =
            typeof(MySqlConnection).GetField("connectionState",
                BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(stateField, Is.Not.Null, "connectionState field not found on MySqlConnection");
        stateField!.SetValue(conn, ConnectionState.Broken);
        Assert.That(conn!.State, Is.EqualTo(ConnectionState.Broken));

        MySQLRetryPolicy.EnsureOpen(conn);

        Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));
        conn.Dispose();
    }
}
