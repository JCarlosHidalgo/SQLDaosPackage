using System.Collections.Concurrent;
using System.Reflection;

using SQLDaosPackage.Entities.Attributes;

namespace SQLDaosPackage.Entities;

/// <summary>
/// Resolves the ordered list of persisted columns for an entity type.
/// </summary>
 /*!
    The resolver walks the public instance properties of \c entityType and
    classifies each one:

    - Properties marked with \c [NotPersisted] are skipped.
    - Properties marked with \c [Identificator] become the key column.
    - Properties marked with \c [FirstForeignId], \c [SecondForeignId],
      \c [ThirdForeignId] or \c [Identifier] become Guid columns.
    - Properties marked with a type attribute (\c [Text], \c [Integer], …) become
      typed columns.
    - Properties without any of the above raise an exception so missing mappings
      are detected the first time the Dao is instantiated.

    Results are cached per type because reflection is expensive and the mapping
    is immutable for a given CLR type.
  */
internal static class EntityColumnsResolver
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyList<EntityColumnDescriptor>> _cache = new();

    public static IReadOnlyList<EntityColumnDescriptor> ResolvePersistedColumns(Type entityType)
    {
        return _cache.GetOrAdd(entityType, BuildDescriptors);
    }

    private static IReadOnlyList<EntityColumnDescriptor> BuildDescriptors(Type entityType)
    {
        PropertyInfo[] properties = entityType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .OrderBy(property => property.MetadataToken)
            .ToArray();

        List<EntityColumnDescriptor> descriptors = new List<EntityColumnDescriptor>(properties.Length);
        foreach (PropertyInfo property in properties)
        {
            if (property.GetCustomAttribute<NotPersistedAttribute>() is not null)
            {
                continue;
            }

            bool isKey = property.GetCustomAttribute<IdentificatorAttribute>() is not null;
            bool isRoleMarked = isKey
                || property.GetCustomAttribute<FirstForeignIdAttribute>() is not null
                || property.GetCustomAttribute<SecondForeignIdAttribute>() is not null
                || property.GetCustomAttribute<ThirdForeignIdAttribute>() is not null
                || property.GetCustomAttribute<IdentifierAttribute>() is not null;

            Attribute? typeAttribute = ResolveTypeAttribute(property);

            if (!isRoleMarked && typeAttribute is null)
            {
                throw new InvalidOperationException(
                    $"Property '{entityType.FullName}.{property.Name}' must carry either a type attribute ([Text], [Integer], …), a role marker ([Identificator], [FirstForeignId], [SecondForeignId], [ThirdForeignId], [Identifier]) or [NotPersisted].");
            }

            bool isNullable = IsNullable(property);
            descriptors.Add(new EntityColumnDescriptor(property.Name, property, typeAttribute, isKey, isNullable));
        }

        return descriptors;
    }

    private static Attribute? ResolveTypeAttribute(PropertyInfo property)
    {
        Attribute? attribute = property.GetCustomAttribute<TextAttribute>();
        attribute ??= property.GetCustomAttribute<IntegerAttribute>();
        attribute ??= property.GetCustomAttribute<SmallIntegerAttribute>();
        attribute ??= property.GetCustomAttribute<BigIntegerAttribute>();
        attribute ??= property.GetCustomAttribute<TimestampAttribute>();
        attribute ??= property.GetCustomAttribute<PreciseTimestampAttribute>();
        attribute ??= property.GetCustomAttribute<DateAttribute>();
        attribute ??= property.GetCustomAttribute<TimeAttribute>();
        attribute ??= property.GetCustomAttribute<FlagAttribute>();
        return attribute;
    }

    private static bool IsNullable(PropertyInfo property)
    {
        Type type = property.PropertyType;
        if (Nullable.GetUnderlyingType(type) is not null)
        {
            return true;
        }
        if (!type.IsValueType)
        {
            NullabilityInfoContext context = new NullabilityInfoContext();
            return context.Create(property).ReadState == NullabilityState.Nullable;
        }
        return false;
    }
}
