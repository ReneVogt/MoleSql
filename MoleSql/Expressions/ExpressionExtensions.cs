using System.Linq.Expressions;

namespace MoleSql.Expressions
{
    static class ExpressionExtensions
    {
        internal static Expression StripQuotes(this Expression expression)
        {
            while (expression?.NodeType == ExpressionType.Quote)
                expression = ((UnaryExpression)expression).Operand;

            return expression;
        }
    }
}
