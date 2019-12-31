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
        protected virtual Expression VisitSelect(SelectExpression select)
        {
            Expression from = VisitSource(select.From);
            Expression where = Visit(select.Where);

            ReadOnlyCollection<ColumnDeclaration> columns = VisitColumnDeclarations(select.Columns);

            return from != select.From || where != select.Where || columns != select.Columns
                       ? new SelectExpression(select.Type, select.Alias, columns, from, where)
                       : select;
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
        protected virtual Expression VisitJoin(JoinExpression join)
        {
            var left = Visit(join.Left);
            var right = Visit(join.Right);
            var condition = Visit(join.Condition);
            return left != join.Left || right != join.Right || condition != join.Condition
                   ? new JoinExpression(join.Type, join.JoinType, left, right, condition)
                   : join;
        }
        protected ReadOnlyCollection<ColumnDeclaration> VisitColumnDeclarations(ReadOnlyCollection<ColumnDeclaration> columns)
        {
            List<ColumnDeclaration> alternate = null;
            for (int i = 0, n = columns.Count; i < n; i++)
            { 
                ColumnDeclaration column = columns[i];
                Expression e = Visit(column.Expression);
                if (alternate == null && e != column.Expression)
                    alternate = columns.Take(i).ToList();

                alternate?.Add(new ColumnDeclaration(column.Name, e));
            }

            return alternate?.AsReadOnly() ?? columns;
        }
    }
}
