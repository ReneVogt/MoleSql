﻿/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */
using System;
using System.Configuration;
using System.Data.SqlClient;
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
    public class MoleSqlTestBase
    {
        public TestContext TestContext { get; set; }

        internal TestDbContext GetDbContext() => new TestDbContext();
        internal TestDbContext GetDbContextWithTransaction() => new TestDbContext(true);
        internal void Log(string msg)
        {
            if (Environment.UserInteractive)
                Console.WriteLine(msg);
            else
                TestContext?.WriteLine(msg);
        }
        internal void AssertAndLogSql(TestDbContext context, string expected)
        {
            string log = context.LogBuilder.ToString();
            Log(log);
            Assert.AreEqual(NormalizeSql(expected), NormalizeSql(log));
        }
        internal void AssertSql(TestDbContext context, string expected)
        {
            string log = context.LogBuilder.ToString();
            Assert.AreEqual(NormalizeSql(expected), NormalizeSql(log));
        }
        static readonly char[] splitChars = { '\r', '\n', '\t', '\v', ' ' };
        static string NormalizeSql(string sql) => string.Join(" ", sql.Split(splitChars, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()));


        [AssemblyInitialize]
        [SuppressMessage("Stil", "IDE0060:Nicht verwendete Parameter entfernen", Justification = "Test framework dictates signature.")]
        public static void InitializeTestAssembly(TestContext testContext)
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestDb"));
        }
        [AssemblyCleanup]
        public static void CleanupTestAssembly()
        {
            try
            {
                var source = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString);
                var master = new SqlConnectionStringBuilder
                {
                    InitialCatalog = "master",
                    DataSource = source.DataSource,
                    IntegratedSecurity = true
                };
                using var context = new DataContext(master.ToString());
                context.ExecuteNonQuery(
                    $"ALTER DATABASE MoleSqlTestDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE MoleSqlTestDb;");
                File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestDb_log.ldf"));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to cleanup database: {e}");
            }
        }

        public static async Task RunTest<T>(string method) where T : class, new()
        {
            InitializeTestAssembly(null);
            try
            {
                await Execute<T, ClassInitializeAttribute>(null, (TestContext)null);

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
                CleanupTestAssembly();
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