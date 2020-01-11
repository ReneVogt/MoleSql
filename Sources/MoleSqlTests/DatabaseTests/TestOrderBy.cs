/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MoleSqlTests.DatabaseTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestOrderBy : MoleSqlTestBase
    {
        [TestMethod]
        public void SimpleOrderBy()
        {
            using var context = GetDbContext();
            var query = from department in context.Departments
                        where department.Id < 7
                        orderby department.Name
                        select department.Name;
            var result = query.ToList();
            result.Should().Equal("Development", "DoubleName", "DoubleName", "Marketing", "Sales", "Support");
            AssertSql(context, @"
SELECT [t0].[Name] 
FROM [Departments] AS t0 
WHERE ([t0].[Id] < @p0) 
ORDER BY [t0].[Name] 
-- @p0 Int Input [7]");
        }
        [TestMethod]
        public void SimpleOrderByDescending()
        {
            using var context = GetDbContext();
            var query = from department in context.Departments
                        where department.Id < 7
                        orderby department.Name descending
                        select department.Name;
            var result = query.ToList();
            result.Should().Equal("Support", "Sales", "Marketing", "DoubleName", "DoubleName", "Development");
            AssertSql(context, @"
SELECT [t0].[Name] 
FROM [Departments] AS t0 
WHERE ([t0].[Id] < @p0) 
ORDER BY [t0].[Name] DESC 
-- @p0 Int Input [7]");
        }
        [TestMethod]
        public void MixedOrderByDescending()
        {
            using var context = GetDbContext();
            var query = from department in context.Departments
                        where department.Id < 7
                        orderby department.Name descending, department.Id
                        select new {department.Id, department.Name};
            var result = query.ToList();
            result.Should().HaveCount(6);
            result[0].Id.Should().Be(3);
            result[0].Name.Should().Be("Support");
            result[1].Id.Should().Be(2);
            result[1].Name.Should().Be("Sales");
            result[2].Id.Should().Be(1);
            result[2].Name.Should().Be("Marketing");
            result[3].Id.Should().Be(5);
            result[3].Name.Should().Be("DoubleName");
            result[4].Id.Should().Be(6);
            result[4].Name.Should().Be("DoubleName");
            result[5].Id.Should().Be(4);
            result[5].Name.Should().Be("Development");
            AssertSql(context, @"
SELECT [t0].[Id], [t0].[Name] 
FROM [Departments] AS t0 
WHERE ([t0].[Id] < @p0) 
ORDER BY [t0].[Name] DESC, [t0].[Id] 
-- @p0 Int Input [7]");
        }
    }
}
