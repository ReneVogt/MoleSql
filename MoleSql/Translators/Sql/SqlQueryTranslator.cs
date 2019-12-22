using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MoleSql.Translators.Sql
{
    sealed class SqlQueryTranslator : ExpressionVisitor
    {
        StringBuilder queryStringBuilder;
        readonly ParameterExpression row = Expression.Parameter(typeof(ProjectionRow), "row");
        Expression projection;

        internal (string commandText, LambdaExpression projector) Translate(Expression expression)
        {
            queryStringBuilder = new StringBuilder();
            Visit(expression.EvaluateLocally());
            return (queryStringBuilder.ToString(), projection != null ? Expression.Lambda(projection, row) : null);
        }
        private static Expression StripQuotes(Expression e)
        {
            while (e?.NodeType == ExpressionType.Quote) 
                e = ((UnaryExpression)e).Operand;

            return e;
        }

        protected override Expression VisitMethodCall(MethodCallExpression callExpression)
        {
            if (callExpression.Method.DeclaringType != typeof(Queryable))
                throw new NotSupportedException($"The method '{callExpression.Method.Name}' is not supported");

            var firstLambda = StripQuotes(callExpression.Arguments[1]) as LambdaExpression;
            switch (callExpression.Method.Name)
            {
                case nameof(Queryable.Where):
                    queryStringBuilder.Append("SELECT * FROM (");
                    Visit(callExpression.Arguments[0]);
                    queryStringBuilder.Append(") AS T WHERE ");
                    Visit(firstLambda?.Body);
                    return callExpression;
                case nameof(Queryable.Select):
                    (string columnsList, var selector) = new ColumnProjector().ProjectColumns(firstLambda?.Body, row);
                    queryStringBuilder.Append("SELECT ");
                    queryStringBuilder.Append(columnsList);
                    queryStringBuilder.Append(" FROM (");
                    Visit(callExpression.Arguments[0]);
                    queryStringBuilder.Append(") AS T ");
                    projection = selector;
                    return callExpression;

            }

            throw new NotSupportedException($"The method '{callExpression.Method.Name}' is not supported");
        }
        protected override Expression VisitUnary(UnaryExpression unaryExpression)
        {
            switch (unaryExpression.NodeType)
            {
                case ExpressionType.Not:
                    queryStringBuilder.Append(" NOT ");
                    Visit(unaryExpression.Operand);
                    break;

                default:
                    throw new NotSupportedException($"The unary operator '{unaryExpression.NodeType}' is not supported");
            }

            return unaryExpression;
        }
        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {
            queryStringBuilder.Append("(");
            Visit(binaryExpression.Left);
            queryStringBuilder.Append(binaryExpression.NodeType switch
            {
                var nt when nt == ExpressionType.And || nt == ExpressionType.AndAlso => " AND ",
                var nt when nt == ExpressionType.Or || nt == ExpressionType.OrElse => " OR ",
                ExpressionType.Equal => " = ",
                ExpressionType.NotEqual => " <> ",
                ExpressionType.LessThan => " < ",
                ExpressionType.LessThanOrEqual => " <= ",
                ExpressionType.GreaterThan => " > ",
                ExpressionType.GreaterThanOrEqual => " >= ",
                _ => throw new NotSupportedException($"The binary operator '{binaryExpression.NodeType}' is not supported")
            });

            Visit(binaryExpression.Right);

            queryStringBuilder.Append(")");

            return binaryExpression;
        }
        protected override Expression VisitConstant(ConstantExpression constant)
        {
            if (constant.Value is IQueryable queryable)
            {
                // assume constant nodes w/ IQueryables are table references
                queryStringBuilder.Append("SELECT * FROM ");
                queryStringBuilder.Append(queryable.ElementType.Name);
                return constant;
            }
            
            if (constant.Value == null)
            {
                queryStringBuilder.Append("NULL");
                return constant;
            }

            switch (Type.GetTypeCode(constant.Value.GetType()))
            {
                case TypeCode.Boolean:
                    queryStringBuilder.Append(((bool)constant.Value) ? 1 : 0);
                    break;

                case TypeCode.String:
                    queryStringBuilder.Append("'");
                    queryStringBuilder.Append(constant.Value);
                    queryStringBuilder.Append("'");
                    break;

                case TypeCode.Object:
                    throw new NotSupportedException($"The constant for '{constant.Value}' is not supported");

                default:
                    queryStringBuilder.Append(constant.Value);
                    break;
            }

            return constant;
        }
        protected override Expression VisitMember(MemberExpression member)
        {
            if (member.Expression == null || member.Expression.NodeType != ExpressionType.Parameter)
                throw new NotSupportedException($"The member '{member.Member.Name}' is not supported");

            queryStringBuilder.Append(member.Member.Name);
            return member;
        }
    }
}
