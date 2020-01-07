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
        public void Min_MinCustomerId_WithoutSelector()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var result = context.Customers.Select(customer => customer.Id).Min();
            result.Should().Be(1);
        }
        [TestMethod]
        public void Min_MinEmployeeBirthDate_WithSelector()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var result = context.Employees.Min(e => e.DateOfBirth);
            result.Should().Be(new DateTime(1970, 1, 1));
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void MinAsync_NotOnTop_NotSupportedException()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            context.Customers.Select(customer => new { T = context.Customers.Select(c => c.Id).MinAsync(default) }).AsEnumerable().ToList();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void MinAsync_NotOnTopSelector_NotSupportedException()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            context.Customers.Select(customer => new { T = context.Customers.MinAsync(c => c.Id, default) }).AsEnumerable().ToList();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task MinAsync_WrongQueryProvider_Exception()
        {
            await new[] { 1 }.AsQueryable().MinAsync();
        }
        [TestMethod]
        public async Task MinAsync_MinCustomerId_WithoutSelector()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var result = await context.Customers.Select(customer => customer.Id).MinAsync();
            result.Should().Be(1);
        }
        [TestMethod]
        public async Task MinAsync_MinEmployeeBirthDate_WithSelector()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var result = await context.Employees.MinAsync(e => e.DateOfBirth);
            result.Should().Be(new DateTime(1970, 1, 1));
        }
    }
}
