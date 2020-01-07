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

        UnusedColumnsRemover() {}

        protected override Expression VisitColumn(ColumnExpression column)
        {
            MarkColumnAsUsed(column.Alias, column.Name);
            return column;
        }
        protected override Expression VisitSubQuery(SubQueryExpression subQuery)
        {
            MarkColumnAsUsed(subQuery.SelectExpression.Alias, subQuery.SelectExpression.Columns[0].Name);
            return base.VisitSubQuery(subQuery);
        }
        protected override Expression VisitSelect(SelectExpression selectExpression)
        {
            ReadOnlyCollection<ColumnDeclaration> columns = selectExpression.Columns;

            List<ColumnDeclaration> alternate = null;
            for (int i = 0; i < selectExpression.Columns.Count; i++)
            {
                ColumnDeclaration columnDeclaration = selectExpression.Columns[i];
                if (IsColumnUsed(selectExpression.Alias, columnDeclaration.Name))
                {
                    Expression expr = Visit(columnDeclaration.Expression);
                    if (expr != columnDeclaration.Expression)
                        columnDeclaration = new ColumnDeclaration(columnDeclaration.Name, expr);
                }
                else
                    columnDeclaration = null;

                if (columnDeclaration != selectExpression.Columns[i] && alternate == null)
                    alternate = selectExpression.Columns.Take(i).ToList();
                if (columnDeclaration != null)
                    alternate?.Add(columnDeclaration);
            }
            
            if (alternate != null)
                columns = alternate.AsReadOnly();

            ReadOnlyCollection<Expression> groupbys = VisitExpressionList(selectExpression.GroupBy);
            ReadOnlyCollection<OrderClause> orderbys = VisitOrderBy(selectExpression.OrderBy);
            Expression where = Visit(selectExpression.Where);
            Expression from = Visit(selectExpression.From);

            ClearColumnsUsed(selectExpression.Alias);

            return columns != selectExpression.Columns ||
                   orderbys != selectExpression.OrderBy ||
                   groupbys != selectExpression.GroupBy ||
                   where != selectExpression.Where ||
                   from != selectExpression.From
                       ? new SelectExpression(selectExpression.Type, selectExpression.Alias, columns, from, where, orderbys, groupbys)
                       : selectExpression;
        }
        protected override Expression VisitProjection(ProjectionExpression projection)
        {
            Expression projector = Visit(projection.Projector);
            SelectExpression source = (SelectExpression)Visit(projection.Source);
            return projector != projection.Projector || source != projection.Source
                       ? new ProjectionExpression(source, projector, projection.IsTopLevelAggregation)
                       : projection;
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

        void MarkColumnAsUsed(string alias, string name)
        {
            if (!allColumnsUsed.TryGetValue(alias, out var columns))
            {
                columns = new HashSet<string>();
                allColumnsUsed.Add(alias, columns);
            }

            columns.Add(name);
        }
        bool IsColumnUsed(string alias, string name) => allColumnsUsed.TryGetValue(alias, out var columns) && columns.Contains(name);
        void ClearColumnsUsed(string alias)
        {
            allColumnsUsed[alias] = new HashSet<string>();
        }

        internal static Expression Remove(Expression expression) => new UnusedColumnsRemover().Visit(expression);
    }
}
