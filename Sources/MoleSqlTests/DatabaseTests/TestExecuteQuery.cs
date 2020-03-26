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
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql;
using MoleSqlTests.TestDb;

namespace MoleSqlTests.DatabaseTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestExecuteQuery : MoleSqlTestBase
    {
        // ReSharper disable once ClassNeverInstantiated.Local
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
            context.Invoking(ctx => ctx.ExecuteQueryAsync<object>($""))
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
            context.Invoking(ctx => ctx.ExecuteQueryAsync($""))
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
            AssertSql(context, $@"
SELECT [Id], [Name] FROM Departments WHERE [Id] IN (@p0, @p1, @p2, @p3) ORDER BY [Id]
-- @p0 Input Int (Size = 0; Prec = 0; Scale = 0) [1]
-- @p1 Input Int (Size = 0; Prec = 0; Scale = 0) [2]
-- @p2 Input Int (Size = 0; Prec = 0; Scale = 0) [3]
-- @p3 Input Int (Size = 0; Prec = 0; Scale = 0) [4]
{context.ContextInfo}
");
        }
        [TestMethod]
        public async Task ExecuteQueryAsync_Generic_CorrectResults()
        {
            using var context = GetDbContext();
            var query = context.ExecuteQueryAsync<TestRow>($"SELECT TOP 4 [Id] AS Identifier, [Name] AS Description, [Name] AS IDontGetMapped FROM Departments ORDER BY [Id]", CancellationToken.None);

            var resultList = await query.Select(element => (element.Identifier, element.Description)).ToListAsync();
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
            Assert.AreEqual(result[0].Name, "René");
            Assert.AreEqual(result[0].DateOfBirth, new DateTime(1979, 5, 3));
            Assert.IsNull(result[0].LastSeen);
            Assert.AreEqual(result[1].Name, "Marc");
            Assert.IsNull(result[1].DateOfBirth);
            Assert.AreEqual(result[1].LastSeen, new DateTime(1954, 4, 1));
        }
        [TestMethod]
        public async Task ExecuteQueryAsync_Dynamic_CorrectResults()
        {
            using var context = GetDbContext();
            var query = context.ExecuteQueryAsync($"SELECT TOP 4 [Id] AS Identifier, [Name] AS Description FROM Departments ORDER BY [Id]", CancellationToken.None);

            var resultList = await query.Cast<dynamic>().Select(element => (element.Identifier, element.Description)).ToListAsync();
            resultList.Should().Equal((1, "Marketing"), (2, "Sales"), (3, "Support"), (4, "Development"));
        }
        [TestMethod]
        public void ExecuteQuery_String_CorrectResults()
        {
            using var context = GetDbContext();
            var query = context.ExecuteQuery<string>($"SELECT TOP 4 [Name] FROM Departments ORDER BY [Id]");

            var resultList = query.ToList();
            resultList.Should().BeOfType<List<string>>();
            resultList.Should().Equal("Marketing", "Sales", "Support", "Development");
        }
        [TestMethod]
        public void ExecuteQuery_Double_CorrectResults()
        {
            using var context = GetDbContext();
            var query = context.ExecuteQuery<double>($"SELECT [DoubleValue] FROM AggregatorTestTable ORDER BY [DoubleValue]");
            var resultList = query.ToList();
            resultList.Should().BeOfType<List<double>>();
            resultList.Should().Equal(-1d, 0d, 1d, 2d, 3d, 4d, 5d);
        }
        [TestMethod]
        public void ExecuteQuery_NullableDouble_CorrectResults()
        {
            using var context = GetDbContext();
            var query = context.ExecuteQuery<double?>($"SELECT [DoubleValue] FROM NullableTestTable ORDER BY [DoubleValue]");
            var resultList = query.ToList();
            resultList.Should().BeOfType<List<double?>>();
            resultList.Should().Equal(null, 0d, 1d, 2d);
        }
        [TestMethod]
        public void ExecuteQuery_MultipleRows_Exception()
        {
            using var context = GetDbContext();
            var query = context.ExecuteQuery<int>($"SELECT [IntValue], [DoubleValue] FROM NullableTestTable ORDER BY [DoubleValue]");
            query.Invoking(q => q.ToList()).Should().Throw<InvalidOperationException>().WithMessage($"*{typeof(int).Name}*");
        }
        [TestMethod]
        public async Task ExecuteQueryAsync_String_CorrectResults()
        {
            using var context = GetDbContext();
            var query = context.ExecuteQueryAsync<string>($"SELECT TOP 4 [Name] FROM Departments ORDER BY [Id]");

            var result = await query.ToListAsync();
            result.Should().Equal("Marketing", "Sales", "Support", "Development");
        }
        [TestMethod]
        public async Task ExecuteQueryAsync_Double_CorrectResults()
        {
            using var context = GetDbContext();
            var query = context.ExecuteQueryAsync<double>($"SELECT [DoubleValue] FROM AggregatorTestTable ORDER BY [DoubleValue]");
            var result = await query.ToListAsync();
            result.Should().Equal(-1d, 0d, 1d, 2d, 3d, 4d, 5d);
        }
        [TestMethod]
        public async Task ExecuteQueryAsync_NullableDouble_CorrectResults()
        {
            using var context = GetDbContext();
            var query = context.ExecuteQueryAsync<double?>($"SELECT [DoubleValue] FROM NullableTestTable ORDER BY [DoubleValue]");
            var result = await query.ToListAsync();
            result.Should().Equal(null, 0d, 1d, 2d);
        }
        [TestMethod]
        public void ExecuteQueryAsync_MultipleRows_Exception()
        {
            using var context = GetDbContext();
            var query = context.ExecuteQueryAsync<int>($"SELECT [IntValue], [DoubleValue] FROM NullableTestTable ORDER BY [DoubleValue]");
            query.Awaiting(async q => await q.GetAsyncEnumerator().MoveNextAsync()).Should().Throw<InvalidOperationException>().WithMessage($"*{typeof(int).Name}*");
        }
        [TestMethod]
        public void ExecuteQuery_NonGeneric_String_CorrectResults()
        {
            using var context = GetDbContext();
            var query = context.ExecuteQuery($"SELECT TOP 4 [Name] FROM Departments ORDER BY [Id]");

            var resultList = query.Cast<string>().ToList();
            resultList.Should().BeOfType<List<string>>();
            resultList.Should().Equal("Marketing", "Sales", "Support", "Development");
        }
        [TestMethod]
        public void ExecuteQuery_NonGeneric_Double_CorrectResults()
        {
            using var context = GetDbContext();
            var query = context.ExecuteQuery($"SELECT [DoubleValue] FROM AggregatorTestTable ORDER BY [DoubleValue]");
            var resultList = query.Cast<double>().ToList();
            resultList.Should().BeOfType<List<double>>();
            resultList.Should().Equal(-1d, 0d, 1d, 2d, 3d, 4d, 5d);
        }
        [TestMethod]
        public void ExecuteQuery_NonGeneric_NullableDouble_CorrectResults()
        {
            using var context = GetDbContext();
            var query = context.ExecuteQuery($"SELECT [DoubleValue] FROM NullableTestTable ORDER BY [DoubleValue]");
            var resultList = query.Cast<double?>().ToList();
            resultList.Should().BeOfType<List<double?>>();
            resultList.Should().Equal(null, 0d, 1d, 2d);
        }
        [TestMethod]
        public async Task ExecuteQueryAsync_NonGeneric_String_CorrectResults()
        {
            using var context = GetDbContext();
            var query = context.ExecuteQueryAsync($"SELECT TOP 4 [Name] FROM Departments ORDER BY [Id]");

            var result = await query.ToListAsync();
            result.Should().Equal("Marketing", "Sales", "Support", "Development");
        }
        [TestMethod]
        public async Task ExecuteQueryAsync_NonGeneric_Double_CorrectResults()
        {
            using var context = GetDbContext();
            var query = context.ExecuteQueryAsync($"SELECT [DoubleValue] FROM AggregatorTestTable ORDER BY [DoubleValue]");
            var result = await query.ToListAsync();
            result.Should().Equal(-1d, 0d, 1d, 2d, 3d, 4d, 5d);
        }
        [TestMethod]
        public async Task ExecuteQueryAsync_NonGeneric_NullableDouble_CorrectResults()
        {
            using var context = GetDbContext();
            var query = context.ExecuteQueryAsync($"SELECT [DoubleValue] FROM NullableTestTable ORDER BY [DoubleValue]");
            var result = await query.ToListAsync();
            result.Should().Equal(null, 0d, 1d, 2d);
        }
    }
}
