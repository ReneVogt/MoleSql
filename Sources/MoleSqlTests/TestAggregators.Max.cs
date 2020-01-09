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

namespace MoleSqlTests
{
    public partial class TestAggregators
    {
        [TestMethod]
        public void Max__WithoutSelector_CorrectValues()
        {
            using var context = GetDbContext();
            context.AggregatorTest.Select(e => e.IntValue).Max().Should().Be(5);
        }
        [TestMethod]
        public void Max__WithSelector_CorrectValues()
        {
            using var context = GetDbContext();
            context.AggregatorTest.Max(x => x.NullableDoubleValue).Should().Be(5);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void MaxAsync_NotOnTop_NotSupportedException()
        {
            using var context = GetDbContext();
            context.AggregatorTest.Select(x => new { T = context.AggregatorTest.Select(c => c.IntValue).MaxAsync(default) }).AsEnumerable().ToList();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void MaxAsync_NotOnTopSelector_NotSupportedException()
        {
            using var context = GetDbContext();
            context.AggregatorTest.Select(x => new { T = context.AggregatorTest.MaxAsync(c => c.IntValue, default) }).AsEnumerable().ToList();
        }
        [TestMethod]
        public async Task MaxAsync_SourceNull_Exception()
        {
            await Task.WhenAll(
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.MaxAsync((IQueryable<int>)null), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.MaxAsync((IQueryable<int>)null, i => i), "source")
            );
        }
        [TestMethod]
        public async Task MaxAsync_SelectorNull_Exception()
        {
            using var context = GetDbContext();
            var query = context.AggregatorTest;
            await ShouldThrowArgumentNullException(() => MoleSqlQueryable.MaxAsync(query, (Expression<Func<AggregatorTestTable, int>>)null), "selector");
        }
        [TestMethod]
        public async Task MaxAsync_WrongProvider_Exception()
        {
            await Task.WhenAll(
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.MaxAsync(Enumerable.Empty<int>().AsQueryable()),
                                                 nameof(MoleSqlQueryable.MaxAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.MaxAsync(Enumerable.Empty<object>().AsQueryable(), o => 0),
                                                 nameof(MoleSqlQueryable.MaxAsync))
            );
        }
        [TestMethod]
        public async Task MaxAsync_CorrectValues()
        {
            using var context = GetDbContext();
            (await context.AggregatorTest.Select(a => a.IntValue).MaxAsync()).Should().Be(5, "should work for Int32");
            (await context.AggregatorTest.MaxAsync(a => a.IntValue)).Should().Be(5, "should work for Int32 with selector");
            (await context.AggregatorTest.Select(a => a.LongValue).MaxAsync()).Should().Be(5, "should work for Int64");
            (await context.AggregatorTest.MaxAsync(a => a.LongValue)).Should().Be(5, "should work for Int64 with selector");
            (await context.AggregatorTest.Select(a => a.FloatValue).MaxAsync()).Should().Be(5, "should work for Float");
            (await context.AggregatorTest.MaxAsync(a => a.FloatValue)).Should().Be(5, "should work for Float with selector");
            (await context.AggregatorTest.Select(a => a.DoubleValue).MaxAsync()).Should().Be(5, "should work for Double");
            (await context.AggregatorTest.MaxAsync(a => a.DoubleValue)).Should().Be(5, "should work for Double with selector");
            (await context.AggregatorTest.Select(a => a.DecimalValue).MaxAsync()).Should().Be(5, "should work for Decimal");
            (await context.AggregatorTest.MaxAsync(a => a.DecimalValue)).Should().Be(5, "should work for Decimal with selector");

            (await context.AggregatorTest.Select(a => a.NullableIntValue).MaxAsync()).Should().Be(5, "should work for Int32?");
            (await context.AggregatorTest.MaxAsync(a => a.NullableIntValue)).Should().Be(5, "should work for Int32? with selector");
            (await context.AggregatorTest.Select(a => a.NullableLongValue).MaxAsync()).Should().Be(5, "should work for Int64?");
            (await context.AggregatorTest.MaxAsync(a => a.NullableLongValue)).Should().Be(5, "should work for Int64? with selector");
            (await context.AggregatorTest.Select(a => a.NullableFloatValue).MaxAsync()).Should().Be(5, "should work for Float?");
            (await context.AggregatorTest.MaxAsync(a => a.NullableFloatValue)).Should().Be(5, "should work for Float? with selector");
            (await context.AggregatorTest.Select(a => a.NullableDoubleValue).MaxAsync()).Should().Be(5, "should work for Double?");
            (await context.AggregatorTest.MaxAsync(a => a.NullableDoubleValue)).Should().Be(5, "should work for Double? with selector");
            (await context.AggregatorTest.Select(a => a.NullableDecimalValue).MaxAsync()).Should().Be(5, "should work for Decimal?");
            (await context.AggregatorTest.MaxAsync(a => a.NullableDecimalValue)).Should().Be(5, "should work for Decimal? with selector");
        }
    }
}
