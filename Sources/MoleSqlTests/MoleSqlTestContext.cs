﻿using System;
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
using MoleSqlTests.TestDb;

namespace MoleSqlTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class MoleSqlTestContext : TestContext
    {
        public static TestContext TestContext { get; private set; }

        [AssemblyInitialize]
        [SuppressMessage("Stil", "IDE0060:Nicht verwendete Parameter entfernen", Justification = "Test framework dictates signature.")]
        public static void AssemblyInitialize(TestContext testContext)
        {
            TestContext = testContext;
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestDb"));
        }
        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            try
            {
                using var context = new MoleSqlDataContext(ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString);
                context.ExecuteNonQuery(
                    $"USE master; ALTER DATABASE MoleSqlTestDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE MoleSqlTestDb;");
                File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestDb_log.ldf"));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to cleanup database: {e}");
            }
        }

        public static async Task RunTest<T>(string method) where T : class, new()
        {
            var context = new MoleSqlTestContext();
            AssemblyInitialize(context);
            try
            {
                await Execute<T, ClassInitializeAttribute>(null, context);

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
        internal static TestDbContext GetDbContext() => new TestDbContext {Log = new StringWriter()};
        internal static TestDbContext GetDbContextWithTransaction() => new TestDbContext(true) {Log = new StringWriter()};
        internal static void AssertSqlDump(MoleSqlDataContext context, string expected)
        {
            if (!(context?.Log is StringWriter sw)) return;
            context.Log = new StringWriter();

            var splitChars = new[] {'\r', '\n', '\t', '\v', ' '};

            expected = string.Join(" ", expected.Split(splitChars, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()));
            string log = string.Join(" ", sw.ToString().Split(splitChars, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()));
            Assert.AreEqual(expected, log);
        }

        static Task ExecuteTest<T>(T testClass, string method) => typeof(T).GetMethod(method)?.Invoke(testClass, new object[0]) as Task ?? Task.CompletedTask;

        static Task Execute<T, TAttribute>(T o, params object[] args) where T : class where TAttribute : Attribute
            => GetMethod<T, TAttribute>(o != null)?.Invoke(o, args) as Task ?? Task.CompletedTask;
        static MethodInfo GetMethod<T, TAttribute>(bool instance) where TAttribute : Attribute =>
            typeof(T).GetMethods(BindingFlags.Public | (instance ? BindingFlags.Instance : BindingFlags.Static))
                     .FirstOrDefault(method => method.IsDecoratedWith<TAttribute>());
    
        public override void WriteLine(string message) => Console.WriteLine(message);
        public override void WriteLine(string format, params object[] args) => Console.WriteLine(format, args);
        public override void AddResultFile(string fileName) => throw new NotImplementedException();
        public override void BeginTimer(string timerName) => throw new NotImplementedException();
        public override void EndTimer(string timerName) => throw new NotImplementedException();
        public override IDictionary Properties => throw new NotImplementedException();
        public override DataRow DataRow => throw new NotImplementedException();
        public override DbConnection DataConnection => throw new NotImplementedException();
    }
}
