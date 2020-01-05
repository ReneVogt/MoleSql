/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MoleSql
{
    /// <summary>
    /// Provides extension methods for the <see cref="MoleSqlDataContext"/>.<br/>
    /// The "Execute..." methods with format strings and parameters are implemented here
    /// to make the compiler chooose the instance methods with <see cref="FormattableString"/>
    /// parameters over these versions with string parameters.
    /// </summary>
    public static class MoleSqlDataContextExtensions
    {
        /// <summary>
        /// Executes the given query and returns a sequence of results.
        /// </summary>
        /// <typeparam name="T">The result type of the queried enumeration.</typeparam>
        /// <param name="context">The <see cref="MoleSqlDataContext"/> this extension should work on.</param>
        /// <param name="query">The format string for the sql command to execute.</param>
        /// <param name="parameters">The query parameters.</param>
        /// <returns>An enumerator for the query results.</returns>
        [ExcludeFromCodeCoverage]
        [StringFormatMethod(formatParameterName: "query")]
        public static IEnumerable<T> ExecuteQuery<T>([NotNull] this MoleSqlDataContext context, string query, params object[] parameters) where T : class, new()
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return context.ExecuteQuery<T>(FormattableStringFactory.Create(query, parameters));
        }
        /// <summary>
        /// Executes the given query and return a sequence of dynamic instances.
        /// </summary>
        /// <param name="context">The <see cref="MoleSqlDataContext"/> this extension should work on.</param>
        /// <param name="query">The format string for the sql command to execute.</param>
        /// <param name="parameters">The query parameters.</param>
        /// <returns>An enumerator for the query results. Those will be dynamic objects.</returns>
        [ExcludeFromCodeCoverage]
        [StringFormatMethod(formatParameterName: "query")]
        public static IEnumerable ExecuteQuery([NotNull] this MoleSqlDataContext context, string query, params object[] parameters)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            return context.ExecuteQuery(FormattableStringFactory.Create(query, parameters));
        }
        /// <summary>
        /// Executes the given query or command and returns the number of affected rows.
        /// </summary>
        /// <param name="context">The <see cref="MoleSqlDataContext"/> this extension should work on.</param>
        /// <param name="query">The format string for the sql command to execute.</param>
        /// <param name="parameters">The query parameters.</param>
        /// <returns>The number of affected rows.</returns>
        [ExcludeFromCodeCoverage]
        [StringFormatMethod(formatParameterName: "query")]
        public static int ExecuteNonQuery([NotNull] this MoleSqlDataContext context, string query, params object[] parameters)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            return context.ExecuteNonQuery(FormattableStringFactory.Create(query, parameters));
        }
        /// <summary>
        /// Executes the given query or command asynchronously and returns a task that on completion returns the number of affected rows.
        /// </summary>
        /// <param name="context">The <see cref="MoleSqlDataContext"/> this extension should work on.</param>
        /// <param name="query">The format string for the sql command to execute.</param>
        /// <param name="parameters">The query parameters.</param>
        /// <returns>The number of affected rows.</returns>
        [ExcludeFromCodeCoverage]
        [StringFormatMethod(formatParameterName: "query")]
        public static Task<int> ExecuteNonQueryAsync([NotNull] this MoleSqlDataContext context, string query, params object[] parameters) =>
            context.ExecuteNonQueryAsync(CancellationToken.None, query, parameters);
        /// <summary>
        /// Executes the given query or command asynchronously and returns a task that on completion returns the number of affected rows.
        /// </summary>
        /// <param name="context">The <see cref="MoleSqlDataContext"/> this extension should work on.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation</param>
        /// <param name="query">The format string for the sql command to execute.</param>
        /// <param name="parameters">The query parameters.</param>
        /// <returns>The number of affected rows.</returns>
        [ExcludeFromCodeCoverage]
        [StringFormatMethod(formatParameterName: "query")]
        [SuppressMessage("Design", "CA1068:CancellationToken-Parameter müssen zuletzt aufgeführt werden", Justification = "This gets awkward with format params.")]
        public static Task<int> ExecuteNonQueryAsync([NotNull] this MoleSqlDataContext context, CancellationToken cancellationToken, string query, params object[] parameters)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            return context.ExecuteNonQueryAsync(FormattableStringFactory.Create(query, parameters), cancellationToken);
        }
    }
}
