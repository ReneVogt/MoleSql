﻿/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql;
using MoleSql.Extensions;
using MoleSqlTests.TestDb;

namespace MoleSqlTests.DatabaseTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestExecuteQuery : MoleSqlTestBase
    {
        sealed class TestRow
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public int Identifier { get; set; }
            public string Description { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }

        [TestMethod]
        public void ExecuteQuery_Generic_AfterDispose_ObjectDisposedException()
        {
            var context = GetDbContext();
            context.Dispose();
            context.Invoking(ctx => ctx.ExecuteQuery<object>($""))
                   .Should()
                   .Throw<ObjectDisposedException>()
                   .Where(e => e.ObjectName == nameof(QueryProvider));
        }
        [TestMethod]
        public void ExecuteQueryAsync_Generic_AfterDispose_ObjectDisposedException()
        {
            var context = GetDbContext();
            context.Dispose();
            context.Awaiting(async ctx => await ctx.ExecuteQueryAsync<object>($"").ToListAsync())
                   .Should()
                   .Throw<ObjectDisposedException>()
                   .Where(e => e.ObjectName == nameof(QueryProvider));
        }
        [TestMethod]
        public void ExecuteQuery_Dynamic_AfterDispose_ObjectDisposedException()
        {
            var context = GetDbContext();
            context.Dispose();
            context.Invoking(ctx => ctx.ExecuteQuery($""))
                   .Should()
                   .Throw<ObjectDisposedException>()
                   .Where(e => e.ObjectName == nameof(QueryProvider));
        }
        [TestMethod]
        public void ExecuteQueryAsync_Dynamic_AfterDispose_ObjectDisposedException()
        {
            var context = GetDbContext();
            context.Dispose();
            context.Awaiting(async ctx => await ctx.ExecuteQueryAsync($"").ToListAsync())
                   .Should()
                   .Throw<ObjectDisposedException>()
                   .Where(e => e.ObjectName == nameof(QueryProvider));
        }
        [TestMethod]
        public void ExecuteQuery_Generic_CorrectResults()
        {
            using var context = GetDbContext();
            var query = context.ExecuteQuery<TestRow>($"SELECT TOP 4 [Id] AS Identifier, [Name] AS Description, [Name] AS IDontGetMapped FROM Departments ORDER BY [Id]");
            query.Select(x => (x.Identifier, x.Description)).Should().Equal((1, "Marketing"), (2, "Sales"), (3, "Support"), (4, "Development"));
        }
        [TestMethod]
        public void ExecuteQuery_InClause_CorrectResults()
        {
            using var context = GetDbContext();
            var ids = new[] { 1, 2, 3, 4 };
            var query = context.ExecuteQuery<Departments>($"SELECT [Id], [Name] FROM Departments WHERE [Id] IN {ids:int} ORDER BY [Id]");
            query.Select(department => (department.Id, department.Name)).Should().Equal((1, "Marketing"), (2, "Sales"), (3, "Support"), (4, "Development"));
            AssertSql(context, @"
SELECT [Id], [Name] FROM Departments WHERE [Id] IN (@p0, @p1, @p2, @p3) ORDER BY [Id]
-- @p0 Int Input [1]
-- @p1 Int Input [2]
-- @p2 Int Input [3]
-- @p3 Int Input [4]");
        }
        [TestMethod]
        public async Task ExecuteQueryAsync_Generic_CorrectResults()
        {
            using var context = GetDbContext();
            var query = context.ExecuteQueryAsync<TestRow>($"SELECT TOP 4 [Id] AS Identifier, [Name] AS Description, [Name] AS IDontGetMapped FROM Departments ORDER BY [Id]", CancellationToken.None);

            var resultList = new List<(int, string)>();
            await foreach (var element in query)
                resultList.Add((element.Identifier, element.Description));

            resultList.Should().Equal((1, "Marketing"), (2, "Sales"), (3, "Support"), (4, "Development"));
        }
        [TestMethod]
        public void ExecuteQuery_Dynamic_CorrectResults()
        {
            using var context = GetDbContext();
            var query = context.ExecuteQuery($"SELECT TOP 4 [Id] AS Identifier, [Name] AS Description FROM Departments ORDER BY [Id]");
            query.Cast<dynamic>().Select(x => (x.Identifier, x.Description)).Should().Equal((1, "Marketing"), (2, "Sales"), (3, "Support"), (4, "Development"));
        }
        [TestMethod]
        public void ExecuteQuery_Dynamic_CorrectResultsWithNulls()
        {
            using var context = GetDbContext();
            var query = context.ExecuteQuery($"SELECT TOP 2 [Name], [DateOfBirth], [LastSeen] FROM [Employees] ORDER BY [Id]");
            var result = query.Cast<dynamic>().ToList();
            result.Should().HaveCount(2);
            Assert.AreEqual(result[0].Name, "Marc");
            Assert.AreEqual(result[0].DateOfBirth, new DateTime(1970, 1, 1));
            Assert.IsNull(result[0].LastSeen);
            Assert.AreEqual(result[1].Name, "Marcel");
            Assert.IsNull(result[1].DateOfBirth);
            Assert.AreEqual(result[1].LastSeen, new DateTime(2010, 10, 25));
        }
        [TestMethod]
        public async Task ExecuteQueryAsync_Dynamic_CorrectResults()
        {
            using var context = GetDbContext();
            var query = context.ExecuteQueryAsync($"SELECT TOP 4 [Id] AS Identifier, [Name] AS Description FROM Departments ORDER BY [Id]", CancellationToken.None);

            var resultList = new List<(int, string)>();
            await foreach (var element in query)
                resultList.Add((element.Identifier, element.Description));

            resultList.Should().Equal((1, "Marketing"), (2, "Sales"), (3, "Support"), (4, "Development"));
        }
    }
}
