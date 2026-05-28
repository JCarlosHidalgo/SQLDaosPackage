using System.Reflection;

namespace SQLDaosPackage.Entities;

/// <summary>
/// Describes how a single entity property maps to a column for the auto-generated
/// INSERT/UPDATE SQL.
/// </summary>
 /*!
    \c IsKey is true only for the property marked with \c [Identificator]; that
    property goes in the WHERE clause of the auto-generated UPDATE statement.
    Foreign-key markers (\c [FirstForeignId] etc.) are persisted columns but are
    not considered the row identifier for UPDATE purposes.

    \c TypeAttribute is the type attribute applied to the property (\c [Text],
    \c [Integer], \c [Timestamp], …) or \c null when the column carries only a
    role marker (\c [Identificator], \c [FirstForeignId], \c [SecondForeignId],
    \c [ThirdForeignId], \c [Identifier]) — those imply a Guid value and need no
    extra type attribute.
  */
internal sealed record EntityColumnDescriptor(
    string ColumnName,
    PropertyInfo Property,
    Attribute? TypeAttribute,
    bool IsKey,
    bool IsNullable);
