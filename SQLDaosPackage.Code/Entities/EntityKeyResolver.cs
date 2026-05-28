using System.Reflection;

namespace SQLDaosPackage.Entities;

/// <summary>
/// Resolves the name of an entity property marked with a key attribute.
/// </summary>
 /*!
    Used by \c MySQLSingleDao, \c MySQLTwoForeignDao and \c MySQLThreeForeignDao to
    discover the column names that back the primary key or the foreign keys of an
    entity, without forcing the entity to expose properties with hard-coded names.
  */
internal static class EntityKeyResolver
{
    //! Returns the name of the only property of \c entityType marked with \c TAttribute.
    /*!
       \param entityType The CLR type to scan.
       \return The property name (used verbatim as the column name in generated SQL).
       \throws InvalidOperationException When zero or more than one property carries
       the attribute.
    */
    public static string ResolvePropertyName<TAttribute>(Type entityType)
        where TAttribute : Attribute
    {
        PropertyInfo[] marked = entityType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.GetCustomAttribute<TAttribute>() is not null)
            .ToArray();

        if (marked.Length == 0)
        {
            throw new InvalidOperationException(
                $"Entity type '{entityType.FullName}' must have one property marked with [{typeof(TAttribute).Name}].");
        }
        if (marked.Length > 1)
        {
            throw new InvalidOperationException(
                $"Entity type '{entityType.FullName}' has more than one property marked with [{typeof(TAttribute).Name}].");
        }

        return marked[0].Name;
    }
}
