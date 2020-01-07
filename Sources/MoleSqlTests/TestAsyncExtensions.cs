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
using MoleSql.Extensions;

namespace MoleSqlTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestAsyncExtensions
    {
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task ToListAsync_WrongQueryProvider_Exception()
        {
            await new object[0].AsQueryable().ToListAsync();
        }
        [TestMethod]
        public async Task ToListAsync_CorrectList()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var list = await context.Employees.OrderBy(e => e.Name).Select(e => e.Name).ToListAsync();
            list.Should().Equal("Marc", "Marcel", "René", "Steve");
        }
        [TestMethod]
        public async Task AsAsyncEnumerable_CorrectList()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var query = context.Employees.OrderBy(e => e.Name).Select(e => e.Name).AsAsyncEnumerable();
            List<string> results = new List<string>();
            await foreach (var name in query)
                results.Add(name);
            results.Should().Equal("Marc", "Marcel", "René", "Steve");
        }
    }
}
