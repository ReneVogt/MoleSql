/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Unit-Tests for the MoleSql.QueryProvider class.
 *
 */

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql;

namespace MoleSqlTests.UnitTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestQueryProvider : MoleSqlTestBase
    {
        [TestMethod]
        public void QueryProvider_CreateQueryNonGeneric_WrongType_ArgumentOutOfRangeException()
        {
            using var context = GetDbContext();
            var query = context.Employees;
            var provider = (QueryProvider)query.Provider;

            provider.Invoking(p => ((IQueryProvider)p).CreateQuery(Expression.Constant("throw"))).Should().Throw<ArgumentOutOfRangeException>();

            // just for code-coverage: not disposed twice
            provider.Dispose();
            context.Dispose(); 
        }
        [TestMethod]
        public void QueryProvider_CreateQueryNonGeneric_Works()
        {
            using var context = GetDbContext();
            var query = context.Employees;
            var provider = query.Provider;
            var expression = Expression.Constant(query);
            var result = provider.CreateQuery(expression);
            result.Expression.Should().Be(expression);
        }
    }
}
