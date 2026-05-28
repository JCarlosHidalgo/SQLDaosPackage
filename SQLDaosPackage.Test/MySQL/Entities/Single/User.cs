using SQLDaosPackage.Entities;
using SQLDaosPackage.Entities.Attributes;

namespace Test.MySQL.Entities.Single;

public class User : IEntity
{
    [Identificator]
    public required Guid Id { get; set; }

    [Text(80)]
    public required string UserName { get; set; }

    [Text(50)]
    public required string Role { get; set; }
}
