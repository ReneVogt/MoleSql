/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable AccessToDisposedClosure

namespace MoleSqlTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestJoinAndSelectMany : MoleSqlTestBase
    {
        [TestMethod]
        public void SimpleInnerJoin()
        {
            using var context = GetDbContext();
            var query = from customer in context.Customers
                        join order in context.Orders
                            on customer.Id equals order.CustomerId
                        join employee in context.Employees
                            on order.EmployeeId equals employee.Id
                        where employee.Name == "René"
                        select new {Customer = customer.Name, Employee = employee.Name, order.Date};
            var result = query.AsEnumerable().OrderBy(x => x.Date).ToList();
            result.Should().HaveCount(2);
            result[0].Customer.Should().Be("Alfons Allerlei");
            result[0].Employee.Should().Be("René");
            result[0].Date.Should().Be(new DateTime(2020, 1, 7));
            result[1].Customer.Should().Be("Alfons Allerlei");
            result[1].Employee.Should().Be("René");
            result[1].Date.Should().Be(new DateTime(2020, 1, 8));
            AssertAndLogSql(context, @"
SELECT [t7].[Name], [t7].[Name1], [t7].[Date] 
FROM ( 
    SELECT [t4].[Name], [t4].[Date], [t5].[Name] AS Name1 
    FROM ( 
        SELECT [t0].[Name], [t2].[EmployeeId], [t2].[Date] 
        FROM [Customers] AS t0 
        INNER JOIN [Orders] AS t2 
            ON ([t0].[Id] = [t2].[CustomerId])
    ) AS t4 
    INNER JOIN [Employees] AS t5 
        ON ([t4].[EmployeeId] = [t5].[Id]) 
) AS t7 
WHERE ([t7].[Name1] = @p0) 
-- @p0 NVarChar Input [René]");
        }
        [TestMethod]
        public void SimpleSelectMany()
        {
            using var context = GetDbContext();
            var query = from employee in context.Employees
                        from order in context.Orders
                        where employee.Name == "René" && employee.Id == order.EmployeeId
                        select new {employee.Salary, order.Date};
            var result = query.AsEnumerable().OrderBy(x => x.Date).ToList();
            result.Should().HaveCount(2);
            result[0].Salary.Should().Be(1);
            result[0].Date.Should().Be(new DateTime(2020, 1, 7));
            result[1].Salary.Should().Be(1);
            result[1].Date.Should().Be(new DateTime(2020, 1, 8));
            AssertAndLogSql(context, @"
SELECT [t0].[Salary], [t2].[Date] 
FROM [Employees] AS t0 
CROSS JOIN [Orders] AS t2 
WHERE (([t0].[Name] = @p0) AND ([t0].[Id] = [t2].[EmployeeId]))
-- @p0 NVarChar Input [René]");
        }
        [TestMethod]
        public void LittleMoreComplexSelectMany()
        {
            using var context = GetDbContext();
            var query = from employee in context.Employees
                        from order in context.Orders
                        from customer in context.Customers
                        where employee.Name == "René" && employee.Id == order.EmployeeId && order.CustomerId == customer.Id
                        select new { customer.Name, employee.Salary, order.Date };
            var result = query.AsEnumerable().OrderBy(x => x.Date).ToList();
            result.Should().HaveCount(2);
            result[0].Name.Should().Be("Alfons Allerlei");
            result[0].Salary.Should().Be(1);
            result[0].Date.Should().Be(new DateTime(2020, 1, 7));
            result[1].Name.Should().Be("Alfons Allerlei");
            result[1].Salary.Should().Be(1);
            result[1].Date.Should().Be(new DateTime(2020, 1, 8));
            AssertAndLogSql(context, @"
SELECT [t7].[Name1], [t7].[Salary], [t7].[Date] 
FROM ( 
    SELECT [t4].[Id], [t4].[Name], [t4].[Salary], [t4].[CustomerId], [t4].[EmployeeId], [t4].[Date], [t5].[Id] AS Id2, [t5].[Name] AS Name1 
    FROM ( 
        SELECT [t0].[Id], [t0].[Name], [t0].[Salary], [t2].[CustomerId], [t2].[EmployeeId], [t2].[Date] 
        FROM [Employees] AS t0 
        CROSS JOIN [Orders] AS t2
    ) AS t4 
    CROSS JOIN [Customers] AS t5
) AS t7 
WHERE ((([t7].[Name] = @p0) AND ([t7].[Id] = [t7].[EmployeeId])) AND ([t7].[CustomerId] = [t7].[Id2])) 
-- @p0 NVarChar Input [René]");
        }
    }
}
