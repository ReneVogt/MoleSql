/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MoleSqlTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestExecuteScalar
    {
        [TestMethod]
        public void ExecuteScalar_NonGeneric_CorrectCount()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var result = context.ExecuteScalar($"SELECT COUNT([Id]) FROM (SELECT TOP 4 [Id] FROM Employees) AS X");
            result.Should().Be(4);
        }
        [TestMethod]
        public void ExecuteScalar_Generic_CorrectCount()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var result = context.ExecuteScalar<int>($"SELECT COUNT([Id]) FROM (SELECT TOP 4 [Id] FROM Employees) AS X");
            result.Should().Be(4);
        }
        [TestMethod]
        public async Task ExecuteScalarAsync_NonGeneric_CorrectCount()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var result = await context.ExecuteScalarAsync($"SELECT COUNT([Id]) FROM (SELECT TOP 4 [Id] FROM Employees) AS X");
            result.Should().Be(4);
        }
        [TestMethod]
        public async Task ExecuteScalarAsync_Generic_CorrectCount()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var result = await context.ExecuteScalarAsync<int>($"SELECT COUNT([Id]) FROM (SELECT TOP 4 [Id] FROM Employees) AS X");
            result.Should().Be(4);
        }
    }
}
