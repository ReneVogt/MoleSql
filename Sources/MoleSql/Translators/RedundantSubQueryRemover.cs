/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MoleSql.Expressions;

namespace MoleSql.Translators
{
    sealed class RedundantSubQueryRemover : DbExpressionVisitor
    {
        sealed class RedundantSubQueryGatherer : DbExpressionVisitor
        {
            List<SelectExpression> redundant;

            RedundantSubQueryGatherer() { }

            protected override Expression VisitSelect(SelectExpression selectExpression)
            {
                if (!IsRedudantSubquery(selectExpression)) return selectExpression;
                redundant ??= new List<SelectExpression>();
                redundant.Add(selectExpression);
                return selectExpression;
            }

            internal static List<SelectExpression> Gather(Expression source)
            {
                var gatherer = new RedundantSubQueryGatherer();
                gatherer.Visit(source);
                return gatherer.redundant;
            }
        }

        RedundantSubQueryRemover() { }

        protected override Expression VisitSelect(SelectExpression selectExpression)
        {
            selectExpression = (SelectExpression)base.VisitSelect(selectExpression);
            
            List<SelectExpression> redundantSubQueries = RedundantSubQueryGatherer.Gather(selectExpression.From);
            if (redundantSubQueries != null)
                selectExpression = (SelectExpression)SubQueryRemover.Remove(selectExpression, redundantSubQueries);

            if (!(selectExpression.From is SelectExpression fromSelect && HasSimpleProjection(fromSelect)))
                return selectExpression;
            
            selectExpression = (SelectExpression)SubQueryRemover.Remove(selectExpression, fromSelect);
            
            Expression where = selectExpression.Where;

            if (fromSelect.Where != null)
                where = where != null 
                        ? Expression.And(fromSelect.Where, where)
                        : fromSelect.Where;

            return where != selectExpression.Where
                       ? new SelectExpression(selectExpression.Type, selectExpression.Alias, selectExpression.Columns, selectExpression.From, where,
                                              selectExpression.OrderBy)
                       : selectExpression;
        }

        static bool IsRedudantSubquery(SelectExpression selectExpression) => HasSimpleProjection(selectExpression) &&
                                                                             selectExpression.Where == null &&
                                                                             (selectExpression.OrderBy == null ||
                                                                              selectExpression.OrderBy.Count == 0);
        static bool HasSimpleProjection(SelectExpression selectExpression) =>
            selectExpression.Columns.All(columnDeclaration =>
                                             columnDeclaration.Expression is ColumnExpression columnExpression &&
                                             columnExpression.Name == columnDeclaration.Name);

        internal static Expression Remove(Expression expression) => new RedundantSubQueryRemover().Visit(expression);

    }
}
