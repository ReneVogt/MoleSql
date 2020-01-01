/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using MoleSql.Expressions;

namespace MoleSql.Translators
{
    sealed class SqlQueryFormatter : DbExpressionVisitor
    {
        enum Indentation
        {
            Same,
            Inner,
            Outer
        }

        readonly StringBuilder commandTextBuilder = new StringBuilder();
        readonly List<(string name, object value)> parameters = new List<(string name, object value)>();
        
        int depth;

        internal int IndentationWidth { get; set; } = 2;

        private void AppendNewLine(Indentation style)
        {
            commandTextBuilder.AppendLine();
            if (style == Indentation.Inner) depth++;
            if (style == Indentation.Outer) depth--;
            commandTextBuilder.Append(' ', depth * IndentationWidth);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m) => throw new NotSupportedException(
                                                                                     $"The method '{m.Method.Name}' is not supported.");
        protected override Expression VisitUnary(UnaryExpression unaryExpression)
        {
            if (unaryExpression.NodeType != ExpressionType.Not)
                throw new NotSupportedException($"The unary operator '{unaryExpression.NodeType}' is not supported.");

            commandTextBuilder.Append(" NOT ");
            Visit(unaryExpression.Operand);

            return unaryExpression;
        }
        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {
            commandTextBuilder.Append("("); 
            Visit(binaryExpression.Left);
            commandTextBuilder.Append(
                binaryExpression.NodeType switch
                {
                    var nt when nt == ExpressionType.And || nt == ExpressionType.AndAlso => " AND ",
                    var nt when nt == ExpressionType.Or || nt == ExpressionType.OrElse => " OR ",
                    ExpressionType.Equal => " = ",
                    ExpressionType.NotEqual => " <> ",
                    ExpressionType.LessThan => " < ",
                    ExpressionType.LessThanOrEqual => " <= ",
                    ExpressionType.GreaterThan => " > ",
                    ExpressionType.GreaterThanOrEqual => " >= ",
                    _ => throw new NotSupportedException($"The binary operator '{binaryExpression.NodeType}' is not supported")
                });
            Visit(binaryExpression.Right);
            commandTextBuilder.Append(")");
            return binaryExpression;
        }
        protected override Expression VisitConstant(ConstantExpression constant)
        {
            string name = $"@p{parameters.Count}";
            commandTextBuilder.Append(name);
            parameters.Add((name, constant.Value));

            return constant;
        }
        protected override Expression VisitColumn(ColumnExpression column)
        {
            if (!string.IsNullOrEmpty(column.Alias))
                commandTextBuilder.Append($"[{column.Alias}].");

            commandTextBuilder.Append($"[{column.Name}]");
            return column;
        }
        protected override Expression VisitSelect(SelectExpression selectExpression)
        {
            commandTextBuilder.Append("SELECT ");


            for (int i = 0; i < selectExpression.Columns.Count; i++)
            {
                ColumnDeclaration column = selectExpression.Columns[i];
                if (i > 0)
                    commandTextBuilder.Append(", ");
                
                ColumnExpression columnExpression = Visit(column.Expression) as ColumnExpression;
                if (columnExpression?.Name != selectExpression.Columns[i].Name)
                    commandTextBuilder.Append($" AS {column.Name}");
            }

            if (selectExpression.From != null)
            {
                AppendNewLine(Indentation.Same); 
                commandTextBuilder.Append("FROM "); 
                VisitSource(selectExpression.From);
            }

            if (selectExpression.Where != null)
            {
                AppendNewLine(Indentation.Same);
                commandTextBuilder.Append("WHERE ");
                Visit(selectExpression.Where);
            }

            if (selectExpression.OrderBy == null || selectExpression.OrderBy.Count == 0) return selectExpression;
            AppendNewLine(Indentation.Same);
            commandTextBuilder.Append("ORDER BY ");
            for (int i = 0, n = selectExpression.OrderBy.Count; i < n; i++)
            {
                var orderClause = selectExpression.OrderBy[i];
                if (i > 0) commandTextBuilder.Append(", ");
                Visit(orderClause.Expression);
                if (orderClause.OrderType != OrderType.Ascending) commandTextBuilder.Append(" DESC");
            }

            return selectExpression;
        }
        protected override Expression VisitJoin(JoinExpression joinExpression)
        {
            VisitSource(joinExpression.Left); 

            AppendNewLine(Indentation.Same);
            commandTextBuilder.Append(joinExpression.JoinType switch
            {
                JoinType.CrossJoin => "CROSS JOIN ",
                JoinType.InnerJoin => "INNER JOIN ",
                JoinType.CrossApply => "CROSS APPLY ",
                _ => throw new ArgumentException($"The JOIN type {joinExpression.JoinType} is not supported!")
            });

            VisitSource(joinExpression.Right);

            if (joinExpression.Condition == null) return joinExpression;
            
            AppendNewLine(Indentation.Inner);
            commandTextBuilder.Append("ON ");
            Visit(joinExpression.Condition); 
            AppendNewLine(Indentation.Outer);

            return joinExpression;
        }
        protected override Expression VisitSource(Expression source)
        {
            switch ((DbExpressionType)source.NodeType)
            {
                case DbExpressionType.Table:
                    TableExpression table = (TableExpression)source;
                    commandTextBuilder.Append($"{table.Name} AS {table.Alias}");
                    break;
                case DbExpressionType.Select:
                    SelectExpression select = (SelectExpression)source;
                    commandTextBuilder.Append("("); 
                    AppendNewLine(Indentation.Inner); 
                    Visit(select);
                    AppendNewLine(Indentation.Outer);
                    commandTextBuilder.Append($") AS {select.Alias}");
                    break;
                case DbExpressionType.Join:
                    VisitJoin((JoinExpression)source);
                    break;
                default:
                    throw new InvalidOperationException("Select source is not of a valid type.");
            }

            return source;
        }

        internal static (string, List<(string name, object value)> parameters) Format(Expression expression)
        {
            var formatter = new SqlQueryFormatter();
            formatter.Visit(expression);

            return (formatter.commandTextBuilder.ToString(), formatter.parameters);
        }
    }
}
