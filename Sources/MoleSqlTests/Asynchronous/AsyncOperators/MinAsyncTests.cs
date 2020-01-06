/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql.QueryProviders;

// ReSharper disable AccessToDisposedClosure
// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace MoleSqlTests.Asynchronous.AsyncOperators
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class MinAsyncTests
    {
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void MinAsync_NotOnTop_NotSupportedException()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            context.Customers.Select(customer => new {T = context.Customers.MinAsync(c => c.Id, default)}).AsEnumerable().ToList();
        }
        [TestMethod]
        public async Task MinAsync_MaxCustomerAge_WithoutSelector()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var result = await context.Customers.Select(customer => customer.Id).MinAsync();
            result.Should().Be(1);
        }
        [TestMethod]
        public async Task MinAsync_MaxCustomerAge_WithSelector()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var result = await context.Customers.MinAsync(c => c.Age);
            result.Should().Be(1);
        }
    }
}
