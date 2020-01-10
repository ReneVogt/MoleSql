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
    sealed class ProjectionExpression : Expression
    {
        internal SelectExpression Source { get; }
        internal Expression Projector { get; }
        internal bool IsTopLevelAggregation { get; }

        public override Type Type { get; }
        public override ExpressionType NodeType { get; }

        internal ProjectionExpression(SelectExpression source, Expression projector, bool isTopLevelAggregation = false)
        {
            Source = source;
            Projector = projector;
            IsTopLevelAggregation = isTopLevelAggregation;
            Type = source.Type;
            NodeType = (ExpressionType)DbExpressionType.Projection;
        }

        [ExcludeFromCodeCoverage]
        public override string ToString() => $"Projection: Source: ({Source}) Projector: ({Projector}){(IsTopLevelAggregation ? " TLA" : string.Empty)}";
    }
}
