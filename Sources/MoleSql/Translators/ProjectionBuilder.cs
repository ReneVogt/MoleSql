/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System;
using System.Collections.Generic;
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
        readonly IList<string> columns;

        ProjectionBuilder(string alias, IList<string> columns)
        {
            this.alias = alias;
            this.columns = columns;
        }

        protected override Expression VisitColumn(ColumnExpression column)
        {
            if (column.Alias != alias) return base.VisitColumn(column);

            return Expression.Call(row, getValueMethod.MakeGenericMethod(column.Type), Expression.Constant(columns.IndexOf(column.Name)));

            //if (!column.Type.IsGenericType || column.Type.GetGenericTypeDefinition() != typeof(Nullable<>))
            //    return Expression.Convert(
            //        Expression.Call(typeof(Convert), nameof(Convert.ChangeType), null,
            //                        getValueCall,
            //                        Expression.Constant(column.Type)
            //        ),
            //        column.Type
            //    );

            //var underlyingType = Nullable.GetUnderlyingType(column.Type);
            //var constructor = column.Type.GetConstructor(new[] {underlyingType});
            //Debug.Assert(constructor != null && underlyingType != null);
            //return Expression.Convert(Expression.New(constructor, Expression.Convert(getValueCall, underlyingType)), column.Type);
        }
        protected override Expression VisitProjection(ProjectionExpression projectionExpression)
        {
            LambdaExpression subQuery = Expression.Lambda(base.VisitProjection(projectionExpression), row);
            Type elementType = TypeSystemHelper.GetElementType(subQuery.Body.Type);
            MethodInfo genericExecuteSubQueryMethod = executeSubQueryMethod.MakeGenericMethod(elementType);
            return Expression.Convert(Expression.Call(row, genericExecuteSubQueryMethod, Expression.Constant(subQuery)), projectionExpression.Type);
        }

        internal static LambdaExpression Build(Expression expression, string alias, IList<string> columns)
        {
            var builder = new ProjectionBuilder(alias, columns);
            var body = builder.Visit(expression);
            Debug.Assert(body != null);
            return Expression.Lambda(body, builder.row);
        }
    }
}
