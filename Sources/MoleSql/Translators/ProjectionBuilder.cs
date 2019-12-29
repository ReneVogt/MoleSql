/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using MoleSql.Expressions;
using MoleSql.Helpers;

namespace MoleSql.Translators
{
    sealed class ProjectionBuilder : DbExpressionVisitor
    {
        static readonly MethodInfo getValueMethod = typeof(ProjectionRow).GetMethod(nameof(ProjectionRow.GetValue), BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly ParameterExpression row = Expression.Parameter(typeof(ProjectionRow), "row");

        ProjectionBuilder()
        {
        }

        protected override Expression VisitColumn(ColumnExpression column) =>
            Expression.Convert(Expression.Call(row, getValueMethod, Expression.Constant(column.Ordinal)), column.Type);

        internal static LambdaExpression Build(Expression expression)
        {
            var body = new ProjectionBuilder().Visit(expression);
            Debug.Assert(body != null);
            return Expression.Lambda(body, row);
        }
    }
}
