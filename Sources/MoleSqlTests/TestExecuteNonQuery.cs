/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MoleSqlTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestExecuteNonQuery
    {
        [TestMethod]
        public void DeleteRows_CorrectRowCount()
        {
            const string Name = "Alfons Allerlei";
            using var context = MoleSqlTestContext.GetDbContextWithTransaction();
            context.ExecuteNonQuery($"DELETE FROM Customers WHERE [Name] = {Name}").Should().Be(1);
            MoleSqlTestContext.AssertSqlDump(context, "DELETE FROM Customers WHERE [Name] = @p0 -- @p0 NVarChar Input [Alfons Allerlei]");
        }
        [TestMethod]
        public async Task ExecuteNonQueryAsync_YieldsCorrectResult()
        {
            using var context = MoleSqlTestContext.GetDbContextWithTransaction();

            int result = await context.ExecuteNonQueryAsync($"DELETE FROM Customers WHERE Name = 'Alfons Allerlei'");
            result.Should().Be(1);
        }
    }
}
