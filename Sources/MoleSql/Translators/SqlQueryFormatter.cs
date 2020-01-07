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

        const int IndentationWidth = 2;

        readonly StringBuilder commandTextBuilder = new StringBuilder();
        readonly List<(string name, object value)> parameters = new List<(string name, object value)>();
        
        int depth;

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

            if (binaryExpression.Right.NodeType == ExpressionType.Constant && binaryExpression.Right is ConstantExpression constantExpression &&
                constantExpression.Value == null)
            {
                commandTextBuilder.Append(binaryExpression.NodeType switch
                {
                    ExpressionType.Equal => " IS NULL",
                    ExpressionType.NotEqual => " IS NOT NULL",
                    _ => throw new InvalidOperationException($"Cannot apply operator '{binaryExpression.NodeType}' to NULL values.")
                });
                commandTextBuilder.Append(")");
                return binaryExpression;
            }

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
                    _ => throw new NotSupportedException($"The binary operator '{binaryExpression.NodeType}' is not supported.")
                });
            Visit(binaryExpression.Right);
            commandTextBuilder.Append(")");
            return binaryExpression;
        }
        protected override Expression VisitConstant(ConstantExpression constant)
        {
            if (constant.Value == null)
                throw new InvalidOperationException("Cannot translate a binary expression with a left side value of NULL.");
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
                ColumnDeclaration columnDeclaration = selectExpression.Columns[i];
                if (i > 0)
                    commandTextBuilder.Append(", ");

                Expression columnSource = Visit(columnDeclaration.Expression);
                if (!string.IsNullOrWhiteSpace(columnDeclaration?.Name) && (!(columnSource is ColumnExpression columnExpression) || columnExpression.Name != selectExpression.Columns[i].Name))
                    commandTextBuilder.Append($" AS {columnDeclaration.Name}");
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

            if (selectExpression.OrderBy?.Count > 0)
            {
                AppendNewLine(Indentation.Same);
                commandTextBuilder.Append("ORDER BY ");
                for (int i = 0, n = selectExpression.OrderBy.Count; i < n; i++)
                {
                    var orderClause = selectExpression.OrderBy[i];
                    if (i > 0) commandTextBuilder.Append(", ");
                    Visit(orderClause.Expression);
                    if (orderClause.OrderType != OrderType.Ascending) commandTextBuilder.Append(" DESC");
                }
            }

            if (selectExpression.GroupBy?.Count > 0)
            {
                AppendNewLine(Indentation.Same);
                commandTextBuilder.Append("GROUP BY ");
                for (int i = 0; i < selectExpression.GroupBy.Count; i++)
                {
                    if (i > 0)
                        commandTextBuilder.Append(", ");
                    Visit(selectExpression.GroupBy[i]);
                }
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
                _ => throw new NotSupportedException($"The JOIN type '{joinExpression.JoinType}' is not supported.")
            });

            VisitSource(joinExpression.Right);

            if (joinExpression.Condition == null) return joinExpression;
            
            AppendNewLine(Indentation.Inner);
            commandTextBuilder.Append("ON ");
            Visit(joinExpression.Condition); 
            Indent(Indentation.Outer);

            return joinExpression;
        }
        protected override Expression VisitSource(Expression source)
        {
            switch ((DbExpressionType)source.NodeType)
            {
                case DbExpressionType.Table:
                    TableExpression table = (TableExpression)source;
                    commandTextBuilder.Append($"[{table.Name}] AS {table.Alias}");
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
                    throw new InvalidOperationException($"Select source type '{source.NodeType}' is not valid.");
            }

            return source;
        }
        protected override Expression VisitAggregate(AggregateExpression aggregate)
        {
            commandTextBuilder.Append(GetAggregateName(aggregate.AggregateType));
            commandTextBuilder.Append("(");
            if (aggregate.Argument != null) 
                Visit(aggregate.Argument);
            else if (RequiresAsteriskWhenNoArgument(aggregate.AggregateType))
                commandTextBuilder.Append("*");
            commandTextBuilder.Append(")");
            return aggregate;
        }
        protected override Expression VisitSubQuery(SubQueryExpression subQueryExpression)
        {
            commandTextBuilder.Append("(");
            AppendNewLine(Indentation.Inner);
            Visit(subQueryExpression.SelectExpression);
            AppendNewLine(Indentation.Same);
            commandTextBuilder.Append(")");
            Indent(Indentation.Outer);
            return subQueryExpression;
        }
        protected override Expression VisitIsNull(IsNullExpression isNullExpression)
        {
            Visit(isNullExpression.Expression);
            commandTextBuilder.Append(" IS NULL");
            return isNullExpression;
        }

        void AppendNewLine(Indentation style)
        {
            commandTextBuilder.AppendLine();
            if (style == Indentation.Inner) depth++;
            if (style == Indentation.Outer) depth--;
            commandTextBuilder.Append(' ', depth * IndentationWidth);
        }
        void Indent(Indentation style)
        {
            if (style == Indentation.Inner)
                depth++;
            else if (style == Indentation.Outer)
                depth--;
        }

        static string GetAggregateName(AggregateType aggregateType) =>
            aggregateType switch
            {
                AggregateType.Count => "COUNT",
                AggregateType.Min => "MIN",
                AggregateType.Max => "MAX",
                AggregateType.Sum => "SUM",
                AggregateType.Average => "AVG",
                _ => throw new NotSupportedException($"Aggregate type '{aggregateType}' is not supported.")
            };
        static bool RequiresAsteriskWhenNoArgument(AggregateType aggregateType) => aggregateType == AggregateType.Count;

        internal static (string, List<(string name, object value)> parameters) Format(Expression expression)
        {
            var formatter = new SqlQueryFormatter();
            formatter.Visit(expression);

            return (formatter.commandTextBuilder.ToString(), formatter.parameters);
        }
    }
}
