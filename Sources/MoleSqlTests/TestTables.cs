/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql.Extensions;
using MoleSqlTests.TestDb;

namespace MoleSqlTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestTables : MoleSqlTestBase
    {
        [TestMethod]
        public void Employees_ExpectedTableProjection()
        {
            using var context = GetDbContext();
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            context.Employees.ToList();
            AssertAndLogSql(context, "SELECT [t0].[Id], [t0].[DepartmentId], [t0].[Name], [t0].[DateOfBirth], [t0].[LastSeen], [t0].[Salary] FROM [Employees] AS t0");
        }
        [TestMethod]
        public async Task EmployeesAsync_ExpectedTableProjection()
        {
            using var context = GetDbContext();
            await ((IAsyncEnumerable<Employees>)context.Employees).ToListAsync();
            AssertAndLogSql(context, "SELECT [t0].[Id], [t0].[DepartmentId], [t0].[Name], [t0].[DateOfBirth], [t0].[LastSeen], [t0].[Salary] FROM [Employees] AS t0");
        }
    }
}
