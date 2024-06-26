using Login.Data.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.UnitTests.Repositories.Context
{
    public class BaseRepositoryTests
    {
        protected TestContext CreateInMemoryContext()
        {
            var _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<LoginContext>()
                .UseSqlite(_connection)
                .Options;

            return new TestContext(options);
        }
    }
}
