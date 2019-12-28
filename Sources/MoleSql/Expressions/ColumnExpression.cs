/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System;
using System.Linq.Expressions;

namespace MoleSql.Expressions
{
    /// <summary>
    /// Represents a column expression in an SQL/CLR hybrid expression tree.
    /// </summary>
    sealed class ColumnExpression : Expression
    {
        internal string Alias { get; }
        internal string Name { get; }
        internal int Ordinal { get; }

        public override Type Type { get; }
        public override ExpressionType NodeType { get; }

        internal ColumnExpression(Type type, string alias, string name, int ordinal)
        {
            Alias = alias;
            Name = name;
            Ordinal = ordinal;
            Type = type;
            NodeType = (ExpressionType)DbExpressionType.Column;
        }
    }
}
