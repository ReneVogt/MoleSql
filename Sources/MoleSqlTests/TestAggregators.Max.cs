/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql.Extensions;

// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace MoleSqlTests
{
    public partial class TestAggregators
    {
        [TestMethod]
        public void Max_MaxEmployeeSalary_WithoutSelector()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var result = context.Employees.Select(e => e.Salary).Max();
            result.Should().Be(200000);
        }
        [TestMethod]
        public void Max_MaxEmployeeSalary_WithSelector()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var result = context.Employees.Max(e => e.Salary);
            result.Should().Be(200000);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void MaxAsync_NotOnTop_NotSupportedException()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            context.Customers.Select(customer => new { T = context.Customers.MaxAsync(c => c.Id, default) }).AsEnumerable().ToList();
        }
        [TestMethod]
        public async Task MaxAsync_MaxEmployeeSalary_WithoutSelector()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var result = await context.Employees.Select(e => e.Salary).MaxAsync();
            result.Should().Be(200000);
        }
        [TestMethod]
        public async Task MaxAsync_MaxEmployeeSalary_WithSelector()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var result = await context.Employees.MaxAsync(e => e.Salary);
            result.Should().Be(200000);
        }
    }
}
