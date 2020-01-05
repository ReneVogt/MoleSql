using System;
using System.Configuration;
using MoleSql;

namespace MoleSqlTests.TestDb
{
    class TestDbContext : MoleSqlDataContext
    {
        public TestDbContext()
            : base(ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString)
        {
            Log = Console.Out;
        }
        //MoleQuery<Customers> Customers => GetTable<Customers>();
    }
}
