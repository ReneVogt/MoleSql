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

// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable InvokeAsExtensionMethod

// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace MoleSqlTests
{
    public partial class TestAggregators
    {
        [TestMethod]
        public void Count_Customers_WithoutPredicate()
        {
            using var context = GetDbContext();
            // ReSharper disable once ReplaceWithSingleCallToCount
            var result = context.Customers.Where(c => c.Id < 4).Count();
            result.Should().Be(3);
        }
        [TestMethod]
        public void Count_Customers_WithPredicate()
        {
            using var context = GetDbContext();
            var result = context.Customers.Count(c => c.Id < 4);
            result.Should().Be(3);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void CountAsync_NotOnTop_NotSupportedException()
        {
            using var context = GetDbContext();
            context.Customers.Select(customer => new { T = context.Departments.CountAsync(default) }).ToList();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void CountAsync_NotOnTopPredicate_NotSupportedException()
        {
            using var context = GetDbContext();
            context.Customers.Select(customer => new {T = context.Customers.CountAsync(x => true, default)}).Select(a => a).ToList();
        }
        [TestMethod]
        public async Task CountAsync_SourceNull_Exception()
        {
            await Task.WhenAll(
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.CountAsync((IQueryable<int>)null), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.CountAsync((IQueryable<int>)null, i => i > 0), "source")
            );
        }
        [TestMethod]
        public async Task CountAsync_SelectorNull_Exception()
        {
            using var context = GetDbContext();
            var query = context.AggregatorTest;
            await ShouldThrowArgumentNullException(() => MoleSqlQueryable.CountAsync(query, null), "predicate");
        }
        [TestMethod]
        public async Task CountAsync_WrongProvider_Exception()
        {
            await Task.WhenAll(
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.CountAsync(Enumerable.Empty<int>().AsQueryable()), nameof(MoleSqlQueryable.CountAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.CountAsync(Enumerable.Empty<int>().AsQueryable(), i => i > 5))
            );
        }
        [TestMethod]
        public async Task CountAsync_WithoutPredicate_CorrectValues()
        {
            using var context = GetDbContext();
            // ReSharper disable once ReplaceWithSingleCallToCount
            var result = await context.AggregatorTest.Where(c => c.IntValue > 3).CountAsync();
            result.Should().Be(2);
        }
        [TestMethod]
        public async Task CountAsync_WithPredicate_CorrectValues()
        {
            using var context = GetDbContext();
            var result = await context.AggregatorTest.CountAsync(c => c.IntValue < 4);
            result.Should().Be(5);
        }
    }
}
