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

namespace MoleSql.Expressions
{
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
                          (ExpressionType)DbExpressionType.Aggregate => VisitAggregate((AggregateExpression)expression),
                          (ExpressionType)DbExpressionType.SubQuery => VisitSubQuery((SubQueryExpression)expression),
                          (ExpressionType)DbExpressionType.AggregateSubQuery => VisitAggregateSubQuery((AggregateSubQueryExpression)expression),
                          (ExpressionType)DbExpressionType.IsNull => VisitIsNull((IsNullExpression)expression),
                          _ => base.Visit(expression)
                      };
        //protected override Expression VisitNew(NewExpression newExpression)
        //{
        //    var arguments = VisitExpressionList(newExpression.Arguments);
        //    return arguments == newExpression.Arguments ? newExpression : Expression.New(newExpression.Constructor, arguments);
        //}
        protected virtual Expression VisitTable(TableExpression table) => table;
        protected virtual Expression VisitColumn(ColumnExpression column) => column;
        protected virtual Expression VisitSelect(SelectExpression selectExpression)
        {
            Expression from = VisitSource(selectExpression.From);
            Expression where = Visit(selectExpression.Where);
            ReadOnlyCollection<ColumnDeclaration> columns = VisitColumnDeclarations(selectExpression.Columns);
            ReadOnlyCollection<OrderClause> orderBy = VisitOrderBy(selectExpression.OrderBy);
            ReadOnlyCollection<Expression> groupBy = VisitExpressionList(selectExpression.GroupBy);

            return from != selectExpression.From || where != selectExpression.Where || columns != selectExpression.Columns || orderBy != selectExpression.OrderBy || groupBy != selectExpression.GroupBy
                       ? new SelectExpression(selectExpression.Type, selectExpression.Alias, columns, from, where, orderBy, groupBy)
                       : selectExpression;
        }
        protected virtual Expression VisitSource(Expression source) => Visit(source);
        protected virtual Expression VisitProjection(ProjectionExpression projection)
        {
            SelectExpression source = (SelectExpression)Visit(projection.Source);
            Expression projector = Visit(projection.Projector);

            return source != projection.Source || projector != projection.Projector
                       ? new ProjectionExpression(source, projector, projection.IsTopLevelAggregation)
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
        protected virtual Expression VisitAggregate(AggregateExpression aggregateExpression)
        {
            if (aggregateExpression == null) return null;

            var argument = Visit(aggregateExpression.Argument);
            return argument != aggregateExpression.Argument
                       ? new AggregateExpression(argument?.Type ?? aggregateExpression.Type, aggregateExpression.AggregateType, argument)
                       : aggregateExpression;
        }
        protected virtual Expression VisitSubQuery(SubQueryExpression subQuery)
        {
            SelectExpression selectExpression = (SelectExpression)Visit(subQuery.SelectExpression);
            return selectExpression != subQuery.SelectExpression
                       ? new SubQueryExpression(subQuery.Type, selectExpression)
                       : subQuery;
        }
        protected virtual Expression VisitAggregateSubQuery(AggregateSubQueryExpression aggregateSubQueryExpression)
        {
            var subQuery = (SubQueryExpression)Visit(aggregateSubQueryExpression.AggregateAsSubQuery);
            return subQuery != aggregateSubQueryExpression.AggregateAsSubQuery
                       ? new AggregateSubQueryExpression(aggregateSubQueryExpression.GroupByAlias, aggregateSubQueryExpression.AggregateInGroupSelect,
                                                         subQuery)
                       : aggregateSubQueryExpression;
        }
        protected virtual Expression VisitIsNull(IsNullExpression isNullExpression)
        {
            if (isNullExpression == null) return null;

            var expression = Visit(isNullExpression.Expression);
            return expression != isNullExpression.Expression
                       ? new IsNullExpression(expression)
                       : isNullExpression;
        }
        protected virtual ReadOnlyCollection<ColumnDeclaration> VisitColumnDeclarations(ReadOnlyCollection<ColumnDeclaration> columns)
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
        protected virtual ReadOnlyCollection<OrderClause> VisitOrderBy(ReadOnlyCollection<OrderClause> orderBy)
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
        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> expressions)
        {
            if (expressions == null) return null;

            List<Expression> alternate = null;
            for (int i = 0; i < expressions.Count; i++)
            {
                Expression expression = expressions[i];
                Expression result = Visit(expression);
                if (result != expression && alternate == null)
                    alternate = expressions.Take(i).ToList();
                alternate?.Add(result);
            }

            return alternate?.AsReadOnly() ?? expressions;
        }
    }
}
