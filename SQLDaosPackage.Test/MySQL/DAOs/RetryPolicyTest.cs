using System.Reflection;
using System.Runtime.CompilerServices;

using MySql.Data.MySqlClient;

using SQLDaosPackage.DAOS.MySQL;

namespace Test.MySQL.DAOs;

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
}
