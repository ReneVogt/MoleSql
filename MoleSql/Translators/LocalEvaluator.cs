using System.Collections.Generic;
using System.Linq.Expressions;

namespace MoleSql.Translators
{
    static class LocalEvaluator
    {
        class Nominator : ExpressionVisitor
        {
            HashSet<Expression> candidates;
            bool canBeEvaluated;

            internal HashSet<Expression> Nominate(Expression expression)
            {
                candidates = new HashSet<Expression>();
                canBeEvaluated = true;
                Visit(expression);
                return candidates;
            }

            public override Expression Visit(Expression node)
            {
                if (node == null) return null;

                bool oldCanBeEvaluated = canBeEvaluated;
                canBeEvaluated = true;
                base.Visit(node);
                canBeEvaluated &= CanBeEvaluated(node);
                if (canBeEvaluated) candidates.Add(node);
                canBeEvaluated &= oldCanBeEvaluated;
                return node;
            }

            static bool CanBeEvaluated(Expression node) => node.NodeType != ExpressionType.Parameter;
        }
        class Evaluator : ExpressionVisitor
        {
            HashSet<Expression> candidates;
            internal Expression Evaluate(Expression expression, HashSet<Expression> nominees)
            {
                candidates = nominees;
                return Visit(expression);
            }
            public override Expression Visit(Expression node) => node == null ? null : candidates.Contains(node) ? Evaluate(node) : base.Visit(node);

            static Expression Evaluate(Expression expression)
            {
                if (expression.NodeType == ExpressionType.Constant)
                    return expression;

                var lambda = Expression.Lambda(expression);
                var function = lambda.Compile();
                return Expression.Constant(function.DynamicInvoke(null), expression.Type);
            }
        }
        internal static Expression EvaluateLocally(this Expression expression) =>
            new Evaluator().Evaluate(expression, new Nominator().Nominate(expression));
    }
}
