/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MoleSql.Expressions;

namespace MoleSql.Translators
{
    /// <summary>
    /// This class visits an expression and will evaluate all parts of it that already can be evaluated.
    /// That is, all parts that don't refer to an parameter expression (hence requiring the database to be queried first).
    /// </summary>
    static class LocalEvaluator
    {
        class Nominator : DbExpressionVisitor
        {
            readonly HashSet<Expression> candidates = new HashSet<Expression>();
            readonly Func<Expression, bool> isEvaluatable;
            bool canBeEvaluated = true;
            bool nestedCantBeEvaluated;
            int nestedCompoundExpression;

            Nominator(Func<Expression, bool> isEvaluatable)
            {
                this.isEvaluatable = isEvaluatable;
            }

            public override Expression Visit(Expression node)
            {
                if (node == null) return null;

                bool oldCanBeEvaluated = canBeEvaluated;
                canBeEvaluated = true;
                base.Visit(node);
                canBeEvaluated &= isEvaluatable(node);
                if (canBeEvaluated && nestedCompoundExpression == 0) candidates.Add(node);
                if (!canBeEvaluated) nestedCantBeEvaluated = true;
                canBeEvaluated &= oldCanBeEvaluated;
                return node;
            }
            protected override Expression VisitMemberInit(MemberInitExpression node)
            {
                nestedCantBeEvaluated = false;
                nestedCompoundExpression += 1;
                var result = base.VisitMemberInit(node);
                if (nestedCantBeEvaluated) canBeEvaluated = false;
                nestedCompoundExpression -= 1;
                if (nestedCompoundExpression == 0 && canBeEvaluated) candidates.Add(node);
                return result;
            }

            internal static HashSet<Expression> Nominate(Expression expression, Func<Expression, bool> canBeEvaluated)
            {
                var nominator = new Nominator(canBeEvaluated);
                nominator.Visit(expression);
                return nominator.candidates;
            }
        }
        class Evaluator : DbExpressionVisitor
        {
            readonly HashSet<Expression> candidates;

            Evaluator(HashSet<Expression> candidates)
            {
                this.candidates = candidates;
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

            internal static Expression Evaluate(Expression expression, HashSet<Expression> nominees) => new Evaluator(nominees).Visit(expression);
        }
        /// <summary>
        /// Traverses the given <paramref name="expression"/> and evaluates all parts of it that can be
        /// evaluated locally without querying the database. That means that all constant expressions,
        /// local method calls, calculations etc. are evaluated and represented in the final expression tree
        /// as primitive constants.<br/>
        /// This needs to be done before translating the expression tree into SQL.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to evaluate.</param>
        /// <param name="canBeEvaluated">A function to determine if an expression can be evaluated locally.</param>
        /// <returns>The resulting <see cref="Expression"/> that no longer contains anything else but parameter or primitive constant expressions.</returns>
        internal static Expression EvaluateLocally(this Expression expression, Func<Expression, bool> canBeEvaluated) =>
            Evaluator.Evaluate(expression, Nominator.Nominate(expression, canBeEvaluated));
    }
}
