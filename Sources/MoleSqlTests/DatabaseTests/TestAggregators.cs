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
using MoleSql;
using MoleSql.Extensions;

// ReSharper disable AccessToDisposedClosure

// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace MoleSqlTests.DatabaseTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public partial class TestAggregators : MoleSqlTestBase
    {
        [TestMethod]
        public void AsyncAggregator_AfterDisposed_ObjectDisposedException()
        {
            var context = GetDbContext();
            var table = context.AggregatorTest;
            context.Dispose();
            table.Invoking(t => t.MaxAsync()).Should().Throw<ObjectDisposedException>().Where(e => e.ObjectName == nameof(QueryProvider));
        }
        [TestMethod]
        public void AggregatedSubqueryReferences_CorrectResult()
        {
            using var context = GetDbContext();
            var query = from employee in context.Employees
                        where employee.Name == "René"
                        select new
                        {
                            employee.Name,
                            OrderCount = context.Orders.Where(order => order.EmployeeId == employee.Id).Select(order => order.Id).Count()
                        };
            var result = query.ToList();

            result.Should().HaveCount(1);
            result[0].Name.Should().Be("René");
            result[0].OrderCount.Should().Be(2);
            AssertSql(context, @"
SELECT [t0].[Name], (
  SELECT COUNT(*)
  FROM [Orders] AS t3
  WHERE ([t3].[EmployeeId] = [t0].[Id])
  ) AS c0
FROM [Employees] AS t0
WHERE ([t0].[Name] = @p0)
-- @p0 NVarChar Input [René]");
        }
    }
}
