using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace MoleSql.Translators.Sql {
    sealed class ColumnProjector : ExpressionVisitor
    {
        static readonly MethodInfo getValueMethod = typeof(ProjectionRow).GetMethod("GetValue", BindingFlags.NonPublic | BindingFlags.Instance);

        readonly StringBuilder columnsListBuilder = new StringBuilder();
        ParameterExpression row;
        int column;

        internal (string columnsList, Expression projector) ProjectColumns(Expression selectorExpression, ParameterExpression rowExpression)
        {
            row = rowExpression;
            var transformedSelector = Visit(selectorExpression);
            return (columnsListBuilder.ToString(), transformedSelector);
        }

        protected override Expression VisitMember(MemberExpression memberExpression)
        {
            if (memberExpression.Expression.NodeType != ExpressionType.Parameter)
                return base.VisitMember(memberExpression);

            if (columnsListBuilder.Length > 0) columnsListBuilder.Append(", ");
            columnsListBuilder.Append(memberExpression.Member.Name); 
            return Expression.Convert(Expression.Call(row, getValueMethod, Expression.Constant(column++)), memberExpression.Type);
        }
    }
}
