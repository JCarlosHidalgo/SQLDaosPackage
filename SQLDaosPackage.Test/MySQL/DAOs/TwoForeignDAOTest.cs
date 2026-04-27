using NUnit.Framework.Legacy;

using SQLDaosPackage.DAOS.MySQL;

using System.Data;
using System.Text;
using System.Linq;

using MySql.Data.MySqlClient;

using Test.MySQL.Entities.Single;
using Test.MySQL.Entities.TwoForeign;
using Test.MySQL.Utils;

namespace Test.MySQL.DAOs;

[TestFixture]
public class TwoForeignDAOTest
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

    private void TruncateAllTables(MySqlConnection _conn)
    {
        MySqlCommand com = new MySqlCommand("TruncateAllTables", _conn);
        com.CommandType = CommandType.StoredProcedure;
        com.ExecuteNonQuery();
    }

    class UserBaseDAO : MySQLSingleDAO <User> 
    {
        public UserBaseDAO()
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
            return new List<User>();
        }

        protected override StringBuilder CreateCommandIntoStringBuilder(User entity)
        {
            string IdConverted = entity.Id.ToString();
            string UserNameConverted = entity.UserName;
            string RoleConverted = entity.Role;

            _sb = new StringBuilder();
            _sb.Append("INSERT INTO ").Append(_tableName).Append(" (Id,UserName,Role) ")
                .Append("VALUES ('").Append(IdConverted).Append("','")
                                    .Append(UserNameConverted).Append("','")
                                    .Append(RoleConverted).Append("');");
            return _sb;
        }

        protected override StringBuilder UpdateCommandIntoStringBuilder(User entity)
        {
            string IdConverted = entity.Id.ToString();
            string UserNameConverted = entity.UserName;
            string RoleConverted = entity.Role;

            _sb = new StringBuilder();
            _sb.Append("UPDATE ").Append(_tableName)
                .Append(" SET UserName = '").Append(UserNameConverted).Append("', ")
                .Append(" Role = '").Append(RoleConverted).Append("' ")
                .Append(" WHERE Id = '").Append(IdConverted).Append("';");
            return _sb;
        }

        public string GetTableName()
        {
            return _tableName!;
        }
    }

    class TenantBaseDAO : MySQLSingleDAO <Tenant> 
    {
        public TenantBaseDAO()
        {
            _connection = _databaseConnection!;
            _tableName = "Tenant";
        }

        protected override Tenant MapReaderToEntity()
        {
            return new Tenant
            {
                Id = _mySqlReader!.GetGuid("Id"),
                Name = _mySqlReader.GetString("Name"),
            };
        }

        protected override List<Tenant> MapReaderToEntitiesList()
        {
            return new List<Tenant>();
        }

        protected override StringBuilder CreateCommandIntoStringBuilder(Tenant entity)
        {
            string IdConverted = entity.Id.ToString();
            string NameConverted = entity.Name;

            _sb = new StringBuilder();
            _sb.Append("INSERT INTO ").Append(_tableName).Append(" (Id,Name) ")
                .Append("VALUES ('").Append(IdConverted).Append("','")
                                    .Append(NameConverted).Append("');");
            return _sb;
        }

        protected override StringBuilder UpdateCommandIntoStringBuilder(Tenant entity)
        {
            return new StringBuilder();
        }

        public string GetTableName()
        {
            return _tableName!;
        }
    }

    class TenantDomainDAO : MySQLTwoForeignDAO <TenantDomain> 
    {
        public TenantDomainDAO()
        {
            _connection = _databaseConnection!;
            _tableName = "TenantDomain";
            _firstForeignKey = "UserId";
            _secondForeignKey = "TenantId";
        }

        protected override TenantDomain MapReaderToEntity()
        {
            return new TenantDomain
            {
                UserId = _mySqlReader!.GetGuid("UserId"),
                TenantId = _mySqlReader.GetGuid("TenantId"),
            };
        }

        protected override List<TenantDomain> MapReaderToEntitiesList()
        {
            return new List<TenantDomain>();
        }

        protected override StringBuilder CreateCommandIntoStringBuilder(TenantDomain entity)
        {
            string UserIdConverted = entity.UserId.ToString();
            string TenantIdConverted = entity.TenantId.ToString();

            _sb = new StringBuilder();
            _sb.Append("INSERT INTO ").Append(_tableName).Append(" (UserId,TenantId) ")
                .Append("VALUES ('").Append(UserIdConverted).Append("','")
                                    .Append(TenantIdConverted).Append("');");
            return _sb;
        }

        protected override StringBuilder UpdateCommandIntoStringBuilder(TenantDomain entity)
        {
            return new StringBuilder();
        }

        public string GetTableName()
        {
            return _tableName!;
        }

        public string GetFirstForeignKeyName()
        {
            return _firstForeignKey!;
        }

        public string GetSecondForeignKeyName()
        {
            return _secondForeignKey!;
        }
    }

    [Test]
    public void Check_dao_table_name()
    {
        TenantDomainDAO dao = new TenantDomainDAO();
        Assert.That(dao.GetTableName(), Is.EqualTo("TenantDomain"));
    }

    [Test]
    public void Check_dao_foreign_keys_names()
    {
        TenantDomainDAO dao = new TenantDomainDAO();
        Assert.That(dao.GetFirstForeignKeyName(), Is.EqualTo("UserId"));
        Assert.That(dao.GetSecondForeignKeyName(), Is.EqualTo("TenantId"));
    }

    [Test]
    public void Check_succesfully_tenant_domain_creation()
    {
        TruncateAllTables(_databaseConnection!);
        
        User user = new User
                    {
                        Id = Guid.NewGuid(),
                        UserName = "UserName",
                        Role = "Role"
                    };

        Tenant tenant = new Tenant
                    {
                        Id = Guid.NewGuid(),
                        Name = "Tenant"
                    };
        
        TenantDomain tenantDomain = new TenantDomain
                    {
                        UserId = user.Id,
                        TenantId = tenant.Id
                    };

        UserBaseDAO userDao = new UserBaseDAO();
        TenantBaseDAO tenantDao = new TenantBaseDAO();
        TenantDomainDAO tenantDomainDao = new TenantDomainDAO();

        userDao.Create(user);
        tenantDao.Create(tenant);
        
        
        int creationResult = tenantDomainDao.Create(tenantDomain);

        Assert.That(creationResult, Is.EqualTo(1));
    }

    [Test]
    public void Check_read_operation()
    {
        TruncateAllTables(_databaseConnection!);
        
        User user = new User
                    {
                        Id = Guid.NewGuid(),
                        UserName = "UserName",
                        Role = "Role"
                    };

        Tenant tenant = new Tenant
                    {
                        Id = Guid.NewGuid(),
                        Name = "Tenant"
                    };
        
        TenantDomain tenantDomain = new TenantDomain
                    {
                        UserId = user.Id,
                        TenantId = tenant.Id
                    };

        UserBaseDAO userDao = new UserBaseDAO();
        TenantBaseDAO tenantDao = new TenantBaseDAO();
        TenantDomainDAO tenantDomainDao = new TenantDomainDAO();

        userDao.Create(user);
        tenantDao.Create(tenant);
        tenantDomainDao.Create(tenantDomain);

        TenantDomain tenantDomainFromDB = tenantDomainDao.Read(user.Id,tenant.Id)!;

        Assert.That(tenantDomain.UserId, Is.EqualTo(tenantDomainFromDB.UserId));
    }
    
    [Test]
    public void Check_delete_operation()
    {
        TruncateAllTables(_databaseConnection!);
        User user = new User
                    {
                        Id = Guid.NewGuid(),
                        UserName = "UserName",
                        Role = "Role"
                    };

        Tenant tenant = new Tenant
                    {
                        Id = Guid.NewGuid(),
                        Name = "Tenant"
                    };

        TenantDomain tenantDomain = new TenantDomain
                    {
                        UserId = user.Id,
                        TenantId = tenant.Id
                    };

        UserBaseDAO userDao = new UserBaseDAO();
        TenantBaseDAO tenantDao = new TenantBaseDAO();
        TenantDomainDAO tenantDomainDao = new TenantDomainDAO();

        userDao.Create(user);
        tenantDao.Create(tenant);
        tenantDomainDao.Create(tenantDomain);

        bool deleted = tenantDomainDao.Delete(tenantDomain.UserId,tenantDomain.TenantId);
        ClassicAssert.True(deleted);
    }

    [Test]
    public async Task Check_succesfully_tenant_domain_creation_async()
    {
        TruncateAllTables(_databaseConnection!);

        User user = new User
                    {
                        Id = Guid.NewGuid(),
                        UserName = "UserName",
                        Role = "Role"
                    };

        Tenant tenant = new Tenant
                    {
                        Id = Guid.NewGuid(),
                        Name = "Tenant"
                    };

        TenantDomain tenantDomain = new TenantDomain
                    {
                        UserId = user.Id,
                        TenantId = tenant.Id
                    };

        UserBaseDAO userDao = new UserBaseDAO();
        TenantBaseDAO tenantDao = new TenantBaseDAO();
        TenantDomainDAO tenantDomainDao = new TenantDomainDAO();

        await userDao.CreateAsync(user);
        await tenantDao.CreateAsync(tenant);

        int creationResult = await tenantDomainDao.CreateAsync(tenantDomain);

        Assert.That(creationResult, Is.EqualTo(1));
    }

    [Test]
    public async Task Check_read_operation_async()
    {
        TruncateAllTables(_databaseConnection!);

        User user = new User
                    {
                        Id = Guid.NewGuid(),
                        UserName = "UserName",
                        Role = "Role"
                    };

        Tenant tenant = new Tenant
                    {
                        Id = Guid.NewGuid(),
                        Name = "Tenant"
                    };

        TenantDomain tenantDomain = new TenantDomain
                    {
                        UserId = user.Id,
                        TenantId = tenant.Id
                    };

        UserBaseDAO userDao = new UserBaseDAO();
        TenantBaseDAO tenantDao = new TenantBaseDAO();
        TenantDomainDAO tenantDomainDao = new TenantDomainDAO();

        await userDao.CreateAsync(user);
        await tenantDao.CreateAsync(tenant);
        await tenantDomainDao.CreateAsync(tenantDomain);

        TenantDomain tenantDomainFromDB = (await tenantDomainDao.ReadAsync(user.Id, tenant.Id))!;

        Assert.That(tenantDomain.UserId, Is.EqualTo(tenantDomainFromDB.UserId));
    }

    [Test]
    public async Task Check_delete_operation_async()
    {
        TruncateAllTables(_databaseConnection!);
        User user = new User
                    {
                        Id = Guid.NewGuid(),
                        UserName = "UserName",
                        Role = "Role"
                    };

        Tenant tenant = new Tenant
                    {
                        Id = Guid.NewGuid(),
                        Name = "Tenant"
                    };

        TenantDomain tenantDomain = new TenantDomain
                    {
                        UserId = user.Id,
                        TenantId = tenant.Id
                    };

        UserBaseDAO userDao = new UserBaseDAO();
        TenantBaseDAO tenantDao = new TenantBaseDAO();
        TenantDomainDAO tenantDomainDao = new TenantDomainDAO();

        await userDao.CreateAsync(user);
        await tenantDao.CreateAsync(tenant);
        await tenantDomainDao.CreateAsync(tenantDomain);

        bool deleted = await tenantDomainDao.DeleteAsync(tenantDomain.UserId, tenantDomain.TenantId);
        ClassicAssert.True(deleted);
    }
}
