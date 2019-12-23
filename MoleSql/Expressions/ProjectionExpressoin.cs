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
