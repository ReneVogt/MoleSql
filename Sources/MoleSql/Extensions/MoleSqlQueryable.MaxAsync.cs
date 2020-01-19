/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MoleSql.Exceptions;

namespace MoleSql.Extensions
{
    public static partial class MoleSqlQueryable
    {
        /// <summary>
        /// Asynchronously executes a query to determine the maximum of the input query (<paramref name="source"/>).
        /// </summary>
        /// <typeparam name="T">The type of the rows in the input query.</typeparam>
        /// <param name="source">The input query.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>A task that on completion returns the maximum value.</returns>
        public static async Task<T> MaxAsync<T>([NotNull] this IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw nameof(MaxAsync).DoesNotSupportDifferentQueryProvider();

            var methodInfo = GetMethodInfo(MaxAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<T>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously executes a query to determine the maximum of the input query (<paramref name="source"/>).
        /// </summary>
        /// <typeparam name="TSource">The type of the rows in the input query.</typeparam>
        /// <typeparam name="TResult">The result type of the projection by <paramref name="selector"/>.</typeparam>
        /// <param name="source">The input query.</param>
        /// <param name="selector">A projection to choose values from the input sequence for which the maximum should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>A task that on completion returns the maximum value.</returns>
        public static async Task<TResult> MaxAsync<TSource, TResult>([NotNull] this IQueryable<TSource> source,
                                                                     [NotNull] Expression<Func<TSource, TResult>> selector,
                                                                     CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw nameof(MaxAsync).DoesNotSupportDifferentQueryProvider();
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var methodInfo = GetMethodInfo(MaxAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<TResult>(expression, cancellationToken).ConfigureAwait(false);
        }
    }
}
