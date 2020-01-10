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
    public class TestExecuteNonQuery : MoleSqlTestBase
    {
        [TestMethod]
        public void UdateRows_CorrectRowCount()
        {
            const string Name = "Alfons Allerlei";
            using var context = GetDbContextWithTransaction();
            context.ExecuteNonQuery($"UPDATE Customers SET Name = 'Hello World' WHERE [Name] = {Name}").Should().Be(1);
            AssertSql(context, "UPDATE Customers SET Name = 'Hello World' WHERE [Name] = @p0 -- @p0 NVarChar Input [Alfons Allerlei]");
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
