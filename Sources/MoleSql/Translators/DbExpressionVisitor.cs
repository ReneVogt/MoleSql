using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using MoleSql.Expressions;

namespace MoleSql.Translators
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
        protected virtual Expression VisitSource(Expression source) => source;
        protected virtual Expression VisitProjection(ProjectionExpression projection)
        {
            SelectExpression source = (SelectExpression)Visit(projection.Source);
            Expression projector = Visit(projection.Projector);

            return source != projection.Source || projector != projection.Projector
                       ? new ProjectionExpression(source, projector)
                       : projection;
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
