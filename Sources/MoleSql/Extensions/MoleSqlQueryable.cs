/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MoleSql.Extensions
{
    /// <summary>
    /// Provides special operators IQueryables based on <see cref="QueryProvider"/> instances.
    /// </summary>
    public static partial class MoleSqlQueryable
    {
        /// <summary>
        /// Lets the query provider execute the <paramref name="source"/> query asynchronously and stores the rows into a list.
        /// </summary>
        /// <typeparam name="T">The type of the resulting rows.</typeparam>
        /// <param name="source">The source query to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>A task that on completion returns a list of the resulting rows.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        public static Task<List<T>> ToListAsync<T>([NotNull] this IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(ToListAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            return provider.ExecuteAsync<T>(source.Expression, cancellationToken).ToListAsync(cancellationToken);
        }
        /// <summary>
        /// Asynchronously iterates the <paramref name="source"/> sequence and stores the elements in a list.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the input sequence.</typeparam>
        /// <param name="source">The asynchronous input sequence.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>A task that on completion returns a list of the sequence elements.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        public static async Task<List<T>> ToListAsync<T>([NotNull] this IAsyncEnumerable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            List<T> list = new List<T>();
            await foreach (var element in source.WithCancellation(cancellationToken))
                list.Add(element);

            return list;
        }
        /// <summary>
        /// Lets the query provider execute the <paramref name="source"/> query asynchronously and returns an asynchronous sequence of the results.
        /// </summary>
        /// <typeparam name="T">The type of the resulting rows.</typeparam>
        /// <param name="source">The source query to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>A sequence of rows that can be iterated asynchronously.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        public static IAsyncEnumerable<T> AsAsyncEnumerable<T>([NotNull] this IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(AsAsyncEnumerable)} only supports queries based on a {nameof(QueryProvider)}.");

            return provider.ExecuteAsync<T>(source.Expression, cancellationToken);
        }

#pragma warning disable IDE0060, CA1801 // Nicht verwendete Parameter entfernen
        // ReSharper disable UnusedParameter.Local
        [ExcludeFromCodeCoverage]
        static MethodInfo GetMethodInfo<T1, T2, T3>(Func<T1, T2, T3> f, T1 unused1, T2 unused2) => f.Method;
        [ExcludeFromCodeCoverage]
        static MethodInfo GetMethodInfo<T1, T2, T3, T4>(Func<T1, T2, T3, T4> f, T1 unused1, T2 unused2, T3 unused) => f.Method;
        // ReSharper restore UnusedParameter.Local
#pragma warning restore IDE0060, CA1801 // Nicht verwendete Parameter entfernen
    }
}
