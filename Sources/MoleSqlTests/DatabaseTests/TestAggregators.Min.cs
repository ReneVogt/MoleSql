/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql.Extensions;
using MoleSqlTests.TestDb;

// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable InvokeAsExtensionMethod

// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace MoleSqlTests.DatabaseTests
{
    public partial class TestAggregators
    {
        [TestMethod]
        public void Min_WithoutSelector_CorrectValues()
        {
            using var context = GetDbContext();
            context.AggregatorTest.Select(e => e.IntValue).Min().Should().Be(-1);
        }
        [TestMethod]
        public void Min_WithSelector_CorrectValues()
        {
            using var context = GetDbContext();
            context.AggregatorTest.Min(x => x.NullableDoubleValue).Should().Be(-1);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void MinAsync_NotOnTop_NotSupportedException()
        {
            using var context = GetDbContext();
            context.AggregatorTest.Select(x => new { T = context.AggregatorTest.Select(c => c.IntValue).MinAsync(default) }).AsEnumerable().ToList();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void MinAsync_NotOnTopSelector_NotSupportedException()
        {
            using var context = GetDbContext();
            context.AggregatorTest.Select(x => new { T = context.AggregatorTest.MinAsync(c => c.IntValue, default) }).AsEnumerable().ToList();
        }
        [TestMethod]
        public async Task MinAsync_SourceNull_Exception()
        {
            await Task.WhenAll(
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.MinAsync((IQueryable<int>)null), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.MinAsync((IQueryable<int>)null, i => i), "source")
            );
        }
        [TestMethod]
        public async Task MinAsync_SelectorNull_Exception()
        {
            using var context = GetDbContext();
            var query = context.AggregatorTest;
            await ShouldThrowArgumentNullException(() => MoleSqlQueryable.MinAsync(query, (Expression<Func<AggregatorTestTable, int>>)null), "selector");
        }
        [TestMethod]
        public async Task MinAsync_WrongProvider_Exception()
        {
            await Task.WhenAll(
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.MinAsync(Enumerable.Empty<int>().AsQueryable()),
                                                 nameof(MoleSqlQueryable.MinAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.MinAsync(Enumerable.Empty<object>().AsQueryable(), o => 0),
                                                 nameof(MoleSqlQueryable.MinAsync))
            );
        }
        [TestMethod]
        public async Task MinAsync_CorrectValues()
        {
            using var context = GetDbContext();
            (await context.AggregatorTest.Select(a => a.IntValue).MinAsync()).Should().Be(-1, "should work for Int32");
            (await context.AggregatorTest.MinAsync(a => a.IntValue)).Should().Be(-1, "should work for Int32 with selector");
            (await context.AggregatorTest.Select(a => a.LongValue).MinAsync()).Should().Be(-1, "should work for Int64");
            (await context.AggregatorTest.MinAsync(a => a.LongValue)).Should().Be(-1, "should work for Int64 with selector");
            (await context.AggregatorTest.Select(a => a.FloatValue).MinAsync()).Should().Be(-1, "should work for Float");
            (await context.AggregatorTest.MinAsync(a => a.FloatValue)).Should().Be(-1, "should work for Float with selector");
            (await context.AggregatorTest.Select(a => a.DoubleValue).MinAsync()).Should().Be(-1, "should work for Double");
            (await context.AggregatorTest.MinAsync(a => a.DoubleValue)).Should().Be(-1, "should work for Double with selector");
            (await context.AggregatorTest.Select(a => a.DecimalValue).MinAsync()).Should().Be(-1, "should work for Decimal");
            (await context.AggregatorTest.MinAsync(a => a.DecimalValue)).Should().Be(-1, "should work for Decimal with selector");

            (await context.AggregatorTest.Select(a => a.NullableIntValue).MinAsync()).Should().Be(-1, "should work for Int32?");
            (await context.AggregatorTest.MinAsync(a => a.NullableIntValue)).Should().Be(-1, "should work for Int32? with selector");
            (await context.AggregatorTest.Select(a => a.NullableLongValue).MinAsync()).Should().Be(-1, "should work for Int64?");
            (await context.AggregatorTest.MinAsync(a => a.NullableLongValue)).Should().Be(-1, "should work for Int64? with selector");
            (await context.AggregatorTest.Select(a => a.NullableFloatValue).MinAsync()).Should().Be(-1, "should work for Float?");
            (await context.AggregatorTest.MinAsync(a => a.NullableFloatValue)).Should().Be(-1, "should work for Float? with selector");
            (await context.AggregatorTest.Select(a => a.NullableDoubleValue).MinAsync()).Should().Be(-1, "should work for Double?");
            (await context.AggregatorTest.MinAsync(a => a.NullableDoubleValue)).Should().Be(-1, "should work for Double? with selector");
            (await context.AggregatorTest.Select(a => a.NullableDecimalValue).MinAsync()).Should().Be(-1, "should work for Decimal?");
            (await context.AggregatorTest.MinAsync(a => a.NullableDecimalValue)).Should().Be(-1, "should work for Decimal? with selector");
        }
    }
}
