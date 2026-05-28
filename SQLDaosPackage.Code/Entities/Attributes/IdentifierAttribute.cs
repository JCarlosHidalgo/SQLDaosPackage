namespace SQLDaosPackage.Entities.Attributes;

/// <summary>
/// Declares the marked Guid property as an identifier column whose role is plain
/// data (neither the entity primary key nor a foreign key tracked by the package).
/// </summary>
 /*!
    Use this for Guid columns that are persisted but do not participate in the
    WHERE-clause of the Dao base methods (\c Read, \c Delete). Examples include
    aggregate references such as \c TenantId on a non-tenant-scoped DAO, or the
    \c AggregateId column of an outbox row. The MySQL implementation maps this
    to \c VARCHAR(36); a future PostgreSQL implementation maps it to \c UUID.
    For columns that play a role in the WHERE-clause prefer \c [Identificator],
    \c [FirstForeignId], \c [SecondForeignId] or \c [ThirdForeignId] — those
    already imply the same Guid mapping.
  */
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class IdentifierAttribute : Attribute
{
}
