/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MoleSqlTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestExecuteQuery
    {
        sealed class TestRow
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public int Identifier { get; set; }
            public string Description { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }

        [TestMethod]
        public void ExecuteQuery_Generic_CorrectResults()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var query = context.ExecuteQuery<TestRow>($"SELECT TOP 4 [Id] AS Identifier, [Name] AS Description FROM Departments ORDER BY [Id]");
            query.Select(x => (x.Identifier, x.Description)).Should().Equal((1, "Marketing"), (2, "Sales"), (3, "Support"), (4, "Development"));
        }
        [TestMethod]
        public async Task ExecuteQueryAsync_Generic_CorrectResults()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var query = context.ExecuteQueryAsync<TestRow>($"SELECT TOP 4 [Id] AS Identifier, [Name] AS Description FROM Departments ORDER BY [Id]", CancellationToken.None);

            var resultList = new List<(int, string)>();
            await foreach (var element in query)
                resultList.Add((element.Identifier, element.Description));

            resultList.Should().Equal((1, "Marketing"), (2, "Sales"), (3, "Support"), (4, "Development"));
        }
        [TestMethod]
        public void ExecuteQuery_Dynamic_CorrectResults()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var query = context.ExecuteQuery($"SELECT TOP 4 [Id] AS Identifier, [Name] AS Description FROM Departments ORDER BY [Id]");
            query.Cast<dynamic>().Select(x => (x.Identifier, x.Description)).Should().Equal((1, "Marketing"), (2, "Sales"), (3, "Support"), (4, "Development"));
        }
        [TestMethod]
        public async Task ExecuteQueryAsync_Dynamic_CorrectResults()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var query = context.ExecuteQueryAsync($"SELECT TOP 4 [Id] AS Identifier, [Name] AS Description FROM Departments ORDER BY [Id]", CancellationToken.None);

            var resultList = new List<(int, string)>();
            await foreach (var element in query)
                resultList.Add((element.Identifier, element.Description));

            resultList.Should().Equal((1, "Marketing"), (2, "Sales"), (3, "Support"), (4, "Development"));
        }
    }
}
