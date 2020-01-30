/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql;

namespace MoleSqlTests.DatabaseTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestExecuteNonQuery : MoleSqlTestBase
    {
        [TestMethod]
        public void ExecuteNonQuery_AfterDispose_ObjectDisposedException()
        {
            var context = GetDbContext();
            context.Dispose();
            context.Invoking(ctx => ctx.ExecuteNonQuery($""))
                   .Should()
                   .Throw<ObjectDisposedException>()
                   .Where(e => e.ObjectName == nameof(QueryProvider));
        }
        [TestMethod]
        public void ExecuteNonQuery_UdateRows_CorrectRowCount()
        {
            const string Name = "Alfons Allerlei";
            using var context = GetDbContextWithTransaction();
            context.ExecuteNonQuery($"UPDATE Customers SET Name = 'Hello World' WHERE [Name] = {Name}").Should().Be(1);
            AssertSql(context, $@"UPDATE Customers SET Name = 'Hello World' WHERE [Name] = @p0
-- @p0 Input NVarChar (Size = 15; Prec = 0; Scale = 0) [Alfons Allerlei]
{context.ContextInfo}");
        }
        [TestMethod]
        public void ExecuteNonQueryAsync_AfterDispose_ObjectDisposedException()
        {
            var context = GetDbContext();
            context.Dispose();
            context.Awaiting(async ctx => await ctx.ExecuteNonQueryAsync($""))
                   .Should()
                   .Throw<ObjectDisposedException>()
                   .Where(e => e.ObjectName == nameof(QueryProvider));
        }
        [TestMethod]
        public async Task ExecuteNonQueryAsync_YieldsCorrectResult()
        {
            using var context = GetDbContextWithTransaction();

            int result = await context.ExecuteNonQueryAsync($"UPDATE Customers SET Name = 'Hello World' WHERE Name = 'Alfons Allerlei'");
            result.Should().Be(1);
        }
    }
}
