namespace SQLDaosPackage.Entities.Attributes;

/// <summary>
/// Declares the marked string property as a bounded-length text column.
/// </summary>
 /*!
    \param length The maximum number of characters the column accepts. The
    MySQL implementation maps this to \c VARCHAR(length); a future PostgreSQL
    implementation may map it to \c VARCHAR(length) or \c TEXT depending on size.
  */
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class TextAttribute : Attribute
{
    public int Length { get; }

    public TextAttribute(int length)
    {
        Length = length;
    }
}
