﻿/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using MoleSql.Expressions;

namespace MoleSql.Translators
{
    /// <summary>
    /// Provides a static entry point (<see cref="Translate"/>) to translate expression trees
    /// into sql queries.
    /// </summary>
    static class SqlQueryTranslator
    {
        /// <summary>
        /// Translates the given <paramref name="expression"/> into an SQL query.
        /// </summary>
        /// <param name="expression">The expression tree to translate.</param>
        /// <returns>A <see cref="TranslationResult"/> holding the information required to build a <see cref="SqlCommand"/> to execute the query.</returns>
        internal static TranslationResult Translate(Expression expression)
        {
            if (!(expression is ProjectionExpression projectionExpression))
            {
                expression = expression.EvaluateLocally();
                expression = QueryBinder.Bind(expression);
                expression = AggregateRewriter.Rewrite(expression);
                expression = OrderByRewriter.Rewrite((ProjectionExpression)expression);
                expression = UnusedColumnsRemover.Remove(expression);
                expression = RedundantSubQueryRemover.Remove(expression);
                projectionExpression = (ProjectionExpression)expression;
            }

            (string commandText, var parameters) = SqlQueryFormatter.Format(projectionExpression.Source);
            var columns = projectionExpression.Source.Columns.Select(c => c.Name).ToArray();
            LambdaExpression projector = ProjectionBuilder.Build(projectionExpression.Projector, projectionExpression.Source.Alias, columns);

            return new TranslationResult(commandText, projector, parameters, projectionExpression.Aggregator, projectionExpression.AggregatorAsync);
        }
    }
}
