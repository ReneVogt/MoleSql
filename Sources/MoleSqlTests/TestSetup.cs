using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql;

namespace MoleSqlTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestSetup
    {
        class MyTestContext : TestContext
        {
            public override void WriteLine(string message) => Console.WriteLine(message);
            public override void WriteLine(string format, params object[] args) => Console.WriteLine(format, args);
            public override void AddResultFile(string fileName) => throw new NotImplementedException();
            public override void BeginTimer(string timerName) => throw new NotImplementedException();
            public override void EndTimer(string timerName) => throw new NotImplementedException();
            public override IDictionary Properties => throw new NotImplementedException();
            public override DataRow DataRow => throw new NotImplementedException();
            public override DbConnection DataConnection => throw new NotImplementedException();
        }

        static readonly MyTestContext mockedContext = new MyTestContext();

        [AssemblyInitialize]
        [SuppressMessage("Stil", "IDE0060:Nicht verwendete Parameter entfernen", Justification = "Test framework dictates signature.")]
        public static void AssemblyInitialize(TestContext testContext)
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
        }
        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            try
            {
                using var context = new MoleSqlDataContext(ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString);
                context.ExecuteNonQuery(
                    "USE master; ALTER DATABASE MoleSqlTestDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE MoleSqlTestDb;");
                File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestDb_log.ldf"));
            }
            catch (Exception e)
            {
                File.AppendAllText(@"C:\Privat\log.log", $"Failed to cleanup database: {e}");
            }
        }

        public static async Task RunTest<T>(string method) where T : class, new()
        {
            AssemblyInitialize(mockedContext);
            try
            {
                await Execute<T, ClassInitializeAttribute>(null, mockedContext);

                T testClass = new T();
                await Execute<T, TestInitializeAttribute>(testClass);

                await ExecuteTest(testClass, method);

                await Execute<T, TestCleanupAttribute>(testClass);
                await Execute<T, ClassCleanupAttribute>(null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                AssemblyCleanup();
            }
        }

        static Task ExecuteTest<T>(T testClass, string method) => typeof(T).GetMethod(method)?.Invoke(testClass, new object[0]) as Task ?? Task.CompletedTask;

        static Task Execute<T, TAttribute>(T o, params object[] args) where T : class where TAttribute : Attribute
            => GetMethod<T, TAttribute>(o != null)?.Invoke(o, args) as Task ?? Task.CompletedTask;
        static MethodInfo GetMethod<T, TAttribute>(bool instance) where TAttribute : Attribute =>
            typeof(T).GetMethods(BindingFlags.Public | (instance ? BindingFlags.Instance : BindingFlags.Static))
                     .FirstOrDefault(method => method.IsDecoratedWith<TAttribute>());
    }
}
