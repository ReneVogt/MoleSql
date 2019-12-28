/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace MoleSql.Expressions
{
    [ExcludeFromCodeCoverage]
    static class ExpressionExtensions
    {
        /// <summary>
        /// Removes the quote expressions around lambda expressions.
        /// </summary>
        /// <param name="expression">The expression to remove outer quotes from.</param>
        /// <returns>The "naked" lambda expression without outer quotes.</returns>
        internal static Expression StripQuotes(this Expression expression)
        { 
            while (expression?.NodeType == ExpressionType.Quote)
                expression = ((UnaryExpression)expression).Operand;

            return expression;
        }
    }
}
