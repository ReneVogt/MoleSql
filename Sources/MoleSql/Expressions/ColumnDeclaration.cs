/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */

using System.Linq.Expressions;

namespace MoleSql.Expressions
{
    sealed class ColumnDeclaration
    {
        internal string Name { get; }
        internal Expression Expression { get; }

        public ColumnDeclaration(string name, Expression expression)
        {
            Name = name;
            Expression = expression;
        }

        public override string ToString() => $"ColumnDeclaration '{Name}': {Expression}";
    }
}
