/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql.QueryProviders;

namespace MoleSqlTests.Asynchronous
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ExecuteAsyncTests
    {
        [TestMethod]
        public async Task ToListAsync_BasicTest()
        {
            using var context = MoleSqlTestContext.GetDbContext();

            var result = await context.Customers.Where(c => c.Id < 5).ToListAsync();
            result[0].Id.Should().Be(1);
            result[0].Name.Should().Be("Alfred");
            result[1].Id.Should().Be(2);
            result[1].Name.Should().Be("Beate");
            result[2].Id.Should().Be(3);
            result[2].Name.Should().Be("Christian");
            result[3].Id.Should().Be(4);
            result[3].Name.Should().Be("Detlef");

            MoleSqlTestContext.AssertSqlDump(context, "SELECT [t0].[Id], [t0].[Name], [t0].[Age] FROM Customers AS t0 WHERE ([t0].[Id] < @p0) -- @p0 Int Input [5]");
        }
    }
}
