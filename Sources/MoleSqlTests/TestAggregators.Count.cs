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
        public void Count_Customers_WithoutPredicate()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            // ReSharper disable once ReplaceWithSingleCallToCount
            var result = context.Customers.Where(c => c.Id < 4).Count();
            result.Should().Be(3);
        }
        [TestMethod]
        public void Count_Customers_WithPredicate()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var result = context.Customers.Count(c => c.Id < 4);
            result.Should().Be(3);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void CountAsync_NotOnTop_NotSupportedException()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            context.Customers.Select(customer => new { T = context.Departments.CountAsync(default) }).ToList();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void CountAsync_NotOnTopPred_NotSupportedException()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            context.Customers.Select(customer => new {T = context.Customers.CountAsync(x => true, default)}).Select(a => a).ToList();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task CountAsync_WrongQueryProvider_Exception()
        {
            await new[] { 1 }.AsQueryable().CountAsync();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task CountAsyncPred_WrongQueryProvider_Exception()
        {
            await new[] { 1 }.AsQueryable().CountAsync(x => true);
        }
        [TestMethod]
        public async Task CountAsync_Customers_WithoutPredicate()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            // ReSharper disable once ReplaceWithSingleCallToCount
            var result = await context.Customers.Where(c => c.Id < 4).CountAsync();
            result.Should().Be(3);
        }
        [TestMethod]
        public async Task CountAsync_Customers_WithPredicate()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var result = await context.Customers.CountAsync(c => c.Id < 4);
            result.Should().Be(3);
        }
    }
}
