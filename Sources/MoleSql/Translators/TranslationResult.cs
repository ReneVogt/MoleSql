/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace MoleSql.Translators
{
    /// <summary>
    /// Represents the result of a complete translation of an expreesion tree into an SQL query.
    /// Includes the sql query text (<see cref="CommandText"/>), a lambda expression to project the results
    /// into objects (<see cref="Projection"/> and a list of parameters for the sql query (<see cref="Parameters"/>).
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal struct TranslationResult
    {
        internal string CommandText { get; set; }
        internal LambdaExpression Projection { get; set; }
        internal List<(string name, object value)> Parameters { get; set; }
        internal TranslationResult(string commandText, LambdaExpression projection, List<(string name, object value)> parameters)
        {
            CommandText = commandText;
            Projection = projection;
            Parameters = parameters;
        }
        internal void Deconstruct(out string commmandText, out LambdaExpression projection, out List<(string name, object value)> parameters)
        {
            commmandText = CommandText;
            projection = Projection;
            parameters = Parameters;
        }
    }
}
