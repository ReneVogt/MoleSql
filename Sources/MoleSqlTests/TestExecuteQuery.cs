/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        [TestMethod]
        public async Task ExecuteQueryAsync_Dynamic_CorrectResults()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var query = context.ExecuteQueryAsync($"SELECT TOP 4 [Id] AS Erkennungszahl, [Name] AS Bezeichnung FROM Departments ORDER BY [Id]", CancellationToken.None);

            var resultList = new List<(int, string)>();
            await foreach (var element in query)
                resultList.Add((element.Erkennungszahl, element.Bezeichnung));

            resultList.Should().Equal((1, "Marketing"), (2, "Sales"), (3, "Support"), (4, "Development"));
        }
    }
}
