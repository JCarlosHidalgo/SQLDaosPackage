using System.Globalization;

using SQLDaosPackage.Entities;
using SQLDaosPackage.Entities.Attributes;

namespace SQLDaosPackage.Daos.MySQL;

/// <summary>
/// Formats CLR values as MySQL literal expressions for inline SQL generation.
/// </summary>
 /*!
    This helper feeds the auto-generated INSERT/UPDATE produced by
    \c MySQLBaseDao.CreateCommandIntoStringBuilder and
    \c UpdateCommandIntoStringBuilder. Values are emitted as escaped literals,
    not as parameter placeholders — the same shape the legacy \c Read/Delete by
    Guid in this package already uses. String values are protected from SQL
    injection by doubling single quotes; numeric and temporal values are
    converted with \c InvariantCulture so they never depend on the runtime's
    locale.

    A future migration to \c MySqlCommand.Parameters would replace this helper
    altogether and is tracked as a follow-up.
  */
internal static class MySqlLiteralFormatter
{
    public static string FormatValue(object? value, EntityColumnDescriptor descriptor)
    {
        if (value is null)
        {
            return "NULL";
        }

        return value switch
        {
            string text => "'" + text.Replace("'", "''") + "'",
            Guid guid => "'" + guid.ToString() + "'",
            bool flag => flag ? "1" : "0",
            DateTime dateTime when descriptor.TypeAttribute is PreciseTimestampAttribute =>
                "'" + dateTime.ToString("yyyy-MM-dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture) + "'",
            DateTime dateTime =>
                "'" + dateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + "'",
            DateOnly date => "'" + date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "'",
            TimeOnly time => "'" + time.ToString("HH:mm:ss", CultureInfo.InvariantCulture) + "'",
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString() ?? "NULL"
        };
    }
}
