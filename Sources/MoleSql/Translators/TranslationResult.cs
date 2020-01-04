﻿/*
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
        internal string CommandText { get; }
        internal LambdaExpression Projection { get; }
        internal List<(string name, object value)> Parameters { get; }
        internal LambdaExpression Aggregator { get; }
        internal TranslationResult(string commandText, LambdaExpression projection, List<(string name, object value)> parameters, LambdaExpression aggregator)
        {
            CommandText = commandText;
            Projection = projection;
            Parameters = parameters;
            Aggregator = aggregator;
        }
        internal void Deconstruct(out string commmandText, out LambdaExpression projection, out List<(string name, object value)> parameters, out LambdaExpression aggregator)
        {
            commmandText = CommandText;
            projection = Projection;
            parameters = Parameters;
            aggregator = Aggregator;
        }
    }
}
