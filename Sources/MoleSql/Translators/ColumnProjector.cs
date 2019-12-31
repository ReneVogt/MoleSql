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
        string[] aliasesExisting;
        string aliasNew;
        int column;

        internal (Expression projector, ReadOnlyCollection<ColumnDeclaration> columns) ProjectColumns(Expression expression, string newAlias, params string[] existingAliases)
        {
            map = new Dictionary<ColumnExpression, ColumnExpression>(); 
            columns = new List<ColumnDeclaration>(); 
            columnNames = new HashSet<string>(); 
            aliasNew = newAlias;
            aliasesExisting = existingAliases;
            candidates = nominator.Nominate(expression);
            return (Visit(expression), columns.AsReadOnly());
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
            if (aliasesExisting.Contains(columnExpression.Alias))
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
