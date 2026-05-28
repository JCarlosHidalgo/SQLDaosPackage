using SQLDaosPackage.Entities;
using SQLDaosPackage.Entities.Attributes;

namespace Test.MySQL.Entities.TwoForeign;

public class TenantDomain : ITwoForeignEntity
{
    [FirstForeignId]
    public Guid UserId { get; set; }

    [SecondForeignId]
    public Guid TenantId { get; set; }
}
