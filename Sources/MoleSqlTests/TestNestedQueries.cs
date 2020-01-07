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
// ReSharper disable AccessToDisposedClosure

namespace MoleSqlTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestNextedQueries
    {
        [TestMethod]
        public void NestedQuery_CorrectResult()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var query = from employee in context.Employees
                        where employee.Name == "René"
                        select new {employee.Name, Orders = context.Orders.Where(order => order.EmployeeId == employee.Id).OrderBy(order => order.Id)};
            var result = query.AsEnumerable().Select(x => new {x.Name, Orders = x.Orders.AsEnumerable().ToList()}).ToList();

            result.Should().HaveCount(1);
            result[0].Name.Should().Be("René");
            result[0].Orders.Should().HaveCountGreaterThan(0);
            result[0].Orders[0].Id.Should().Be(1);
            MoleSqlTestContext.AssertSqlDump(context, "DELETE FROM Customers WHERE [Name] = @p0 -- @p0 NVarChar Input [Alfons Allerlei]");
        }
    }
}
