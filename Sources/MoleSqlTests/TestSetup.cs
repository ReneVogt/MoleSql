using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql;

namespace MoleSqlTests
{
    static class TestSetup
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
        }
        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            using var context = new MoleSqlDataContext(ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString);
            context.ExecuteNonQuery("USE master; ALTER DATABASE MoleSqlTestDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE MoleSqlTestDb;");
        }

        public static Task RunTest<T>(string method)
        {
            return Task.CompletedTask;
        }
    }
}
