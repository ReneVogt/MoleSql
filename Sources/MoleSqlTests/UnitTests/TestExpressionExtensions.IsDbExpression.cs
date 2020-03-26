using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql.Expressions;

namespace MoleSqlTests.UnitTests
{
    partial class TestExpressionExtensions
    {
        [TestMethod]
        public void IsDbExpression_Null_False()
        {
            ((Expression)null).IsDbExpression().Should().BeFalse();
        }
        [TestMethod]
        public void IsDbExpression_NonDbExpression_False()
        {

            Expression.Constant(this).IsDbExpression().Should().BeFalse();
        }
        [TestMethod]
        public void IsDbExpression_TableExpression_True()
        {
            Expression e = new TableExpression(GetType(), "alias", "name");
            e.IsDbExpression().Should().BeTrue();
        }
        [TestMethod]
        public void IsDbExpression_ColumnExpression_True()
        {
            Expression e = new ColumnExpression(GetType(), "alias", "name");
            e.IsDbExpression().Should().BeTrue();
        }
        [TestMethod]
        public void IsDbExpression_SelectExpression_True()
        {
            Expression e = new SelectExpression(GetType(), "alias", Enumerable.Empty<ColumnDeclaration>(),
                                                Expression.Empty(), null);
            e.IsDbExpression().Should().BeTrue();
        }
        [TestMethod]
        public void IsDbExpression_ProjectionExpression_True()
        {
            Expression e = new ProjectionExpression(new SelectExpression(GetType(), "alias", Enumerable.Empty<ColumnDeclaration>(),
                                                                         Expression.Empty(), null), Expression.Empty());
            e.IsDbExpression().Should().BeTrue();
        }
        [TestMethod]
        public void IsDbExpression_JoinExpression_True()
        {
            Expression e = new JoinExpression(GetType(), JoinType.CrossApply, Expression.Empty(), Expression.Empty(), null);
            e.IsDbExpression().Should().BeTrue();
        }
        [TestMethod]
        public void IsDbExpression_AggregateExpression_True()
        {
            Expression e = new AggregateExpression(GetType(), AggregateType.Average, null);
            e.IsDbExpression().Should().BeTrue();
        }
        [TestMethod]
        public void IsDbExpression_SubQueryExpression_True()
        {
            Expression e = new SubQueryExpression(GetType(), new SelectExpression(GetType(), "alias", Enumerable.Empty<ColumnDeclaration>(),
                                                                                  Expression.Empty(), null));
            e.IsDbExpression().Should().BeTrue();
        }
        [TestMethod]
        public void IsDbExpression_AggregateSubQueryExpression_True()
        {
            Expression e = new AggregateSubQueryExpression("alias", Expression.Empty(),
                                                           new SubQueryExpression(
                                                               GetType(),
                                                               new SelectExpression(GetType(), "alias", Enumerable.Empty<ColumnDeclaration>(),
                                                                                    Expression.Empty(), null)));
            e.IsDbExpression().Should().BeTrue();
        }
        [TestMethod]
        public void IsDbExpression_IsNullExpression_True()
        {
            Expression e = new IsNullExpression(Expression.Empty());
            e.IsDbExpression().Should().BeTrue();
        }
    }
}
