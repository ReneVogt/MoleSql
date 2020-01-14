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
using MoleSql.Mapper;
using MoleSqlTests.TestDb;
// ReSharper disable PossibleMultipleEnumeration

namespace MoleSqlTests.DatabaseTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestTablesAndBasics : MoleSqlTestBase
    {
        [TestMethod]
        public void Table_AggregatorTests_ExpectedTableProjectionAndValues()
        {
            using var context = GetDbContext();
            context.Log.Should().NotBeNull();
            var result = context.AggregatorTest.ToList();
            result.Should().HaveCount(7);
            result.Sort((a,b) => a.DecimalValue.CompareTo(b.DecimalValue));
            for (int i = 0, n = result.Count; i < n; i++)
            {
                int d = i - 1;
                result[i].IntValue.Should().Be(d);
                result[i].LongValue.Should().Be(d);
                result[i].FloatValue.Should().Be(d);
                result[i].DoubleValue.Should().Be(d);
                result[i].DecimalValue.Should().Be(d);
                result[i].NullableIntValue.Should().Be(d);
                result[i].NullableLongValue.Should().Be(d);
                result[i].NullableFloatValue.Should().Be(d);
                result[i].NullableDoubleValue.Should().Be(d);
                result[i].NullableDecimalValue.Should().Be(d);
            }
            AssertSql(context, @"
SELECT [t0].[IntValue], [t0].[LongValue], [t0].[FloatValue], [t0].[DoubleValue], [t0].[DecimalValue], [t0].[NullableIntValue], [t0].[NullableLongValue], [t0].[NullableFloatValue], [t0].[NullableDoubleValue], [t0].[NullableDecimalValue] 
FROM [AggregatorTestTable] AS t0");
        }
        [TestMethod]
        public async Task Table_AggregatorTestsAsync_ExpectedTableProjectionAndValues()
        {
            using var context = GetDbContext();
            context.Log.Should().NotBeNull();
            var result = await context.AggregatorTest.ToListAsync();
            result.Should().HaveCount(7);
            result.Sort((a, b) => a.DecimalValue.CompareTo(b.DecimalValue));
            for (int i = 0, n = result.Count; i < n; i++)
            {
                int d = i - 1;
                result[i].IntValue.Should().Be(d);
                result[i].LongValue.Should().Be(d);
                result[i].FloatValue.Should().Be(d);
                result[i].DoubleValue.Should().Be(d);
                result[i].DecimalValue.Should().Be(d);
                result[i].NullableIntValue.Should().Be(d);
                result[i].NullableLongValue.Should().Be(d);
                result[i].NullableFloatValue.Should().Be(d);
                result[i].NullableDoubleValue.Should().Be(d);
                result[i].NullableDecimalValue.Should().Be(d);
            }
            AssertSql(context, @"
SELECT [t0].[IntValue], [t0].[LongValue], [t0].[FloatValue], [t0].[DoubleValue], [t0].[DecimalValue], [t0].[NullableIntValue], [t0].[NullableLongValue], [t0].[NullableFloatValue], [t0].[NullableDoubleValue], [t0].[NullableDecimalValue] 
FROM [AggregatorTestTable] AS t0");
        }
        [TestMethod]
        public void Table_NullableTests_ExpectedTableProjectionAndValues()
        {
            using var context = GetDbContext();
            context.Log.Should().NotBeNull();
            var result = context.NullableTest.ToList();
            result.Should().HaveCount(4);
            result.Sort((a, b) => a.DecimalValue == null ? b.DecimalValue == null ? 0 : -1 :
                                  b.DecimalValue == null ? 1 : a.DecimalValue.Value.CompareTo(b.DecimalValue.Value));
            result[0].IntValue.Should().BeNull();
            result[0].LongValue.Should().BeNull();
            result[0].FloatValue.Should().BeNull();
            result[0].DoubleValue.Should().BeNull();
            result[0].DecimalValue.Should().BeNull();

            for (int i = 1, n = result.Count; i < n; i++)
            {
                int d = i - 1;
                result[i].IntValue.Should().Be(d);
                result[i].LongValue.Should().Be(d);
                result[i].FloatValue.Should().Be(d);
                result[i].DoubleValue.Should().Be(d);
                result[i].DecimalValue.Should().Be(d);
            }
            AssertSql(context, @"
SELECT [t0].[IntValue], [t0].[LongValue], [t0].[FloatValue], [t0].[DoubleValue], [t0].[DecimalValue]
FROM [NullableTestTable] AS t0
");
        }
        [TestMethod]
        public async Task Table_NullableTestsAsync_ExpectedTableProjectionAndValues()
        {
            using var context = GetDbContext();
            context.Log.Should().NotBeNull();
            var result = await context.NullableTest.ToListAsync();
            result.Should().HaveCount(4);
            result.Sort((a, b) => a.DecimalValue == null ? b.DecimalValue == null ? 0 : -1 :
                                  b.DecimalValue == null ? 1 : a.DecimalValue.Value.CompareTo(b.DecimalValue.Value));
            result[0].IntValue.Should().BeNull();
            result[0].LongValue.Should().BeNull();
            result[0].FloatValue.Should().BeNull();
            result[0].DoubleValue.Should().BeNull();
            result[0].DecimalValue.Should().BeNull();

            for (int i = 1, n = result.Count; i < n; i++)
            {
                int d = i - 1;
                result[i].IntValue.Should().Be(d);
                result[i].LongValue.Should().Be(d);
                result[i].FloatValue.Should().Be(d);
                result[i].DoubleValue.Should().Be(d);
                result[i].DecimalValue.Should().Be(d);
            }
            AssertSql(context, @"
SELECT [t0].[IntValue], [t0].[LongValue], [t0].[FloatValue], [t0].[DoubleValue], [t0].[DecimalValue]
FROM [NullableTestTable] AS t0
");
        }
        [TestMethod]
        public void Table_Employees_ExpectedTableProjection()
        {
            using var context = GetDbContext();
            context.Log.Should().NotBeNull();
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            context.Employees.ToList();
            AssertSql(context, "SELECT [t0].[Id], [t0].[DepartmentId], [t0].[Name], [t0].[DateOfBirth], [t0].[LastSeen], [t0].[Salary] FROM [Employees] AS t0");
        }
        [TestMethod]
        public async Task Table_EmployeesAsync_ExpectedTableProjection()
        {
            using var context = GetDbContext();
            await context.Employees.ToListAsync();
            AssertSql(context, "SELECT [t0].[Id], [t0].[DepartmentId], [t0].[Name], [t0].[DateOfBirth], [t0].[LastSeen], [t0].[Salary] FROM [Employees] AS t0");
        }
        [TestMethod]
        public void Table_EmployeesAfterDisposed_ObjectDisposedException()
        {
            var context = GetDbContext();
            context.Dispose();
            context.Invoking(ctx => ctx.Employees)
                   .Should()
                   .Throw<ObjectDisposedException>()
                   .Where(e => e.ObjectName == nameof(DataContext))
                   .WithMessage($"*{nameof(DataContext)}*");
        }
        [TestMethod]
        public void Table_QueryEmployeesAfterDisposed_ObjectDisposedException()
        {
            var context = GetDbContext();
            var table = context.Employees;
            string typename = table.Provider.GetType().Name;
            context.Dispose();
            table.Invoking(t => t.ToList())
                 .Should()
                 .Throw<ObjectDisposedException>()
                 .Where(e => e.ObjectName == typename)
                 .WithMessage($"*{typename}*");

        }
        [TestMethod]
        public void Basics_DontUseQueryTwice()
        {
            using var context = GetDbContext();
            var query = context.Employees;
            var enumerable = (IEnumerable<Employees>)query.Provider.Execute(query.Expression);
            enumerable.Count().Should().BeGreaterThan(0);
            enumerable.Invoking(e => e.ToList())
                      .Should()
                      .Throw<ObjectDisposedException>()
                      .Where(e => e.ObjectName == nameof(ProjectionReader<Employees>));
        }
        [TestMethod]
        public void Basics_DontUseQueryTwice_Async()
        {
            using var context = GetDbContext();
            var query = context.Employees;
            var enumerable = (IAsyncEnumerable<Employees>)query.Provider.Execute(query.Expression);
            enumerable.GetAsyncEnumerator().Should().NotBeNull();
            enumerable.Invoking(e => e.GetAsyncEnumerator())
                      .Should()
                      .Throw<ObjectDisposedException>()
                      .Where(e => e.ObjectName == nameof(ProjectionReader<Employees>));
        }
    }
}
