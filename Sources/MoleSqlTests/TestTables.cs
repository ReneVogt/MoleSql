/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MoleSqlTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestTables
    {
        [TestMethod]
        public void Employees_ExpectedTableProjection()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            context.Employees.ToList();
            MoleSqlTestContext.AssertSqlDump(context, "SELECT [t0].[Id], [t0].[DepartmentId], [t0].[Name], [t0].[DateOfBirth], [t0].[LastSeen], [t0].[Salary] FROM [Employees]");
        }
    }
}
