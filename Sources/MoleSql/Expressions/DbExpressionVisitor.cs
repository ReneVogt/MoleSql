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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace MoleSql.Expressions
{
    [ExcludeFromCodeCoverage]
    class DbExpressionVisitor : ExpressionVisitor
    {
        public override Expression Visit(Expression expression) =>
            expression == null
                ? null
                : expression.NodeType switch
                      {
                          (ExpressionType)DbExpressionType.Table => VisitTable((TableExpression)expression),
                          (ExpressionType)DbExpressionType.Column => VisitColumn((ColumnExpression)expression),
                          (ExpressionType)DbExpressionType.Select => VisitSelect((SelectExpression)expression),
                          (ExpressionType)DbExpressionType.Projection => VisitProjection((ProjectionExpression)expression),
                          (ExpressionType)DbExpressionType.Join => VisitJoin((JoinExpression)expression),
                          _ => base.Visit(expression)
                      };
        protected virtual Expression VisitTable(TableExpression table) => table;
        protected virtual Expression VisitColumn(ColumnExpression column) => column;
        protected virtual Expression VisitSelect(SelectExpression selectExpression)
        {
            Expression from = VisitSource(selectExpression.From);
            Expression where = Visit(selectExpression.Where);
            ReadOnlyCollection<ColumnDeclaration> columns = VisitColumnDeclarations(selectExpression.Columns);
            ReadOnlyCollection<OrderClause> orderBy = VisitOrderBy(selectExpression.OrderBy);

            return from != selectExpression.From || where != selectExpression.Where || columns != selectExpression.Columns || orderBy != selectExpression.OrderBy
                       ? new SelectExpression(selectExpression.Type, selectExpression.Alias, columns, from, where, orderBy)
                       : selectExpression;
        }
        protected virtual Expression VisitSource(Expression source) => Visit(source);
        protected virtual Expression VisitProjection(ProjectionExpression projection)
        {
            SelectExpression source = (SelectExpression)Visit(projection.Source);
            Expression projector = Visit(projection.Projector);

            return source != projection.Source || projector != projection.Projector
                       ? new ProjectionExpression(source, projector)
                       : projection;
        }
        protected virtual Expression VisitJoin(JoinExpression joinExpression)
        {
            var left = Visit(joinExpression.Left);
            var right = Visit(joinExpression.Right);
            var condition = Visit(joinExpression.Condition);
            return left != joinExpression.Left || right != joinExpression.Right || condition != joinExpression.Condition
                   ? new JoinExpression(joinExpression.Type, joinExpression.JoinType, left, right, condition)
                   : joinExpression;
        }
        protected ReadOnlyCollection<ColumnDeclaration> VisitColumnDeclarations(ReadOnlyCollection<ColumnDeclaration> columns)
        {
            List<ColumnDeclaration> alternate = null;
            for (int i = 0; i < columns.Count; i++)
            {
                ColumnDeclaration column = columns[i];
                Expression e = Visit(column.Expression);
                if (alternate == null && e != column.Expression)
                    alternate = columns.Take(i).ToList();

                alternate?.Add(new ColumnDeclaration(column.Name, e));
            }

            return alternate?.AsReadOnly() ?? columns;
        }
        protected ReadOnlyCollection<OrderClause> VisitOrderBy(ReadOnlyCollection<OrderClause> orderBy)
        {
            if (orderBy == null) return null;

            List<OrderClause> alternate = null;
            for (int i = 0; i < orderBy.Count; i++)
            {
                OrderClause order = orderBy[i];
                Expression e = Visit(order.Expression);
                if (alternate == null && e != order.Expression)
                    alternate = orderBy.Take(i).ToList();

                alternate?.Add(new OrderClause(order.OrderType, e));
            }

            return alternate?.AsReadOnly() ?? orderBy;
        }
    }
}
