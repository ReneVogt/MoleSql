/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System;
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
        static readonly MethodInfo executeSubQueryMethod = typeof(ProjectionRow).GetMethod(nameof(ProjectionRow.ExecuteSubQuery), BindingFlags.Instance | BindingFlags.NonPublic);
        
        readonly ParameterExpression row = Expression.Parameter(typeof(ProjectionRow), "row");
        readonly string alias;

        ProjectionBuilder(string alias)
        {
            this.alias = alias;
        }

        protected override Expression VisitColumn(ColumnExpression column) =>
            column.Alias == alias
                ? Expression.Convert(Expression.Call(row, getValueMethod, Expression.Constant(column.Ordinal)), column.Type)
                : base.VisitColumn(column);
        protected override Expression VisitProjection(ProjectionExpression projectionExpression)
        {
            LambdaExpression subQuery = Expression.Lambda(base.VisitProjection(projectionExpression), row);
            Type elementType = TypeSystemHelper.GetElementType(subQuery.Body.Type);
            MethodInfo genericExecuteSubQueryMethod = executeSubQueryMethod.MakeGenericMethod(elementType);
            return Expression.Convert(Expression.Call(row, genericExecuteSubQueryMethod, Expression.Constant(subQuery)), projectionExpression.Type);
        }

        internal static LambdaExpression Build(Expression expression, string alias)
        {
            var builder = new ProjectionBuilder(alias);
            var body = builder.Visit(expression);
            Debug.Assert(body != null);
            return Expression.Lambda(body, builder.row);
        }
    }
}
