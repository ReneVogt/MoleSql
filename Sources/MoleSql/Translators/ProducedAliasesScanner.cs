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
    sealed class ProducedAliasesScanner : DbExpressionVisitor
    {
        readonly HashSet<string> aliases = new HashSet<string>();

        ProducedAliasesScanner() { }

        protected override Expression VisitSelect(SelectExpression selectExpression)
        {
            aliases.Add(selectExpression.Alias);
            return selectExpression;
        }

        protected override Expression VisitTable(TableExpression table)
        {
            aliases.Add(table.Alias);
            return table;
        }

        internal static HashSet<string> Gather(Expression source)
        {
            var scanner = new ProducedAliasesScanner();
            scanner.Visit(source);
            return scanner.aliases;
        }

    }
}
