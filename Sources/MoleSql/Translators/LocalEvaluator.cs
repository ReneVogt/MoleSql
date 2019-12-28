/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MoleSql.Translators
{
    /// <summary>
    /// This class visits an expression and will evaluate all parts of it that already can be evaluated.
    /// That is, all parts that don't refer to an parameter expression (hence requiring the database to be queried first).
    /// </summary>
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
        /// <summary>
        /// Traverses the given <paramref name="expression"/> and evaluates all parts of it that can be
        /// evaluated locally without querying the database. That means that all constant expressions,
        /// local method calls, calculations etc. are evaluated and represented in the final expression tree
        /// as primitive constants.<br/>
        /// This needs to be done before translating the expression tree into SQL.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to evaluate.</param>
        /// <returns>The resulting <see cref="Expression"/> that no longer contains anything else but parameter or primitive constant expressions.</returns>
        internal static Expression EvaluateLocally(this Expression expression) =>
            new Evaluator().Evaluate(expression, new Nominator().Nominate(expression));
    }
}
