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
        public void Average_Customers_WithoutSelector()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            // ReSharper disable once ReplaceWithSingleCallToCount
            var result = context.Customers.Where(c => c.Id < 4).Select(c => c.Id).Average();
            result.Should().Be(2);
        }
        [TestMethod]
        public void Average_Customers_WithSelector()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            // ReSharper disable once ReplaceWithSingleCallToCount
            var result = context.Customers.Where(c => c.Id < 4).Average(c => c.Id);
            result.Should().Be(2);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void AverageAsync_NotOnTop_NotSupportedException()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            context.Customers.Select(customer => new { T = context.Customers.AverageAsync(c => c.Id, default) }).AsEnumerable().ToList();
        }
        [TestMethod]
        public async Task AverageAsync_Customers_WithoutSelector()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            // ReSharper disable once ReplaceWithSingleCallToCount
            var result = await context.Customers.Where(c => c.Id < 4).Select(c => c.Id).AverageAsync();
            result.Should().Be(2);
        }
        [TestMethod]
        public async Task AverageAsync_Customers_WithSelector()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            // ReSharper disable once ReplaceWithSingleCallToCount
            var result = await context.Customers.Where(c => c.Id < 4).AverageAsync(c => c.Id);
            result.Should().Be(2);
        }
    }
}
