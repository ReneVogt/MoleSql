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

// ReSharper disable AccessToDisposedClosure
// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable InvokeAsExtensionMethod
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
#pragma warning disable CS4014

namespace MoleSqlTests.DatabaseTests
{
    public partial class TestAggregators
    {
        [TestMethod]
        public void Average_WithoutSelector_CorrectValues()
        {
            using var context = GetDbContext();
            context.AggregatorTest.Select(c => c.IntValue).Average().Should().Be(2);
        }
        [TestMethod]
        public void Average_WithSelector_CorrectValues()
        {
            using var context = GetDbContext();
            context.AggregatorTest.Average(c => c.IntValue).Should().Be(2);
        }
        [TestMethod]
        public void AverageAsync_NotOnTop_NotSupportedException()
        {
            using var context = GetDbContext();
            Action test = () => context.AggregatorTest.Select(x => new { T = context.AggregatorTest.Select(c => c.IntValue).AverageAsync(default) }).AsEnumerable().ToList();
            test.Should().Throw<NotSupportedException>().Where(e => e.Message.Contains(nameof(MoleSqlQueryable.AverageAsync)));
        }
        [TestMethod]
        public void AverageAsync_NotOnTopSelector_NotSupportedException()
        {
            using var context = GetDbContext();
            Action test = () => context.AggregatorTest.Select(x => new { T = context.AggregatorTest.AverageAsync(c => c.IntValue, default) }).AsEnumerable().ToList();
            test.Should().Throw<NotSupportedException>().Where(e => e.Message.Contains(nameof(MoleSqlQueryable.AverageAsync)));
        }
        [TestMethod]
        public async Task AverageAsync_SourceNull_Exception()
        {
            await Task.WhenAll(
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync((IQueryable<int>)null), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync((IQueryable<int?>)null), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync((IQueryable<long>)null), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync((IQueryable<long?>)null), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync((IQueryable<float>)null), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync((IQueryable<float?>)null), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync((IQueryable<double>)null), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync((IQueryable<double?>)null), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync((IQueryable<decimal>)null), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync((IQueryable<decimal?>)null), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync((IQueryable<object>)null, i => (int)i), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync((IQueryable<object>)null, i => (int?)i), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync((IQueryable<object>)null, i => (long)i), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync((IQueryable<object>)null, i => (long?)i), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync((IQueryable<object>)null, i => (float)i), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync((IQueryable<object>)null, i => (float?)i), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync((IQueryable<object>)null, i => (double)i), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync((IQueryable<object>)null, i => (double?)i), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync((IQueryable<object>)null, i => (decimal)i), "source"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync((IQueryable<object>)null, i => (decimal?)i), "source")
            );
        }
        [TestMethod]
        public async Task AverageAsync_SelectorNull_Exception()
        {
            using var context = GetDbContext();
            var query = context.AggregatorTest;
            await Task.WhenAll(
                // ReSharper disable once RedundantCast
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync(query, null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync(query, (Expression<Func<AggregatorTestTable, int?>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync(query, (Expression<Func<AggregatorTestTable, long>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync(query, (Expression<Func<AggregatorTestTable, long?>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync(query, (Expression<Func<AggregatorTestTable, float>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync(query, (Expression<Func<AggregatorTestTable, float?>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync(query, (Expression<Func<AggregatorTestTable, double>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync(query, (Expression<Func<AggregatorTestTable, double?>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync(query, (Expression<Func<AggregatorTestTable, decimal>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync(query, (Expression<Func<AggregatorTestTable, decimal?>>)null), "selector")
            );
        }
        [TestMethod]
        public async Task AverageAsync_WrongProvider_Exception()
        {
            await Task.WhenAll(
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.AverageAsync(Enumerable.Empty<int>().AsQueryable()),
                                                 nameof(MoleSqlQueryable.AverageAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.AverageAsync(Enumerable.Empty<int?>().AsQueryable()),
                                                 nameof(MoleSqlQueryable.AverageAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.AverageAsync(Enumerable.Empty<long>().AsQueryable()),
                                                 nameof(MoleSqlQueryable.AverageAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.AverageAsync(Enumerable.Empty<long?>().AsQueryable()),
                                                 nameof(MoleSqlQueryable.AverageAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.AverageAsync(Enumerable.Empty<float>().AsQueryable()),
                                                 nameof(MoleSqlQueryable.AverageAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.AverageAsync(Enumerable.Empty<float?>().AsQueryable()),
                                                 nameof(MoleSqlQueryable.AverageAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.AverageAsync(Enumerable.Empty<double>().AsQueryable()),
                                                 nameof(MoleSqlQueryable.AverageAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.AverageAsync(Enumerable.Empty<double?>().AsQueryable()),
                                                 nameof(MoleSqlQueryable.AverageAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.AverageAsync(Enumerable.Empty<decimal>().AsQueryable()),
                                                 nameof(MoleSqlQueryable.AverageAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.AverageAsync(Enumerable.Empty<decimal?>().AsQueryable()),
                                                 nameof(MoleSqlQueryable.AverageAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.AverageAsync(Enumerable.Empty<object>().AsQueryable(), o => 0),
                                                 nameof(MoleSqlQueryable.AverageAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.AverageAsync(Enumerable.Empty<object>().AsQueryable(), o => (int?)0),
                                                 nameof(MoleSqlQueryable.AverageAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.AverageAsync(Enumerable.Empty<object>().AsQueryable(), o => (long)0),
                                                 nameof(MoleSqlQueryable.AverageAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.AverageAsync(Enumerable.Empty<object>().AsQueryable(), o => (long?)0),
                                                 nameof(MoleSqlQueryable.AverageAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.AverageAsync(Enumerable.Empty<object>().AsQueryable(), o => (float)0),
                                                 nameof(MoleSqlQueryable.AverageAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.AverageAsync(Enumerable.Empty<object>().AsQueryable(), o => (float?)0),
                                                 nameof(MoleSqlQueryable.AverageAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.AverageAsync(Enumerable.Empty<object>().AsQueryable(), o => (double)0),
                                                 nameof(MoleSqlQueryable.AverageAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.AverageAsync(Enumerable.Empty<object>().AsQueryable(), o => (double?)0),
                                                 nameof(MoleSqlQueryable.AverageAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.AverageAsync(Enumerable.Empty<object>().AsQueryable(), o => (decimal)0),
                                                 nameof(MoleSqlQueryable.AverageAsync)),
                ShouldThrowNotSupportedException(() => MoleSqlQueryable.AverageAsync(Enumerable.Empty<object>().AsQueryable(), o => (decimal?)0),
                                                 nameof(MoleSqlQueryable.AverageAsync))
            );
        }
        [TestMethod]
        public async Task AverageAsync_CorrectValues()
        {
            using var context = GetDbContext();
            (await context.AggregatorTest.Select(a => a.IntValue).AverageAsync()).Should().Be(2, "should work for Int32");
            (await context.AggregatorTest.AverageAsync(a => a.IntValue)).Should().Be(2, "should work for Int32 with selector");
            (await context.AggregatorTest.Select(a => a.LongValue).AverageAsync()).Should().Be(2, "should work for Int64");
            (await context.AggregatorTest.AverageAsync(a => a.LongValue)).Should().Be(2, "should work for Int64 with selector");
            (await context.AggregatorTest.Select(a => a.FloatValue).AverageAsync()).Should().Be(2, "should work for Float");
            (await context.AggregatorTest.AverageAsync(a => a.FloatValue)).Should().Be(2, "should work for Float with selector");
            (await context.AggregatorTest.Select(a => a.DoubleValue).AverageAsync()).Should().Be(2, "should work for Double");
            (await context.AggregatorTest.AverageAsync(a => a.DoubleValue)).Should().Be(2, "should work for Double with selector");
            (await context.AggregatorTest.Select(a => a.DecimalValue).AverageAsync()).Should().Be(2, "should work for Decimal");
            (await context.AggregatorTest.AverageAsync(a => a.DecimalValue)).Should().Be(2, "should work for Decimal with selector");

            (await context.AggregatorTest.Select(a => a.NullableIntValue).AverageAsync()).Should().Be(2, "should work for Int32?");
            (await context.AggregatorTest.AverageAsync(a => a.NullableIntValue)).Should().Be(2, "should work for Int32? with selector");
            (await context.AggregatorTest.Select(a => a.NullableLongValue).AverageAsync()).Should().Be(2, "should work for Int64?");
            (await context.AggregatorTest.AverageAsync(a => a.NullableLongValue)).Should().Be(2, "should work for Int64? with selector");
            (await context.AggregatorTest.Select(a => a.NullableFloatValue).AverageAsync()).Should().Be(2, "should work for Float?");
            (await context.AggregatorTest.AverageAsync(a => a.NullableFloatValue)).Should().Be(2, "should work for Float? with selector");
            (await context.AggregatorTest.Select(a => a.NullableDoubleValue).AverageAsync()).Should().Be(2, "should work for Double?");
            (await context.AggregatorTest.AverageAsync(a => a.NullableDoubleValue)).Should().Be(2, "should work for Double? with selector");
            (await context.AggregatorTest.Select(a => a.NullableDecimalValue).AverageAsync()).Should().Be(2, "should work for Decimal?");
            (await context.AggregatorTest.AverageAsync(a => a.NullableDecimalValue)).Should().Be(2, "should work for Decimal? with selector");
        }
    }
}
