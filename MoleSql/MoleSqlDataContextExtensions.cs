using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        public static IEnumerable<T> ExecuteQuery<T>([NotNull] this MoleSqlDataContext context, string query, params object[] parameters)
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
        public static int ExecuteNonQuery([NotNull] this MoleSqlDataContext context, string query, params object[] parameters)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            return context.ExecuteNonQuery(FormattableStringFactory.Create(query, parameters));
        }

    }
}
