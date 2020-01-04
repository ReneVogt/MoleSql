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
    sealed class ColumnProjector : DbExpressionVisitor
    {
        class Nominator : DbExpressionVisitor
        {
            bool isBlocked;
            readonly HashSet<Expression> candidates = new HashSet<Expression>();

            Nominator() { }

            public override Expression Visit(Expression expression)
            {
                if (expression == null) return null;
                
                bool oldIsBlocked = isBlocked; 
                isBlocked = false;

                if (expression.NodeType != (ExpressionType)DbExpressionType.SubQuery)
                    base.Visit(expression);

                isBlocked |= !CanBeColumn(expression);
                if (!isBlocked)
                    candidates.Add(expression);

                isBlocked |= oldIsBlocked;
                return expression;
            }

            static bool CanBeColumn(Expression expression) => expression.NodeType switch
            {
                (ExpressionType)DbExpressionType.Column => true,
                (ExpressionType)DbExpressionType.SubQuery => true,
                (ExpressionType)DbExpressionType.AggregateSubQuery => true,
                (ExpressionType)DbExpressionType.Aggregate => true,
                _ => false
            };

            internal static HashSet<Expression> Nominate(Expression expression)
            {
                var nominator = new Nominator();
                nominator.Visit(expression);
                return nominator.candidates;
            }
        }

        readonly Dictionary<ColumnExpression, ColumnExpression> map = new Dictionary<ColumnExpression, ColumnExpression>();
        readonly List<ColumnDeclaration> columns = new List<ColumnDeclaration>();
        readonly HashSet<string> columnNames = new HashSet<string>();
        readonly HashSet<Expression> candidates;
        readonly string[] existingAliases;
        readonly string newAlias;
        int column;

        ColumnProjector(HashSet<Expression> candidates, string newAlias, string[] existingAliases)
        {
            this.candidates = candidates;
            this.newAlias = newAlias;
            this.existingAliases = existingAliases;
        }

        public override Expression Visit(Expression expression)
        {
            if (!candidates.Contains(expression)) return base.Visit(expression);

            string columnName;

            if (expression.NodeType != (ExpressionType)DbExpressionType.Column)
            {
                columnName = GetNextColumnName();
                columns.Add(new ColumnDeclaration(columnName, expression));
                return new ColumnExpression(expression.Type, newAlias, columnName);
            }

            ColumnExpression columnExpression = (ColumnExpression)expression;
            if (map.TryGetValue(columnExpression, out var mapped)) return mapped;
            if (existingAliases.Contains(columnExpression.Alias))
            {
                columnName = GetUniqueColumnName(columnExpression.Name);
                columns.Add(new ColumnDeclaration(columnName, columnExpression));

                mapped = new ColumnExpression(columnExpression.Type, newAlias, columnName);
                map[columnExpression] = mapped;
                columnNames.Add(columnName);

                return mapped;
            }

            return columnExpression;
        }

        string GetUniqueColumnName(string name)
        {
            string baseName = name;
            int suffix = 1;
            while (columnNames.Contains(name))
                name = $"{baseName}{suffix++}";

            return name;
        }
        string GetNextColumnName() => GetUniqueColumnName($"c{column++}");

        internal static (Expression projector, ReadOnlyCollection<ColumnDeclaration> columns) ProjectColumns(Expression expression, string newAlias, params string[] existingAliases)
        {
            var projector = new ColumnProjector(Nominator.Nominate(expression), newAlias, existingAliases);
            return (projector.Visit(expression), projector.columns.AsReadOnly());
        }

    }
}
