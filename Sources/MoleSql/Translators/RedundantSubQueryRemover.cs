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
        sealed class RedundantSubqueryGatherer : DbExpressionVisitor
        {
            List<SelectExpression> redundant;

            RedundantSubqueryGatherer()
            {
            }

            protected override Expression VisitSelect(SelectExpression selectExpression)
            {
                if (!IsRedudantSubquery(selectExpression)) return selectExpression;
                redundant ??= new List<SelectExpression>();
                redundant.Add(selectExpression);
                return selectExpression;
            }

            static bool IsRedudantSubquery(SelectExpression selectExpression) => 
                (selectExpression.From is SelectExpression || selectExpression.From is TableExpression) && 
                (ProjectionIsSimple(selectExpression) || ProjectionIsNameMapOnly(selectExpression)) && 
                selectExpression.Where == null && 
                !(selectExpression.OrderBy?.Count > 0) &&
                !(selectExpression.GroupBy?.Count > 0);

            internal static List<SelectExpression> Gather(Expression source)
            {
                RedundantSubqueryGatherer gatherer = new RedundantSubqueryGatherer();
                gatherer.Visit(source);
                return gatherer.redundant;
            }
        }

        private RedundantSubQueryRemover() {}

        protected override Expression VisitSelect(SelectExpression selectExpression)
        {
            selectExpression = (SelectExpression)base.VisitSelect(selectExpression);

            List<SelectExpression> redundant = RedundantSubqueryGatherer.Gather(selectExpression.From);
            if (redundant != null)
                selectExpression = SubQueryRemover.Remove(selectExpression, redundant);

            while (CanMergeWithFrom(selectExpression))
            {
                SelectExpression fromSelect = (SelectExpression)selectExpression.From;
                selectExpression = SubQueryRemover.Remove(selectExpression, fromSelect);

                Expression where = selectExpression.Where;
                if (fromSelect.Where != null)
                {
                    where = where == null 
                            ? fromSelect.Where 
                            : Expression.And(fromSelect.Where, where);
                }

                if (where != selectExpression.Where)
                    selectExpression = new SelectExpression(
                        selectExpression.Type, 
                        selectExpression.Alias, 
                        selectExpression.Columns, 
                        selectExpression.From, 
                        where, 
                        selectExpression.OrderBy, 
                        selectExpression.GroupBy);
            }

            return selectExpression;
        }
        protected override Expression VisitProjection(ProjectionExpression projection)
        {
            projection = (ProjectionExpression)base.VisitProjection(projection);
            if (!(projection.Source.From is SelectExpression)) return projection;

            List<SelectExpression> redundant = RedundantSubqueryGatherer.Gather(projection.Source);
            return redundant == null 
                   ? projection
                   : SubQueryRemover.Remove(projection, redundant);
        }

        static bool CanMergeWithFrom(SelectExpression selectExpression) => selectExpression.From is SelectExpression fromSelect &&
                                                                           (ProjectionIsSimple(fromSelect) || ProjectionIsNameMapOnly(fromSelect)) &&
                                                                           !(fromSelect.OrderBy?.Count > 0) &&
                                                                           !(fromSelect.GroupBy?.Count > 0);
        static bool ProjectionIsSimple(SelectExpression selectExpression) =>
            selectExpression.Columns.All(columnDeclaration =>
                                             columnDeclaration.Expression is ColumnExpression columnExpression &&
                                             columnDeclaration.Name == columnExpression.Name);
        static bool ProjectionIsNameMapOnly(SelectExpression selectExpression)
        {
            if (!(selectExpression.From is SelectExpression fromSelect) || selectExpression.Columns.Count != fromSelect.Columns.Count)
                return false;
            for (int i = 0; i < selectExpression.Columns.Count; i++)
            {
                if (!(selectExpression.Columns[i].Expression is ColumnExpression col) || col.Name != fromSelect.Columns[i].Name)
                    return false;
            }
            return true;
        }

        internal static Expression Remove(Expression expression) => new RedundantSubQueryRemover().Visit(expression);
    }
}
