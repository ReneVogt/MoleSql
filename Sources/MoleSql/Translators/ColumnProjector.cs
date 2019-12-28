/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System.Collections.Generic;
using System.Linq.Expressions;
using MoleSql.Expressions;

namespace MoleSql.Translators
{
    sealed class ColumnProjector : DbExpressionVisitor
    {
        class Nominator : DbExpressionVisitor
        {
            bool isBlocked;
            HashSet<Expression> candidates;

            internal HashSet<Expression> Nominate(Expression expression)
            {
                candidates = new HashSet<Expression>(); 
                isBlocked = false;
                Visit(expression); 
                return candidates;
            }

            public override Expression Visit(Expression expression)
            {
                if (expression == null) return null;
                
                bool oldIsBlocked = isBlocked; 
                isBlocked = false;

                base.Visit(expression);

                isBlocked |= expression.NodeType != (ExpressionType)DbExpressionType.Column;
                if (!isBlocked)
                    candidates.Add(expression);

                isBlocked |= oldIsBlocked;
                return expression;
            }
        }

        readonly Nominator nominator = new Nominator();
        Dictionary<ColumnExpression, ColumnExpression> map;
        List<ColumnDeclaration> columns;
        HashSet<string> columnNames;
        HashSet<Expression> candidates;
        string aliasExisting;
        string aliasNew;
        int column;

        internal ProjectedColumns ProjectColumns(Expression expression, string newAlias, string existingAlias)
        {
            map = new Dictionary<ColumnExpression, ColumnExpression>(); 
            columns = new List<ColumnDeclaration>(); 
            columnNames = new HashSet<string>(); 
            aliasNew = newAlias;
            aliasExisting = existingAlias;
            candidates = nominator.Nominate(expression);
            return new ProjectedColumns(Visit(expression), columns.AsReadOnly());
        }

        public override Expression Visit(Expression expression)
        {
            if (!candidates.Contains(expression)) return base.Visit(expression);

            int ordinal;
            string columnName;

            if (expression.NodeType != (ExpressionType)DbExpressionType.Column)
            {
                columnName = GetNextColumnName();
                ordinal = columns.Count;
                columns.Add(new ColumnDeclaration(columnName, expression));
                return new ColumnExpression(expression.Type, aliasNew, columnName, ordinal);
            }

            ColumnExpression columnExpression = (ColumnExpression)expression;
            if (map.TryGetValue(columnExpression, out var mapped)) return mapped;
            if (aliasExisting == columnExpression.Alias)
            {
                ordinal = columns.Count;
                columnName = GetUniqueColumnName(columnExpression.Name);
                columns.Add(new ColumnDeclaration(columnName, columnExpression));

                mapped = new ColumnExpression(columnExpression.Type, aliasNew, columnName, ordinal);
                map[columnExpression] = mapped;
                columnNames.Add(columnName);

                return mapped;
            }

            return columnExpression;
        }

        private string GetUniqueColumnName(string name)
        {
            string baseName = name;
            int suffix = 1;
            while (columnNames.Contains(name))
                name = $"{baseName}{suffix++}";

            return name;
        }
        private string GetNextColumnName() => GetUniqueColumnName($"c{column++}");
    }
}
