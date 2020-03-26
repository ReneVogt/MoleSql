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

// ReSharper disable InvokeAsExtensionMethod
// ReSharper disable AssignNullToNotNullAttribute

// ReSharper disable AccessToDisposedClosure

// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace MoleSqlTests.DatabaseTests
{
    public partial class TestAggregators
    {
        [TestMethod]
        public void Sum_WithoutSelector_CorrectValues()
        {
            using var context = GetDbContext();
            context.AggregatorTest.Select(c => c.IntValue).Sum().Should().Be(10);
        }
        [TestMethod]
        public void Sum_WithSelector_CorrectValues()
        {
            using var context = GetDbContext();
            context.AggregatorTest.Sum(c => c.IntValue).Should().Be(10);
        }
        [TestMethod]
        public void SumAsync_NotOnTop_NotSupportedException()
        {
            using var context = GetDbContext();
            Action test = () => context.AggregatorTest.Select(c => new { T = context.AggregatorTest.Select(x => x.IntValue).SumAsync(default) }).AsEnumerable().ToList();
            test.Should().Throw<NotSupportedException>().Where(e => e.Message.Contains(nameof(MoleSqlQueryable.SumAsync)));
        }
        [TestMethod]
        public void SumAsync_NotOnTopSelector_NotSupportedException()
        {
            using var context = GetDbContext();
            Action test = () => context.AggregatorTest.Select(x => new { T = context.AggregatorTest.SumAsync(c => c.IntValue, default) }).AsEnumerable().ToList();
            test.Should().Throw<NotSupportedException>().Where(e => e.Message.Contains(nameof(MoleSqlQueryable.SumAsync)));
        }
        [TestMethod]
        public async Task SumAsync_SourceNull_Exception()
        {
            await Task.WhenAll(
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync((IQueryable<int>)null), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync((IQueryable<int?>)null), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync((IQueryable<long>)null), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync((IQueryable<long?>)null), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync((IQueryable<float>)null), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync((IQueryable<float?>)null), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync((IQueryable<double>)null), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync((IQueryable<double?>)null), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync((IQueryable<decimal>)null), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync((IQueryable<decimal?>)null), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync((IQueryable<object>)null, i => (int)i), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync((IQueryable<object>)null, i => (int?)i), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync((IQueryable<object>)null, i => (long)i), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync((IQueryable<object>)null, i => (long?)i), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync((IQueryable<object>)null, i => (float)i), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync((IQueryable<object>)null, i => (float?)i), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync((IQueryable<object>)null, i => (double)i), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync((IQueryable<object>)null, i => (double?)i), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync((IQueryable<object>)null, i => (decimal)i), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync((IQueryable<object>)null, i => (decimal?)i), "source")
            );
        }
        [TestMethod]
        public async Task SumAsync_SelectorNull_Exception()
        {
            using var context = GetDbContext();
            var query = context.AggregatorTest;
            await Task.WhenAll(
                // ReSharper disable once RedundantCast
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync(query, null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync(query, (Expression<Func<AggregatorTestTable, int?>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync(query, (Expression<Func<AggregatorTestTable, long>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync(query, (Expression<Func<AggregatorTestTable, long?>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync(query, (Expression<Func<AggregatorTestTable, float>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync(query, (Expression<Func<AggregatorTestTable, float?>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync(query, (Expression<Func<AggregatorTestTable, double>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync(query, (Expression<Func<AggregatorTestTable, double?>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync(query, (Expression<Func<AggregatorTestTable, decimal>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.SumAsync(query, (Expression<Func<AggregatorTestTable, decimal?>>)null), "selector")
            );
        }
        [TestMethod]
        public async Task SumAsync_WrongProvider_Exception()
        {
            await Task.WhenAll(
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.SumAsync(Enumerable.Empty<int>().AsQueryable()),
                                                 nameof(MoleSqlQueryable.SumAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.SumAsync(Enumerable.Empty<int?>().AsQueryable()),
                                                 nameof(MoleSqlQueryable.SumAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.SumAsync(Enumerable.Empty<long>().AsQueryable()),
                                                 nameof(MoleSqlQueryable.SumAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.SumAsync(Enumerable.Empty<long?>().AsQueryable()),
                                                 nameof(MoleSqlQueryable.SumAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.SumAsync(Enumerable.Empty<float>().AsQueryable()),
                                                 nameof(MoleSqlQueryable.SumAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.SumAsync(Enumerable.Empty<float?>().AsQueryable()),
                                                 nameof(MoleSqlQueryable.SumAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.SumAsync(Enumerable.Empty<double>().AsQueryable()),
                                                 nameof(MoleSqlQueryable.SumAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.SumAsync(Enumerable.Empty<double?>().AsQueryable()),
                                                 nameof(MoleSqlQueryable.SumAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.SumAsync(Enumerable.Empty<decimal>().AsQueryable()),
                                                 nameof(MoleSqlQueryable.SumAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.SumAsync(Enumerable.Empty<decimal?>().AsQueryable()),
                                                 nameof(MoleSqlQueryable.SumAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.SumAsync(Enumerable.Empty<object>().AsQueryable(), o => 0),
                                                 nameof(MoleSqlQueryable.SumAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.SumAsync(Enumerable.Empty<object>().AsQueryable(), o => (int?)0),
                                                 nameof(MoleSqlQueryable.SumAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.SumAsync(Enumerable.Empty<object>().AsQueryable(), o => (long)0),
                                                 nameof(MoleSqlQueryable.SumAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.SumAsync(Enumerable.Empty<object>().AsQueryable(), o => (long?)0),
                                                 nameof(MoleSqlQueryable.SumAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.SumAsync(Enumerable.Empty<object>().AsQueryable(), o => (float)0),
                                                 nameof(MoleSqlQueryable.SumAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.SumAsync(Enumerable.Empty<object>().AsQueryable(), o => (float?)0),
                                                 nameof(MoleSqlQueryable.SumAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.SumAsync(Enumerable.Empty<object>().AsQueryable(), o => (double)0),
                                                 nameof(MoleSqlQueryable.SumAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.SumAsync(Enumerable.Empty<object>().AsQueryable(), o => (double?)0),
                                                 nameof(MoleSqlQueryable.SumAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.SumAsync(Enumerable.Empty<object>().AsQueryable(), o => (decimal)0),
                                                 nameof(MoleSqlQueryable.SumAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.SumAsync(Enumerable.Empty<object>().AsQueryable(), o => (decimal?)0),
                                                 nameof(MoleSqlQueryable.SumAsync))
            );
        }
        [TestMethod]
        public async Task SumAsync_CorrectValues()
        {
            using var context = GetDbContext();
            (await context.AggregatorTest.Select(a => a.IntValue).SumAsync()).Should().Be(10, "should work for Int314");
            (await context.AggregatorTest.SumAsync(a => a.IntValue)).Should().Be(10, "should work for Int314 with selector");
            (await context.AggregatorTest.Select(a => a.LongValue).SumAsync()).Should().Be(10, "should work for Int64");
            (await context.AggregatorTest.SumAsync(a => a.LongValue)).Should().Be(10, "should work for Int64 with selector");
            (await context.AggregatorTest.Select(a => a.FloatValue).SumAsync()).Should().Be(10, "should work for Float");
            (await context.AggregatorTest.SumAsync(a => a.FloatValue)).Should().Be(10, "should work for Float with selector");
            (await context.AggregatorTest.Select(a => a.DoubleValue).SumAsync()).Should().Be(10, "should work for Double");
            (await context.AggregatorTest.SumAsync(a => a.DoubleValue)).Should().Be(10, "should work for Double with selector");
            (await context.AggregatorTest.Select(a => a.DecimalValue).SumAsync()).Should().Be(10, "should work for Decimal");
            (await context.AggregatorTest.SumAsync(a => a.DecimalValue)).Should().Be(10, "should work for Decimal with selector");

            (await context.AggregatorTest.Select(a => a.NullableIntValue).SumAsync()).Should().Be(8, "should work for Int314?");
            (await context.AggregatorTest.SumAsync(a => a.NullableIntValue)).Should().Be(8, "should work for Int314? with selector");
            (await context.AggregatorTest.Select(a => a.NullableLongValue).SumAsync()).Should().Be(8, "should work for Int64?");
            (await context.AggregatorTest.SumAsync(a => a.NullableLongValue)).Should().Be(8, "should work for Int64? with selector");
            (await context.AggregatorTest.Select(a => a.NullableFloatValue).SumAsync()).Should().Be(8, "should work for Float?");
            (await context.AggregatorTest.SumAsync(a => a.NullableFloatValue)).Should().Be(8, "should work for Float? with selector");
            (await context.AggregatorTest.Select(a => a.NullableDoubleValue).SumAsync()).Should().Be(8, "should work for Double?");
            (await context.AggregatorTest.SumAsync(a => a.NullableDoubleValue)).Should().Be(8, "should work for Double? with selector");
            (await context.AggregatorTest.Select(a => a.NullableDecimalValue).SumAsync()).Should().Be(8, "should work for Decimal?");
            (await context.AggregatorTest.SumAsync(a => a.NullableDecimalValue)).Should().Be(8, "should work for Decimal? with selector");
        }
    }
}
