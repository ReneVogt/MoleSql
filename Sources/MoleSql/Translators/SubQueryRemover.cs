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
using System.Linq;
using System.Linq.Expressions;
using MoleSql.Expressions;

namespace MoleSql.Translators
{
    sealed class SubQueryRemover : DbExpressionVisitor
    {
        readonly HashSet<SelectExpression> selectsToRemove;
        readonly Dictionary<string, Dictionary<string, Expression>> map;

        SubQueryRemover(IEnumerable<SelectExpression> selectsToRemove)
        {
            this.selectsToRemove = new HashSet<SelectExpression>(selectsToRemove);
            map = this.selectsToRemove.ToDictionary(selectExpression => selectExpression.Alias,
                                               selectExpression =>
                                                   selectExpression.Columns.ToDictionary(column => column.Name, column => column.Expression));
        }

        protected override Expression VisitSelect(SelectExpression selectExpression) =>
            selectsToRemove.Contains(selectExpression) ? Visit(selectExpression.From) : base.VisitSelect(selectExpression);
        protected override Expression VisitColumn(ColumnExpression columnExpression) => map.TryGetValue(columnExpression.Alias, out var nameMap)
                                                                                            ? nameMap.TryGetValue(columnExpression.Name,
                                                                                                                  out var expression)
                                                                                                  ? Visit(expression)
                                                                                                  : throw new InvalidOperationException(
                                                                                                        "Reference to undefined column.")
                                                                                            : columnExpression;

        internal static SelectExpression Remove(SelectExpression outerSelect, params SelectExpression[] selectsToRemove) => Remove(outerSelect, (IEnumerable<SelectExpression>)selectsToRemove);
        internal static SelectExpression Remove(SelectExpression outerSelect, IEnumerable<SelectExpression> selectsToRemove) => (SelectExpression)new SubQueryRemover(selectsToRemove).Visit(outerSelect);
        internal static SelectExpression Remove(ProjectionExpression projection, params SelectExpression[] selectsToRemove) => Remove(projection, (IEnumerable<SelectExpression>)selectsToRemove);
        internal static SelectExpression Remove(ProjectionExpression projection, IEnumerable<SelectExpression> selectsToRemove) => (SelectExpression)new SubQueryRemover(selectsToRemove).Visit(projection);
    }
}
