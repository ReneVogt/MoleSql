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
    sealed class JoinExpression : Expression
    {
        internal JoinType JoinType { get; }
        internal Expression Left { get; }
        internal Expression Right { get; }
#pragma warning disable 109 // it "hides" a static method
        internal new Expression Condition { get; }
#pragma warning restore 109

        public override Type Type { get; }
        public override ExpressionType NodeType { get; }

        internal JoinExpression(Type type, JoinType joinType, Expression left, Expression right, Expression condition)
        {
            JoinType = joinType;
            Left = left;
            Right = right;
            Condition = condition;
            Type = type;
            NodeType = (ExpressionType)DbExpressionType.Join;
        }

        public override string ToString() => $"{JoinType} Left: ({Left}) Right: ({Right}) Condition: ({Condition})";
    }
}
