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

namespace MoleSqlTests.DatabaseTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestNextedQueries : MoleSqlTestBase
    {
        [TestMethod]
        public void NestedQuery_ReferencedList_CorrectResult()
        {
            using var context = GetDbContext();
            var query = from employee in context.Employees
                        where employee.Name == "René"
                        select new
                        {
                            employee.Name, Orders = context.Orders.Where(order => order.EmployeeId == employee.Id).OrderBy(order => order.Id)
                        };
            var result = query.AsEnumerable().Select(x => new {x.Name, Orders = x.Orders.AsEnumerable().ToList()}).ToList();

            result.Should().HaveCount(1);
            result[0].Name.Should().Be("René");
            result[0].Orders.Should().HaveCountGreaterThan(0);
            result[0].Orders[0].Id.Should().Be(1);
            AssertSql(context, @"
SELECT [t0].[Name], [t0].[Id] 
FROM [Employees] AS t0 WHERE ([t0].[Name] = @p0) 
-- @p0 NVarChar Input [René] 
            
SELECT [t3].[Id], [t3].[CustomerId], [t3].[EmployeeId], [t3].[Date], [t3].[Discount] 
FROM [Orders] AS t3 WHERE ([t3].[EmployeeId] = @p0) 
ORDER BY [t3].[Id] 
-- @p0 Int Input [5]");
        }
        [TestMethod]
        public void NestedQuery_GroupedToIGroupings_IEnumerableToIQueryable()
        {
            using var context = GetDbContext();
            var query = from employee in context.Employees
                        where employee.Name == "René"
                        select new
                        {
                            employee.Name,
                            OrderCount = context.Orders.GroupBy(order => order.CustomerId)
                        };
            var result = query.ToList();
            result.Should().HaveCount(1);
            // TODO: check result when query works
            AssertSql(context, @"");
        }
        [TestMethod]
        public void NestedQuery_GroupedToIGroupingsWithSelector_IEnumerableToIQueryable()
        {
            using var context = GetDbContext();
            var query = from employee in context.Employees
                        where employee.Name == "René"
                        select new
                        {
                            employee.Name,
                            OrderCount = context.Orders.GroupBy(order => order.CustomerId).Select(g => g.Count())
                        };
            var result = query.ToList();
            result.Should().HaveCount(1);
            // TODO: check result when query works
            AssertSql(context, @"");
        }
    }
}
