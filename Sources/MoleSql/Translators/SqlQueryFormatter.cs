﻿/*
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
        protected override Expression VisitSelect(SelectExpression select)
        {
            commandTextBuilder.Append("SELECT ");


            for (int i = 0; i < select.Columns.Count; i++)
            {
                ColumnDeclaration column = select.Columns[i];
                if (i > 0)
                    commandTextBuilder.Append(", ");
                
                ColumnExpression columnExpression = Visit(column.Expression) as ColumnExpression;
                if (columnExpression?.Name != select.Columns[i].Name)
                    commandTextBuilder.Append($" AS {column.Name}");
            }

            if (select.From != null)
            {
                AppendNewLine(Indentation.Same); 
                commandTextBuilder.Append("FROM "); 
                VisitSource(select.From);
            }

            if (select.Where != null)
            {
                AppendNewLine(Indentation.Same);
                commandTextBuilder.Append("WHERE ");
                Visit(select.Where);
            }

            return select;
        }
        protected override Expression VisitJoin(JoinExpression join)
        {
            VisitSource(join.Left); 

            AppendNewLine(Indentation.Same);
            commandTextBuilder.Append(join.JoinType switch
            {
                JoinType.CrossJoin => "CROSS JOIN ",
                JoinType.InnerJoin => "INNER JOIN ",
                JoinType.CrossApply => "CROSS APPLY ",
                _ => throw new ArgumentException($"The JOIN type {join.JoinType} is not supported!")
            });

            VisitSource(join.Right);

            if (join.Condition == null) return join;
            
            AppendNewLine(Indentation.Inner);
            commandTextBuilder.Append("ON ");
            Visit(join.Condition); 
            AppendNewLine(Indentation.Outer);

            return join;
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
