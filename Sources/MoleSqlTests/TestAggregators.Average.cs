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
        public void Average_Customers_WithoutSelector()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            // ReSharper disable once ReplaceWithSingleCallToCount
            var result = context.Customers.Where(c => c.Id < 4).Select(c => c.Id).Average();
            result.Should().Be(2);
        }
        [TestMethod]
        public void Average_Customers_WithSelector()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            // ReSharper disable once ReplaceWithSingleCallToCount
            var result = context.Customers.Where(c => c.Id < 4).Average(c => c.Id);
            result.Should().Be(2);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void AverageAsync_NotOnTop_NotSupportedException()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            context.Customers.Select(customer => new { T = context.Customers.Select(c => c.Id).AverageAsync(default) }).AsEnumerable().ToList();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void AverageAsync_NotOnTopSelector_NotSupportedException()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            context.Customers.Select(customer => new { T = context.Customers.AverageAsync(c => c.Id, default) }).AsEnumerable().ToList();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task AverageAsyncInt_WrongQueryProvider_Exception()
        {
            await new[] { 1 }.AsQueryable().AverageAsync();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task AverageAsyncNullableInt_WrongQueryProvider_Exception()
        {
            await new int?[] { 1 }.AsQueryable().AverageAsync();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task AverageAsyncIntPred_WrongQueryProvider_Exception()
        {
            await new[] { 1 }.AsQueryable().AverageAsync(x => x);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task AverageAsyncNullableIntPred_WrongQueryProvider_Exception()
        {
            await new int?[] { 1 }.AsQueryable().AverageAsync(x => x);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task AverageAsyncLong_WrongQueryProvider_Exception()
        {
            await new[] { 1L }.AsQueryable().AverageAsync();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task AverageAsyncNullableLong_WrongQueryProvider_Exception()
        {
            await new long?[] { 1 }.AsQueryable().AverageAsync();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task AverageAsyncLongPred_WrongQueryProvider_Exception()
        {
            await new[] { 1L }.AsQueryable().AverageAsync(x => x);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task AverageAsyncNullableLongPred_WrongQueryProvider_Exception()
        {
            await new long?[] { 1 }.AsQueryable().AverageAsync(x => x);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task AverageAsyncFloat_WrongQueryProvider_Exception()
        {
            await new[] { 1f }.AsQueryable().AverageAsync();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task AverageAsyncNullableFloat_WrongQueryProvider_Exception()
        {
            await new float?[] { 1f }.AsQueryable().AverageAsync();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task AverageAsyncFloatPred_WrongQueryProvider_Exception()
        {
            await new[] { 1f }.AsQueryable().AverageAsync(x => x);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task AverageAsyncNullableFloatPred_WrongQueryProvider_Exception()
        {
            await new float?[] { 1f }.AsQueryable().AverageAsync(x => x);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task AverageAsyncDouble_WrongQueryProvider_Exception()
        {
            await new[] { 1d }.AsQueryable().AverageAsync();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task AverageAsyncNullableDouble_WrongQueryProvider_Exception()
        {
            await new double?[] { 1d }.AsQueryable().AverageAsync();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task AverageAsyncDoublePred_WrongQueryProvider_Exception()
        {
            await new[] { 1d }.AsQueryable().AverageAsync(x => x);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task AverageAsyncNullableDoublePred_WrongQueryProvider_Exception()
        {
            await new double?[] { 1d }.AsQueryable().AverageAsync(x => x);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task AverageAsyncDecimal_WrongQueryProvider_Exception()
        {
            await new[] { 1m }.AsQueryable().AverageAsync();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task AverageAsyncNullableDecimal_WrongQueryProvider_Exception()
        {
            await new decimal?[] { 1m }.AsQueryable().AverageAsync();
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task AverageAsyncDecimalPred_WrongQueryProvider_Exception()
        {
            await new[] { 1m }.AsQueryable().AverageAsync(x => x);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task AverageAsyncNullableDecimalPred_WrongQueryProvider_Exception()
        {
            await new decimal?[] { 1m }.AsQueryable().AverageAsync(x => x);
        }
        [TestMethod]
        public async Task AverageAsync_Customers_WithoutSelector()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            // ReSharper disable once ReplaceWithSingleCallToCount
            var result = await context.Customers.Where(c => c.Id < 4).Select(c => c.Id).AverageAsync();
            result.Should().Be(2);
        }
        [TestMethod]
        public async Task AverageAsync_Customers_WithSelector()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            // ReSharper disable once ReplaceWithSingleCallToCount
            var result = await context.Customers.Where(c => c.Id < 4).AverageAsync(c => c.Id);
            result.Should().Be(2);
        }
    }
}
