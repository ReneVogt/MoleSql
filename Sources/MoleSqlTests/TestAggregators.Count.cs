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
        public void Count_Customers_WithoutPredicate()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            // ReSharper disable once ReplaceWithSingleCallToCount
            var result = context.Customers.Where(c => c.Id < 4).Count();
            result.Should().Be(3);
        }
        [TestMethod]
        public void Count_Customers_WithPredicate()
        {
            using var context = MoleSqlTestContext.GetDbContext();
            var result = context.Customers.Count(c => c.Id < 4);
            result.Should().Be(3);
        }
    }
}
