/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql;
using MoleSql.Extensions;

namespace MoleSqlTests.DatabaseTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestAsyncExtensions : MoleSqlTestBase
    {
        [TestMethod]
        public void ToListAsync_NoSource_Exception()
        {
            IQueryable<int> query = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            query.Awaiting(q => q.ToListAsync()).Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "source");
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task ToListAsync_WrongQueryProvider_Exception()
        {
            await new object[0].AsQueryable().ToListAsync();
        }
        [TestMethod]
        public async Task ToListAsync_CorrectList()
        {
            using var context = GetDbContext();
            var list = await context.Employees.OrderBy(e => e.Name).Select(e => e.Name).ToListAsync();
            list.Should().Equal("Marc", "Marcel", "René", "Steve");
        }
        [TestMethod]
        public void AsAsyncEnumerable_NoSource_Exception()
        {
            IQueryable<int> query = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            query.Invoking(q => q.AsAsyncEnumerable()).Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "source");
        }
        [TestMethod]
        public void AsAsyncEnumerable_WrongProvider_Exception()
        {
            IQueryable<int> query = new[]{1,2,3}.AsQueryable();
            query.Invoking(q => q.AsAsyncEnumerable()).Should().Throw<NotSupportedException>().WithMessage($"*{typeof(QueryProvider).FullName}*");
        }
        [TestMethod]
        public async Task AsAsyncEnumerable_CorrectList()
        {
            using var context = GetDbContext();
            var query = context.Employees.OrderBy(e => e.Name).Select(e => e.Name).AsAsyncEnumerable();
            List<string> results = new List<string>();
            await foreach (var name in query)
                results.Add(name);
            results.Should().Equal("Marc", "Marcel", "René", "Steve");
        }
    }
}
