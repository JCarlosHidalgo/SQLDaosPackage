using SQLDaosPackage.Entities;
using SQLDaosPackage.Entities.Attributes;

namespace Test.MySQL.Entities.Single;

public class Tenant : IEntity
{
    [Identificator]
    public required Guid Id { get; set; }

    [Text(200)]
    public required string Name { get; set; }
}
