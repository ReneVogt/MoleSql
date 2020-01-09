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
        public void Sum_SumIds_WithoutSelector()
        {
            using var context = GetDbContext();
            var result = context.Employees.Where(e => e.Id < 5 && e.Id > 1).Select(e => e.Id).Sum();
            result.Should().Be(9);
        }
        [TestMethod]
        public void Sum_SumIds_WithSelector()
        {
            using var context = GetDbContext();
            var result = context.Employees.Where(e => e.Id < 5 && e.Id > 1).Sum(e => e.Id);
            result.Should().Be(9);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void SumAsync_NotOnTop_NotSupportedException()
        {
            using var context = GetDbContext();
            context.Customers.Select(customer => new { T = context.Customers.Select(c => c.Id).SumAsync(default) }).AsEnumerable().ToList();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void SumAsync_NotOnTopSelector_NotSupportedException()
        {
            using var context = GetDbContext();
            context.Customers.Select(customer => new { T = context.Customers.SumAsync(c => c.Id, default) }).AsEnumerable().ToList();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task SumAsyncInt_WrongQueryProvider_Exception()
        {
            await new[] { 1 }.AsQueryable().SumAsync();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task SumAsyncNullableInt_WrongQueryProvider_Exception()
        {
            await new int?[] { 1 }.AsQueryable().SumAsync();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task SumAsyncIntPred_WrongQueryProvider_Exception()
        {
            await new[] { 1 }.AsQueryable().SumAsync(x => x);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task SumAsyncNullableIntPred_WrongQueryProvider_Exception()
        {
            await new int?[] { 1 }.AsQueryable().SumAsync(x => x);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task SumAsyncLong_WrongQueryProvider_Exception()
        {
            await new[] { 1L }.AsQueryable().SumAsync();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task SumAsyncNullableLong_WrongQueryProvider_Exception()
        {
            await new long?[] { 1 }.AsQueryable().SumAsync();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task SumAsyncLongPred_WrongQueryProvider_Exception()
        {
            await new[] { 1L }.AsQueryable().SumAsync(x => x);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task SumAsyncNullableLongPred_WrongQueryProvider_Exception()
        {
            await new long?[] { 1 }.AsQueryable().SumAsync(x => x);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task SumAsyncFloat_WrongQueryProvider_Exception()
        {
            await new[] { 1f }.AsQueryable().SumAsync();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task SumAsyncNullableFloat_WrongQueryProvider_Exception()
        {
            await new float?[] { 1f }.AsQueryable().SumAsync();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task SumAsyncFloatPred_WrongQueryProvider_Exception()
        {
            await new[] { 1f }.AsQueryable().SumAsync(x => x);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task SumAsyncNullableFloatPred_WrongQueryProvider_Exception()
        {
            await new float?[] { 1f }.AsQueryable().SumAsync(x => x);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task SumAsyncDouble_WrongQueryProvider_Exception()
        {
            await new[] { 1d }.AsQueryable().SumAsync();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task SumAsyncNullableDouble_WrongQueryProvider_Exception()
        {
            await new double?[] { 1d }.AsQueryable().SumAsync();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task SumAsyncDoublePred_WrongQueryProvider_Exception()
        {
            await new[] { 1d }.AsQueryable().SumAsync(x => x);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task SumAsyncNullableDoublePred_WrongQueryProvider_Exception()
        {
            await new double?[] { 1d }.AsQueryable().SumAsync(x => x);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task SumAsyncDecimal_WrongQueryProvider_Exception()
        {
            await new[] { 1m }.AsQueryable().SumAsync();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task SumAsyncNullableDecimal_WrongQueryProvider_Exception()
        {
            await new decimal?[] { 1m }.AsQueryable().SumAsync();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task SumAsyncDecimalPred_WrongQueryProvider_Exception()
        {
            await new[] { 1m }.AsQueryable().SumAsync(x => x);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task SumAsyncNullableDecimalPred_WrongQueryProvider_Exception()
        {
            await new decimal?[] { 1m }.AsQueryable().SumAsync(x => x);
        }
        [TestMethod]
        public async Task SumAsync_SumIds_WithoutSelector()
        {
            using var context = GetDbContext();
            var result = await context.Employees.Where(e => e.Id < 5 && e.Id > 1).Select(e => e.Id).SumAsync();
            result.Should().Be(9);
        }
        [TestMethod]
        public async Task SumAsync_SumIds_WithSelector()
        {
            using var context = GetDbContext();
            var result = await context.Employees.Where(e => e.Id < 5 && e.Id > 1).SumAsync(e => e.Id);
            result.Should().Be(9);
        }
    }
}
