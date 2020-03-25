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
    sealed class AggregateExpression : Expression
    {
        internal AggregateType AggregateType { get; }
        internal Expression? Argument { get; }
        public override Type Type { get; }
        public override ExpressionType NodeType { get; }

        internal AggregateExpression(Type type, AggregateType aggregateType, Expression? argument)
        {
            AggregateType = aggregateType;
            Argument = argument;
            Type = type;
            NodeType = (ExpressionType)DbExpressionType.Aggregate;
        }

        [ExcludeFromCodeCoverage]
        public override string ToString() => $"{AggregateType} ({Argument})";
    }
}
