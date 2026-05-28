namespace SQLDaosPackage.Entities.Attributes;

/// <summary>
/// Excludes the marked property from the auto-generated INSERT/UPDATE SQL.
/// </summary>
 /*!
    Apply on entity properties that are not backed by a column: navigation
    collections, computed values resolved server-side, or transient projections.
    Marked properties are skipped by \c EntityColumnsResolver.
  */
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class NotPersistedAttribute : Attribute
{
}
