using System.Data;
using System.Linq;
using System.Text;

using MySql.Data.MySqlClient;

using NUnit.Framework.Legacy;

using SQLDaosPackage.Daos.MySQL;

using Test.MySQL.Entities.Single;
using Test.MySQL.Entities.TwoForeign;
using Test.MySQL.Utils;

namespace Test.MySQL.Daos;

[TestFixture]
public class TwoForeignDaoTest
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
            return new List<User>();
        }

        public string GetTableName()
        {
            return _tableName!;
        }
    }

    class TenantBaseDao : MySQLSingleDao<Tenant>
    {
        public TenantBaseDao()
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

        public string GetTableName()
        {
            return _tableName!;
        }
    }

    class TenantDomainDao : MySQLTwoForeignDao<TenantDomain>
    {
        public TenantDomainDao()
        {
            _connection = _databaseConnection!;
            _tableName = "TenantDomain";
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
        TenantDomainDao dao = new TenantDomainDao();
        Assert.That(dao.GetTableName(), Is.EqualTo("TenantDomain"));
    }

    [Test]
    public void Check_dao_foreign_keys_names()
    {
        TenantDomainDao dao = new TenantDomainDao();
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

        UserBaseDao userDao = new UserBaseDao();
        TenantBaseDao tenantDao = new TenantBaseDao();
        TenantDomainDao tenantDomainDao = new TenantDomainDao();

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

        UserBaseDao userDao = new UserBaseDao();
        TenantBaseDao tenantDao = new TenantBaseDao();
        TenantDomainDao tenantDomainDao = new TenantDomainDao();

        userDao.Create(user);
        tenantDao.Create(tenant);
        tenantDomainDao.Create(tenantDomain);

        TenantDomain tenantDomainFromDB = tenantDomainDao.Read(user.Id, tenant.Id)!;

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

        UserBaseDao userDao = new UserBaseDao();
        TenantBaseDao tenantDao = new TenantBaseDao();
        TenantDomainDao tenantDomainDao = new TenantDomainDao();

        userDao.Create(user);
        tenantDao.Create(tenant);
        tenantDomainDao.Create(tenantDomain);

        bool deleted = tenantDomainDao.Delete(tenantDomain.UserId, tenantDomain.TenantId);
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

        UserBaseDao userDao = new UserBaseDao();
        TenantBaseDao tenantDao = new TenantBaseDao();
        TenantDomainDao tenantDomainDao = new TenantDomainDao();

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

        UserBaseDao userDao = new UserBaseDao();
        TenantBaseDao tenantDao = new TenantBaseDao();
        TenantDomainDao tenantDomainDao = new TenantDomainDao();

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

        UserBaseDao userDao = new UserBaseDao();
        TenantBaseDao tenantDao = new TenantBaseDao();
        TenantDomainDao tenantDomainDao = new TenantDomainDao();

        await userDao.CreateAsync(user);
        await tenantDao.CreateAsync(tenant);
        await tenantDomainDao.CreateAsync(tenantDomain);

        bool deleted = await tenantDomainDao.DeleteAsync(tenantDomain.UserId, tenantDomain.TenantId);
        ClassicAssert.True(deleted);
    }
}
