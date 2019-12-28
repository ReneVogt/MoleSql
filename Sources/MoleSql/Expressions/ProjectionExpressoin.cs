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

        public override Type Type { get; }
        public override ExpressionType NodeType { get; }

        internal ProjectionExpression(SelectExpression source, Expression projector)
        {
            Source = source;
            Projector = projector;
            Type = Projector.Type;
            NodeType = (ExpressionType)DbExpressionType.Projection;
        }
    }
}
