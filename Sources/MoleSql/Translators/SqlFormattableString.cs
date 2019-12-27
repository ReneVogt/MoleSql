/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Inspired by Jon Skeet (https://github.com/jskeet/DemoCode/blob/master/Abusing%20CSharp/Code/StringInterpolation/ParameterizedSql.cs).
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace MoleSql.Translators
{
    /// <summary>
    /// Provides the extension method <see cref="CreateParameterizedCommand"/> to create
    /// parameterized SQL queries (<see cref="SqlCommand"/>) from formattable strings (<see cref="FormattableString"/>).
    /// </summary>
    static class SqlFormattableString
    {
        /// <summary>
        /// Holds a sql format parameter.
        /// </summary>
        /// <remarks>
        /// The format parameter is converted into an <see cref="SqlParameter"/> with the
        /// corresponding parameter name ('@pXX'). The second parameter of the constructor
        /// is used to count the parameters.
        /// If the argument is an <see cref="IEnumerable"/> (but not <see cref="string"/>),
        /// multiple SqlParameters are created and later formated with braces ("(@p0, @p1...)").
        /// </remarks>
        sealed class FormatCapturingParameter : IFormattable
        {
            readonly SqlParameter parameter;
            readonly SqlParameter[] parameters;

            internal IEnumerable<SqlParameter> Parameters
            {
                get
                {
                    if (parameter != null) yield return parameter;
                    if (parameters == null) yield break;
                    foreach (var p in parameters) yield return p;
                }
            }

            internal FormatCapturingParameter(object value, ref int parameterCount)
            {
                if (!(value is IEnumerable enumerable) || value is string)
                {
                    parameter = new SqlParameter(FormattableString.Invariant($"@p{parameterCount++}"), value);
                    return;
                }

                int paramCount = parameterCount;
                parameters = enumerable.Cast<object>()
                                       .Select(o => new SqlParameter(FormattableString.Invariant($"@p{paramCount++}"), o))
                                       .ToArray();
                parameterCount = paramCount;
            }
            public string ToString(string format, IFormatProvider formatProvider)
            {
                if (Enum.TryParse(format, true, out SqlDbType type))
                {
                    if (parameters != null)
                        foreach (var p in parameters)
                            p.SqlDbType = type;
                    if (parameter != null)
                        parameter.SqlDbType = type;
                }

                if (parameter != null)
                    return parameter.ParameterName;

                Debug.Assert(parameters != null);
                return "(" + string.Join(", ", parameters.Select(p => p.ParameterName)) + ")";
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2100", Justification = "This is what this is all about.")]
        internal static SqlCommand CreateParameterizedCommand(this SqlConnection connection, FormattableString formattableString)
        {
            int parameters = 0;
            var formatArguments = formattableString.GetArguments()
                                       .Select(arg => new FormatCapturingParameter(arg, ref parameters))
                                       .ToArray();
            string sql = string.Format(CultureInfo.InvariantCulture, formattableString.Format, formatArguments.Cast<object>().ToArray());
            var command = new SqlCommand(sql, connection);
            command.Parameters.AddRange(formatArguments.SelectMany(arg => arg.Parameters).ToArray());
            return command;
        }
    }
}
