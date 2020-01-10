/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace MoleSql.Expressions
{
    sealed class TableExpression : Expression
    {
        internal string Alias { get; }
        internal string Name { get; }

        public override Type Type { get; }
        public override ExpressionType NodeType { get; }

        internal TableExpression(Type type, string alias, string name)
        {
            Alias = alias;
            Name = name;
            Type = type;
            NodeType = (ExpressionType)DbExpressionType.Table;
        }

        [ExcludeFromCodeCoverage]
        public override string ToString() => $"Table '{Name}' AS '{Alias}'";
    }
}
