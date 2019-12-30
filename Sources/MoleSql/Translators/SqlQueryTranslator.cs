/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using MoleSql.Expressions;

namespace MoleSql.Translators
{
    /// <summary>
    /// Provides a static entry point (<see cref="Translate"/>) to translate expression trees
    /// into sql queries.
    /// </summary>
    [ExcludeFromCodeCoverage]
    static class SqlQueryTranslator
    {
        /// <summary>
        /// Translates the given <paramref name="expression"/> into an SQL query.
        /// </summary>
        /// <param name="expression">The expression tree to translate.</param>
        /// <returns>A <see cref="TranslationResult"/> holding the information required to build a <see cref="SqlCommand"/> to execute the query.</returns>
        internal static TranslationResult Translate(Expression expression)
        {
            var projectionExpression = expression as ProjectionExpression ?? QueryBinder.Bind(expression.EvaluateLocally());
            (string commandText, var parameters) = SqlQueryFormatter.Format(projectionExpression.Source);
            LambdaExpression projector = ProjectionBuilder.Build(projectionExpression.Projector, projectionExpression.Source.Alias);

            return new TranslationResult(commandText, projector, parameters);
        }
    }
}
