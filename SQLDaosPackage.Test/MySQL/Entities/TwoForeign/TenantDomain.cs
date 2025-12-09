namespace Test.MySQL.Entities.TwoForeign;

public class TenantDomain
{
    public Guid UserId      { get;set; }
    public Guid TenantId    { get;set; }
}