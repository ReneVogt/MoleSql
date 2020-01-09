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

namespace MoleSqlTests
{
    public partial class TestAggregators : MoleSqlTestBase
    {
        [TestMethod]
        public void Average_Customers_WithoutSelector()
        {
            using var context = GetDbContext();
            // ReSharper disable once ReplaceWithSingleCallToCount
            var result = context.Customers.Where(c => c.Id < 4).Select(c => c.Id).Average();
            result.Should().Be(2);
        }
        [TestMethod]
        public void Average_Customers_WithSelector()
        {
            using var context = GetDbContext();
            // ReSharper disable once ReplaceWithSingleCallToCount
            var result = context.Customers.Where(c => c.Id < 4).Average(c => c.Id);
            result.Should().Be(2);
        }
        [TestMethod]
        public void AverageAsync_NotOnTop_NotSupportedException()
        {
            using var context = GetDbContext();
            Action test = () => context.Customers.Select(customer => new { T = context.Customers.Select(c => c.Id).AverageAsync(default) }).AsEnumerable().ToList();
            test.Should().Throw<NotSupportedException>().Where(e => e.Message.Contains(nameof(MoleSqlQueryable.AverageAsync)));
        }
        [TestMethod]
        public void AverageAsync_NotOnTopSelector_NotSupportedException()
        {
            using var context = GetDbContext();
            Action test = () => context.Customers.Select(customer => new { T = context.Customers.AverageAsync(c => c.Id, default) }).AsEnumerable().ToList();
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
            var query = context.Employees;
            await Task.WhenAll(
                // ReSharper disable once RedundantCast
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync(query, null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync(query, (Expression<Func<Employees, int?>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync(query, (Expression<Func<Employees, long>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync(query, (Expression<Func<Employees, long?>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync(query, (Expression<Func<Employees, float>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync(query, (Expression<Func<Employees, float?>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync(query, (Expression<Func<Employees, double>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync(query, (Expression<Func<Employees, double?>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync(query, (Expression<Func<Employees, decimal>>)null), "selector"),
                ShouldThrowArgumentNullException(() => MoleSqlQueryable.AverageAsync(query, (Expression<Func<Employees, decimal?>>)null), "selector")
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
        public async Task AverageAsync_Customers_WithoutSelector()
        {
            using var context = GetDbContext();
            // ReSharper disable once ReplaceWithSingleCallToCount
            var result = await context.Customers.Where(c => c.Id < 4).Select(c => c.Id).AverageAsync();
            result.Should().Be(2);
        }
        [TestMethod]
        public async Task AverageAsync_Customers_WithSelector()
        {
            using var context = GetDbContext();
            // ReSharper disable once ReplaceWithSingleCallToCount
            var result = await context.Customers.Where(c => c.Id < 4).AverageAsync(c => c.Id);
            result.Should().Be(2);
        }
    }
}
