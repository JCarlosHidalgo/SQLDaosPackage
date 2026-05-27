using System.Data;
using System.Linq;
using System.Text;

using MySql.Data.MySqlClient;

using NUnit.Framework.Legacy;

using SQLDaosPackage.Daos.MySQL;

using Test.MySQL.Entities.Single;
using Test.MySQL.Utils;

namespace Test.MySQL.Daos;

[TestFixture]
public class BaseDaoTest
{
    public static MySqlConnection? _databaseConnection;

    [OneTimeSetUp]
    public void Init_connection()
    {
        MySQLConnectionUtils test = new MySQLConnectionUtils();
        _databaseConnection = test.GetConnection();
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
        _databaseConnection?.Dispose();
    }

    private void TruncateAllTables(MySqlConnection conn)
    {
        MySqlCommand com = new MySqlCommand("TruncateAllTables", conn);
        com.CommandType = CommandType.StoredProcedure;
        com.ExecuteNonQuery();
    }

    class UserBaseDao : MySQLSingleDao<User>
    {
        public UserBaseDao()
        {
            _connection = _databaseConnection!;
            _tableName = "User";
        }

        protected override User MapReaderToEntity()
        {
            return new User
            {
                Id = _mySqlReader!.GetGuid("Id"),
                UserName = _mySqlReader.GetString("UserName"),
                Role = _mySqlReader.GetString("Role")
            };
        }

        protected override List<User> MapReaderToEntitiesList()
        {
            _entitiesList = new List<User>();
            while (_mySqlReader!.Read())
            {
                _entity = MapReaderToEntity();
                _entitiesList.Add(_entity);
            }
            _mySqlReader.Close();
            return _entitiesList;
        }

        protected override StringBuilder CreateCommandIntoStringBuilder(User entity)
        {
            string idConverted = entity.Id.ToString();
            string userNameConverted = entity.UserName;
            string roleConverted = entity.Role;

            _sb = new StringBuilder();
            _sb.Append("INSERT INTO ").Append(_tableName).Append(" (Id,UserName,Role) ")
                .Append("VALUES ('").Append(idConverted).Append("','")
                                    .Append(userNameConverted).Append("','")
                                    .Append(roleConverted).Append("');");
            return _sb;
        }

        protected override StringBuilder UpdateCommandIntoStringBuilder(User entity)
        {
            string idConverted = entity.Id.ToString();
            string userNameConverted = entity.UserName;
            string roleConverted = entity.Role;

            _sb = new StringBuilder();
            _sb.Append("UPDATE ").Append(_tableName)
                .Append(" SET UserName = '").Append(userNameConverted).Append("', ")
                .Append(" Role = '").Append(roleConverted).Append("' ")
                .Append(" WHERE Id = '").Append(idConverted).Append("';");
            return _sb;
        }

        public string GetTableName()
        {
            return _tableName!;
        }

        public List<User> GetUsersWithRoleMatching(string roleMatch)
        {
            MySqlCommand com = GetCommandStoredProcedure("GetUsersWithRoleMatching");
            com.Parameters.AddWithValue("@userRole", roleMatch);
            com.Parameters["@userRole"].Direction = ParameterDirection.Input;

            _mySqlReader = com.ExecuteReader();

            return MapReaderToEntitiesList();
        }
    }

    [Test]
    public void Check_dao_table_name()
    {
        UserBaseDao dao = new UserBaseDao();
        Assert.That(dao.GetTableName(), Is.EqualTo("User"));
    }

    [Test]
    public void Check_succesfully_user_creation()
    {
        TruncateAllTables(_databaseConnection!);
        UserBaseDao dao = new UserBaseDao();
        User user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "UserName",
            Role = "Role"
        };

        int creationResult = dao.Create(user);

        Assert.That(creationResult, Is.EqualTo(1));
    }

    [Test]
    public void Check_duplicated_user_creation()
    {
        TruncateAllTables(_databaseConnection!);
        UserBaseDao dao = new UserBaseDao();
        User user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "UserName",
            Role = "Role"
        };

        int creationResult = dao.Create(user);
        Assert.That(creationResult, Is.EqualTo(1));

        int secondCreationResult = dao.Create(user);
        Assert.That(secondCreationResult, Is.EqualTo(-1));
    }

    [Test]
    public void Check_read_all_operation()
    {
        TruncateAllTables(_databaseConnection!);
        UserBaseDao dao = new UserBaseDao();
        List<User> users = new List<User>{
            new User()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000000"),
                UserName = "UserName1",
                Role = "Role1"
            },
            new User()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                UserName = "UserName2",
                Role = "Role2"
            },
            new User()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                UserName = "UserName3",
                Role = "Role3"
            },
        };

        foreach (User user in users)
        {
            dao.Create(user);
        }

        List<User> usersFromDB = dao.ReadAll();

        Assert.That(usersFromDB.ElementAt(0).Id, Is.EqualTo(users.ElementAt(0).Id));
        Assert.That(usersFromDB.ElementAt(1).Id, Is.EqualTo(users.ElementAt(1).Id));
        Assert.That(usersFromDB.ElementAt(2).Id, Is.EqualTo(users.ElementAt(2).Id));
    }

    [Test]
    public void Check_stored_procedure_operation()
    {
        TruncateAllTables(_databaseConnection!);
        UserBaseDao dao = new UserBaseDao();
        dao.Create(new User()
        {
            Id = Guid.NewGuid(),
            UserName = "UserName",
            Role = "RoleMatch"
        });
        dao.Create(new User()
        {
            Id = Guid.NewGuid(),
            UserName = "UserName",
            Role = "RoleNotMatch"
        });

        List<User> users = dao.GetUsersWithRoleMatching("RoleMatch");
        Assert.That(users.ElementAt(0).Role, Is.EqualTo("RoleMatch"));
    }

    [Test]
    public void Check_update_operation()
    {
        TruncateAllTables(_databaseConnection!);
        UserBaseDao dao = new UserBaseDao();
        Guid guid = Guid.NewGuid();
        User user = new User()
        {
            Id = guid,
            UserName = "UserName",
            Role = "UserRole"
        };
        dao.Create(user);

        user.UserName = "AnotherUserName";
        dao.Update(user);

        User userFromDB = dao.Read(guid)!;

        Assert.That(userFromDB.UserName, Is.EqualTo("AnotherUserName"));
    }

    [Test]
    public void Check_delete_operation()
    {
        TruncateAllTables(_databaseConnection!);
        UserBaseDao dao = new UserBaseDao();
        User user = new User()
        {
            Id = Guid.NewGuid(),
            UserName = "UserName",
            Role = "UserRole"
        };
        dao.Create(user);
        bool deleted = dao.Delete(user.Id);
        ClassicAssert.True(deleted);
    }

    [Test]
    public async Task Check_succesfully_user_creation_async()
    {
        TruncateAllTables(_databaseConnection!);
        UserBaseDao dao = new UserBaseDao();
        User user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "UserName",
            Role = "Role"
        };

        int creationResult = await dao.CreateAsync(user);

        Assert.That(creationResult, Is.EqualTo(1));
    }

    [Test]
    public async Task Check_duplicated_user_creation_async()
    {
        TruncateAllTables(_databaseConnection!);
        UserBaseDao dao = new UserBaseDao();
        User user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "UserName",
            Role = "Role"
        };

        int creationResult = await dao.CreateAsync(user);
        Assert.That(creationResult, Is.EqualTo(1));

        int secondCreationResult = await dao.CreateAsync(user);
        Assert.That(secondCreationResult, Is.EqualTo(-1));
    }

    [Test]
    public async Task Check_read_all_operation_async()
    {
        TruncateAllTables(_databaseConnection!);
        UserBaseDao dao = new UserBaseDao();
        List<User> users = new List<User>{
            new User()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000000"),
                UserName = "UserName1",
                Role = "Role1"
            },
            new User()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                UserName = "UserName2",
                Role = "Role2"
            },
            new User()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                UserName = "UserName3",
                Role = "Role3"
            },
        };

        foreach (User user in users)
        {
            await dao.CreateAsync(user);
        }

        List<User> usersFromDB = await dao.ReadAllAsync();

        Assert.That(usersFromDB.ElementAt(0).Id, Is.EqualTo(users.ElementAt(0).Id));
        Assert.That(usersFromDB.ElementAt(1).Id, Is.EqualTo(users.ElementAt(1).Id));
        Assert.That(usersFromDB.ElementAt(2).Id, Is.EqualTo(users.ElementAt(2).Id));
    }

    [Test]
    public async Task Check_update_operation_async()
    {
        TruncateAllTables(_databaseConnection!);
        UserBaseDao dao = new UserBaseDao();
        Guid guid = Guid.NewGuid();
        User user = new User()
        {
            Id = guid,
            UserName = "UserName",
            Role = "UserRole"
        };
        await dao.CreateAsync(user);

        user.UserName = "AnotherUserName";
        await dao.UpdateAsync(user);

        User userFromDB = (await dao.ReadAsync(guid))!;

        Assert.That(userFromDB.UserName, Is.EqualTo("AnotherUserName"));
    }

    [Test]
    public async Task Check_delete_operation_async()
    {
        TruncateAllTables(_databaseConnection!);
        UserBaseDao dao = new UserBaseDao();
        User user = new User()
        {
            Id = Guid.NewGuid(),
            UserName = "UserName",
            Role = "UserRole"
        };
        await dao.CreateAsync(user);
        bool deleted = await dao.DeleteAsync(user.Id);
        ClassicAssert.True(deleted);
    }
}
