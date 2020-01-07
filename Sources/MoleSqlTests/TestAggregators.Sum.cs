/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace MoleSqlTests
{
    public partial class TestAggregators
    {
        [TestMethod]
        public void Sum_SumIds_WithoutSelector()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var result = context.Employees.Where(e => e.Id < 5 && e.Id > 1).Select(e => e.Id).Sum();
            result.Should().Be(9);
        }
        [TestMethod]
        public void Sum_SumIds_WithSelector()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var result = context.Employees.Where(e => e.Id < 5 && e.Id > 1).Sum(e => e.Id);
            result.Should().Be(9);
        }
    }
}
