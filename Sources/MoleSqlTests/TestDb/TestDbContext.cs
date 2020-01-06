using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using MoleSql;

namespace MoleSqlTests.TestDb
{
    [ExcludeFromCodeCoverage]
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
