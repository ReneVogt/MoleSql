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
    sealed class ProjectionExpression : Expression
    {
        internal SelectExpression Source { get; }
        internal Expression Projector { get; }
        internal LambdaExpression Aggregator { get; }
        internal LambdaExpression AggregatorAsync { get; }

        public override Type Type { get; }
        public override ExpressionType NodeType { get; }

        internal ProjectionExpression(SelectExpression source, Expression projector, LambdaExpression aggregator = null, LambdaExpression aggregatorAsync = null)
        {
            Source = source;
            Projector = projector;
            Type = source.Type;
            Aggregator = aggregator;
            AggregatorAsync = aggregatorAsync;
            NodeType = (ExpressionType)DbExpressionType.Projection;
        }

        public override string ToString() => $"Projection: Source: ({Source}) Projector: ({Projector})";

    }
}
