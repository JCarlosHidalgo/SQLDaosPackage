using System.Data;

using MySql.Data.MySqlClient;

namespace SQLDaosPackage.DAOS.MySQL;

/// <summary>
/// Static helper that retries MySQL operations on transient failures
/// (connection drops, server-gone-away, deadlocks). Reopens the underlying
/// connection between attempts if it is no longer Open.
/// </summary>
public static class MySQLRetryPolicy
{
    private const int MaxAttempts = 3;
    private const int InitialDelayMilliseconds = 100;

    private static readonly HashSet<int> TransientErrorNumbers = new()
    {
        1042,
        1043,
        1205,
        1213,
        2002,
        2003,
        2006,
        2013,
        2055
    };

    public static async Task<TResult> ExecuteAsync<TResult>(
        MySqlConnection? connection,
        Func<Task<TResult>> operation,
        MySqlTransaction? transaction = null)
    {
        int delayMilliseconds = InitialDelayMilliseconds;
        MySqlException? lastException = null;

        for (int attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            try
            {
                return await operation();
            }
            catch (MySqlException ex) when (transaction is null && IsTransient(ex))
            {
                lastException = ex;
                if (attempt == MaxAttempts) break;
                await Task.Delay(delayMilliseconds);
                delayMilliseconds *= 2;
                await EnsureOpenAsync(connection);
            }
        }

        throw lastException!;
    }

    public static Task ExecuteAsync(
        MySqlConnection? connection,
        Func<Task> operation,
        MySqlTransaction? transaction = null)
    {
        return ExecuteAsync<object?>(
            connection,
            async () => { await operation(); return null; },
            transaction);
    }

    private static bool IsTransient(MySqlException exception) =>
        TransientErrorNumbers.Contains(exception.Number);

    private static async Task EnsureOpenAsync(MySqlConnection? connection)
    {
        if (connection is null) return;
        if (connection.State == ConnectionState.Open) return;

        try
        {
            if (connection.State == ConnectionState.Broken)
            {
                await connection.CloseAsync();
            }
            await connection.OpenAsync();
        }
        catch (MySqlException)
        {
        }
    }
}
