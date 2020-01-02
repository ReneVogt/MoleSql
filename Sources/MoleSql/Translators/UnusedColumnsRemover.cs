/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using MoleSql.Expressions;

namespace MoleSql.Translators
{
    sealed class UnusedColumnsRemover : DbExpressionVisitor
    {
        readonly Dictionary<string, HashSet<string>> allColumnsUsed = new Dictionary<string, HashSet<string>>();

        UnusedColumnsRemover() { }

        protected override Expression VisitColumn(ColumnExpression columnExpression)
        {
            if (!allColumnsUsed.TryGetValue(columnExpression.Alias, out var columns))
            {
                columns = new HashSet<string>(); 
                allColumnsUsed.Add(columnExpression.Alias, columns);
            }

            columns.Add(columnExpression.Name);
            return columnExpression;
        }
        protected override Expression VisitSelect(SelectExpression selectExpression)
        {
            ReadOnlyCollection<ColumnDeclaration> columns = selectExpression.Columns;
            
            if (allColumnsUsed.TryGetValue(selectExpression.Alias, out var columnsUsed))
            {
                List<ColumnDeclaration> alternate = null;
                for (int i = 0; i < selectExpression.Columns.Count; i++)
                {
                    ColumnDeclaration columnDeclaration = selectExpression.Columns[i];

                    if (!columnsUsed.Contains(columnDeclaration.Name)) 
                        columnDeclaration = null;
                    else
                    {
                        Expression columnExpression = Visit(columnDeclaration.Expression);
                        if (columnExpression != columnDeclaration.Expression)
                            columnDeclaration = new ColumnDeclaration(columnDeclaration.Name, columnDeclaration.Expression);
                    }

                    if (columnDeclaration != selectExpression.Columns[i] && alternate == null)
                        alternate = selectExpression.Columns.Take(i).ToList();

                    if (columnDeclaration != null)
                        alternate?.Add(columnDeclaration);
                }

                if (alternate != null)
                    columns = alternate.AsReadOnly();
            }

            ReadOnlyCollection<OrderClause> orderbys = VisitOrderBy(selectExpression.OrderBy);
            Expression where = Visit(selectExpression.Where);
            Expression from = Visit(selectExpression.From);

            return columns != selectExpression.Columns || orderbys != selectExpression.OrderBy || where != selectExpression.Where ||
                   from != selectExpression.From
                       ? new SelectExpression(selectExpression.Type, selectExpression.Alias, columns, from, where, orderbys)
                       : selectExpression;
        }
        protected override Expression VisitProjection(ProjectionExpression projectionExpression)
        {
            Expression projector = Visit(projectionExpression.Projector);
            SelectExpression source = (SelectExpression)Visit(projectionExpression.Source);
            return projector != projectionExpression.Projector || source != projectionExpression.Source
                       ? new ProjectionExpression(source, projector)
                       : projectionExpression;
        }
        protected override Expression VisitJoin(JoinExpression joinExpression)
        {
            Expression condition = Visit(joinExpression.Condition);
            Expression right = VisitSource(joinExpression.Right);
            Expression left = VisitSource(joinExpression.Left);

            return left != joinExpression.Left || right != joinExpression.Right || condition != joinExpression.Condition
                       ? new JoinExpression(joinExpression.Type, joinExpression.JoinType, left, right, condition)
                       : joinExpression;
        }

        internal static Expression Remove(Expression expression) => new UnusedColumnsRemover().Visit(expression);
    }
}
