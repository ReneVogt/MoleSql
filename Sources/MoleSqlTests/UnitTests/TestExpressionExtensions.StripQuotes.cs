using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql.Expressions;

namespace MoleSqlTests.UnitTests
{
    partial class TestExpressionExtensions
    {
        [TestMethod]
        public void StripQuotes_Null_Null()
        {
            ((Expression)null).StripQuotes().Should().BeNull();
        }
        [TestMethod]
        public void StripQuotes_NonQuotedExpression_Expression()
        {
            Expression e = Expression.Constant(42);
            e.StripQuotes().Should().BeSameAs(e);
        }
        [TestMethod]
        public void StripQuotes_QuotedExpression_Stripped()
        {
            Expression e = Expression.Lambda(Expression.Empty());
            var quoted = Expression.Quote(e);
            quoted.StripQuotes().Should().BeSameAs(e);
        }
    }
}
