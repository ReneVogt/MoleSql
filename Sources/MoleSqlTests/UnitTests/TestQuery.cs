/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Unit-Tests for the MoleSql.Query<T> class.
 *
 */

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql;

// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable ObjectCreationAsStatement

namespace MoleSqlTests.UnitTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestQuery : MoleSqlTestBase
    {
        [TestMethod]
        public void Query_ConstructorNoExpression_Constant()
        {
            var query = new Query<int>(null);
            query.Expression.Should().BeOfType<ConstantExpression>();
            query.Expression.NodeType.Should().Be(ExpressionType.Constant);
            ((ConstantExpression)query.Expression).Value.Should().BeSameAs(query);
        }
        [TestMethod]
        public void Query_ConstructorNullExpression_ArgumentNullException()
        {
            ((Action)(() => new Query<int>(null, null))).Should().Throw<ArgumentNullException>();
        }
        [TestMethod]
        public void Query_WronglyTypedExpression_ArgumentOutOfRangeException()
        {
            ((Action)(() => new Query<int>(null, Expression.Constant(12)))).Should().Throw<ArgumentOutOfRangeException>();
        }
        [TestMethod]
        public void Query_WronglyTypedQueryableExpression_ArgumentOutOfRangeException()
        {
            ((Action)(() => new Query<int>(null, Expression.Constant(null, typeof(IQueryable<string>))))).Should().Throw<ArgumentOutOfRangeException>();
        }
        [TestMethod]
        public void Query_Works()
        {
            var expression = Expression.Constant(null, typeof(IQueryable<int>));
            var query = new Query<int>(null, expression);
            query.Expression.Should().Be(expression);
        }
    }
}
