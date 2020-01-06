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

namespace MoleSqlTests.Basic
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class SelectAndWhereTests
    {
        [TestMethod]
        public void SelectAndWhere_ValueTypes()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var query = from customer in context.Customers
                        where customer.Id < 5
                        select customer.Id;
            var result = query.AsEnumerable().OrderBy(i => i).ToList();
            result.Should().Equal(1, 2, 3, 4);
            MoleSqlTestContext.AssertSqlDump(context, "SELECT [t0].[Id] FROM Customers AS t0 WHERE ([t0].[Id] < @p0) -- @p0 Int Input [5] ");
        }
    }
}
