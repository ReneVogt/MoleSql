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
    sealed class OrderByRewriter : DbExpressionVisitor
    {
        IEnumerable<OrderClause>? gatheredOrderings;
        bool isOuterMostSelect = true;

        OrderByRewriter()
        {
        }

        protected override Expression VisitSelect(SelectExpression selectExpression)
        {
            bool saveIsOuterMostSelect = isOuterMostSelect;

            try
            {
                isOuterMostSelect = false;
                selectExpression = (SelectExpression)base.VisitSelect(selectExpression);
                if (selectExpression.OrderBy?.Count > 0)
                    PrependOrderings(selectExpression.OrderBy);

                bool canHaveOrderBy = saveIsOuterMostSelect;
                bool canPassOnOrderings = !saveIsOuterMostSelect;

                IEnumerable<OrderClause>? orderings = canHaveOrderBy ? gatheredOrderings : null;
                ReadOnlyCollection<ColumnDeclaration> columns = selectExpression.Columns;
                if (gatheredOrderings != null)
                {
                    if (canPassOnOrderings)
                    {
                        HashSet<string> producedAliases = ProducedAliasesScanner.Gather(selectExpression.From);
                        (columns, gatheredOrderings) = RebindOrderings(gatheredOrderings, selectExpression.Alias, producedAliases, selectExpression.Columns);
                    }
                    else
                        gatheredOrderings = null;
                }

                return !ReferenceEquals(orderings, selectExpression.OrderBy) || columns != selectExpression.Columns 
                           ? new SelectExpression(selectExpression.Type, selectExpression.Alias, columns, selectExpression.From, selectExpression.Where, orderings)
                           : selectExpression;
            }
            finally
            {
                isOuterMostSelect = saveIsOuterMostSelect;
            }
        }
        protected override Expression VisitJoin(JoinExpression joinExpression)
        { 
            Expression left = VisitSource(joinExpression.Left);
            IEnumerable<OrderClause>? leftOrders = gatheredOrderings; 
            
            gatheredOrderings = null;
            Expression right = VisitSource(joinExpression.Right);
            
            PrependOrderings(leftOrders);

            Expression? condition = Visit(joinExpression.Condition);

            return left != joinExpression.Left || right != joinExpression.Right || condition != joinExpression.Condition
                       ? new JoinExpression(joinExpression.Type, joinExpression.JoinType, left, right, condition)
                       : joinExpression;
        }

        void PrependOrderings(IEnumerable<OrderClause>? newOrderings)
        {
            if (newOrderings == null) return;
            if (gatheredOrderings == null)
            {
                gatheredOrderings = newOrderings;
                return;
            }

            if (!(gatheredOrderings is List<OrderClause> list))
                gatheredOrderings = list = new List<OrderClause>(gatheredOrderings);
            list.InsertRange(0, newOrderings);
        }
        
        static (ReadOnlyCollection<ColumnDeclaration> columns, ReadOnlyCollection<OrderClause> orderClauses) RebindOrderings(IEnumerable<OrderClause> orderings, string alias, HashSet<string> existingAliases, ReadOnlyCollection<ColumnDeclaration> existingColumns)
        {
            List<ColumnDeclaration>? newColumns = null;
            List<ColumnDeclaration> existingColumnDeclarations = new List<ColumnDeclaration>(existingColumns);

            List<OrderClause> newOrderings = new List<OrderClause>();

            foreach (OrderClause ordering in orderings)
            {
                Expression expression = ordering.Expression;
                ColumnExpression? columnExpression = expression as ColumnExpression;

                if (columnExpression != null && existingAliases?.Contains(columnExpression.Alias) != true) continue;

                foreach (ColumnDeclaration columnDeclaration in existingColumnDeclarations)
                {
                    if (columnDeclaration.Expression == ordering.Expression ||
                        columnExpression != null && columnDeclaration.Expression is ColumnExpression declaredColumnExpression && 
                        columnExpression.Alias == declaredColumnExpression.Alias &&
                        columnExpression.Name == declaredColumnExpression.Name)
                    {
                        expression = new ColumnExpression(expression.Type, alias, columnDeclaration.Name);
                        break;
                    }
                }

                if (expression == ordering.Expression)
                {
                    if (newColumns == null)
                    {
                        newColumns = new List<ColumnDeclaration>(existingColumnDeclarations);
                        existingColumnDeclarations = newColumns;
                    }

                    string colName = columnExpression != null ? columnExpression.Name : "c" + existingColumnDeclarations.Count;
                    newColumns.Add(new ColumnDeclaration(colName, ordering.Expression));
                    expression = new ColumnExpression(expression.Type, alias, colName);
                }

                newOrderings.Add(new OrderClause(ordering.OrderType, expression));
            }

            return (existingColumns.ToList().AsReadOnly(), newOrderings.ToList().AsReadOnly());
        }

        internal static ProjectionExpression Rewrite(ProjectionExpression expression) => (ProjectionExpression)new OrderByRewriter().Visit(expression)!;
    }
}
