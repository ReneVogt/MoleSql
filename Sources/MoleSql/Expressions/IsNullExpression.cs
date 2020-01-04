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
    sealed class IsNullExpression : Expression
    {
        internal Expression Expression { get; }
        public override Type Type { get; }
        public override ExpressionType NodeType { get; }

        internal IsNullExpression(Expression expression)
        {
            Expression = expression;
            Type = typeof(bool);
            NodeType = (ExpressionType)DbExpressionType.IsNull;
        }

        public override string ToString() => $"IsNull ({Expression})";
    }
}
