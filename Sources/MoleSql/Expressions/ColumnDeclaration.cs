/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace MoleSql.Expressions
{
    [ExcludeFromCodeCoverage]
    sealed class ColumnDeclaration
    {
        internal string Name { get; }
        internal Expression Expression { get; }

        public ColumnDeclaration(string name, Expression expression)
        {
            Name = name;
            Expression = expression;
        }
    }
}
