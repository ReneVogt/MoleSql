﻿/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System.Linq.Expressions;
using System.Reflection;
using MoleSql.Expressions;

namespace MoleSql.Translators
{
    sealed class ProjectionBuilder : DbExpressionVisitor
    {
        static readonly MethodInfo getValueMethod = typeof(ProjectionRow).GetMethod("GetValue", BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly ParameterExpression row = Expression.Parameter(typeof(ProjectionRow), "row");

        // ReSharper disable once AssignNullToNotNullAttribute
        internal LambdaExpression Build(Expression expression) => Expression.Lambda(Visit(expression), row);

        protected override Expression VisitColumn(ColumnExpression column) => Expression.Convert(Expression.Call(row, getValueMethod, Expression.Constant(column.Ordinal)), column.Type);
    }
}
