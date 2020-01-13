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
            var enumerable = (IEnumerable<Employees>)query.Provider.Execute(query.Expression);
            enumerable.ToList().Should().HaveCountGreaterThan(0);
            enumerable.Invoking(e => e.ToList())
                      .Should()
                      .Throw<ObjectDisposedException>()
                      .Where(e => e.ObjectName == nameof(ProjectionReader<Employees>));
        }
    }
}
