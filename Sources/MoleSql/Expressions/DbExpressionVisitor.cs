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
    /// <summary>
    /// Extends the <see cref="ExpressionVisitor"/> class to handle CLR/SQL-hybrid expression trees
    /// and the extended <see cref="ExpressionType"/> values in <see cref="DbExpressionType"/>.
    /// </summary>
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
                          _ => base.Visit(expression)
                      };
        protected virtual Expression VisitTable(TableExpression table) => table;
        protected virtual Expression VisitColumn(ColumnExpression column) => column;
        /// <summary>
        /// Visits a <see cref="SelectExpression"/> by visiting its <see cref="SelectExpression.From"/> and
        /// <see cref="SelectExpression.Where"/> expressions and the expressions for the column declarations (<see cref="SelectExpression.Columns"/>).
        /// </summary>
        /// <param name="select">The <see cref="SelectExpression"/> to visit.</param>
        /// <returns>The resulting <see cref="SelectExpression"/>.</returns>
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
        /// <summary>
        /// Visits a <see cref="ProjectionExpression"/> by visiting its <see cref="SelectExpression"/> <see cref="ProjectionExpression.Source"/>
        /// and its <see cref="ProjectionExpression.Projector"/>.
        /// </summary>
        /// <param name="projection">The <see cref="ProjectionExpression"/> to visit.</param>
        /// <returns>The resulting <see cref="ProjectionExpression"/>.</returns>
        protected virtual Expression VisitProjection(ProjectionExpression projection)
        {
            SelectExpression source = (SelectExpression)Visit(projection.Source);
            Expression projector = Visit(projection.Projector);

            return source != projection.Source || projector != projection.Projector
                       ? new ProjectionExpression(source, projector)
                       : projection;
        }
        /// <summary>
        /// Visits the expression of each <see cref="ColumnDeclaration"/> of a <see cref="SelectExpression"/>.
        /// </summary>
        /// <param name="columns">The list of <see cref="ColumnDeclaration"/>s to visit.</param>
        /// <returns>The resulting list of <see cref="ColumnDeclaration"/>s.</returns>
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
