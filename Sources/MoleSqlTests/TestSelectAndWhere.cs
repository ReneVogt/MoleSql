/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MoleSqlTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestSelectAndWhere
    {
        [TestMethod]
        public void SelectAndWhere_WithNullCheck()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var query = from customer in context.Customers
                        where customer.Id < 4 && customer.Name != null
                        select customer.Id;
            var result = query.AsEnumerable().OrderBy(i => i).ToList();
            result.Should().Equal(1, 2, 3);
            MoleSqlTestContext.AssertSqlDump(context, "SELECT [t0].[Id] FROM Customers AS t0 WHERE (([t0].[Id] < @p0) AND ([t0].[Name] IS NOT NULL)) -- @p0 Int Input [4] ");
        }
        [TestMethod]
        public void SelectAndWhere_ValueTypes()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var query = from customer in context.Customers
                        where customer.Id < 4
                        select customer.Id;
            var result = query.AsEnumerable().OrderBy(i => i).ToList();
            result.Should().Equal(1, 2, 3);
            MoleSqlTestContext.AssertSqlDump(context, "SELECT [t0].[Id] FROM Customers AS t0 WHERE ([t0].[Id] < @p0) -- @p0 Int Input [4] ");
        }
    }
}
