/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql;
using MoleSqlTests.TestDb;

namespace MoleSqlTests.DatabaseTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestTransactions : MoleSqlTestBase
    {
        const string transactionName = "MoleSqlTransaction";

        [TestMethod]
        public async Task Transaction_NoNameNorLevel()
        {
            using var context = new DataContext(TestDbContext.ConnectionString);
            context.Connection.Should().NotBeNull();
            using var _ = context.Transaction = context.BeginTransaction();
            var result = await context.ExecuteScalarAsync<string>($"SELECT name FROM sys.dm_tran_active_transactions WHERE name = 'user_transaction'");
            result.Should().Be("user_transaction");
        }
        [TestMethod]
        public async Task Transaction_NoNameButLevel()
        {
            using var context = new DataContext(TestDbContext.ConnectionString);
            context.Connection.Should().NotBeNull();
            using var _ = context.Transaction = context.BeginTransaction(IsolationLevel.ReadCommitted);
            var result = await context.ExecuteScalarAsync<string>($"SELECT name FROM sys.dm_tran_active_transactions WHERE name = 'user_transaction'");
            result.Should().Be("user_transaction");
        }
        [TestMethod]
        public async Task Transaction_NoLevelButName()
        {
            StringBuilder logBuilder = new StringBuilder();
            using var context = new DataContext(TestDbContext.ConnectionString) {Log = new StringWriter(logBuilder)};
            string contextInfo =
                $"-- Context: {typeof(QueryProvider).FullName} v{typeof(QueryProvider).Assembly.GetName().Version}, {context.Connection.DataSource}\\{context.Connection.Database}";
            context.Connection.Should().NotBeNull();
            using var _ = context.Transaction = context.BeginTransaction(transactionName);
            var result = await context.ExecuteScalarAsync<string>($"SELECT name FROM sys.dm_tran_active_transactions WHERE name = {transactionName}");
            result.Should().Be(transactionName);
            AssertSql(logBuilder.ToString(), $"SELECT name FROM sys.dm_tran_active_transactions WHERE name = @p0 -- @p0 Input NVarChar (Size = 18; Prec = 0; Scale = 0) [MoleSqlTransaction] {contextInfo}");
        }
        [TestMethod]
        public async Task Transaction_NameAndLevel()
        {
            using var connection = new SqlConnection(TestDbContext.ConnectionString);
            using var context = new DataContext(connection);
            context.Connection.Should().BeSameAs(connection);
            using var _ = context.Transaction = context.BeginTransaction(IsolationLevel.ReadUncommitted, transactionName);
            var result = await context.ExecuteScalarAsync<string>($"SELECT name FROM sys.dm_tran_active_transactions WHERE name = {transactionName}");
            result.Should().Be(transactionName);
        }
    }
}
