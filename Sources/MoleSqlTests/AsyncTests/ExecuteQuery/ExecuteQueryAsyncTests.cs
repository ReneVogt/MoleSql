using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MoleSqlTests.AsyncTests.ExecuteQuery
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ExecuteQueryAsyncTests
    {
        [TestMethod]
        public async Task ExecuteQueryAsync_Dynamic_CorrectResults()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var query = await context.ExecuteQueryAsync($"SELECT TOP 3 [Name], [Age] FROM Customers ORDER BY [Name]", CancellationToken.None);

            var resultList = new List<(string name, int age)>();
            await foreach (var element in query)
                resultList.Add((element.Name, element.Age));

            var expected = new[] {("Alfred", 20), ("Beate", 30), ("Christian", 40)};
            resultList.Should().Equal(expected);
        }
    }
}
