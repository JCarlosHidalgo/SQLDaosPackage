namespace Test.MySQL.Entities.Single;

public class User
{
    public required Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string Role { get; set; }
}
