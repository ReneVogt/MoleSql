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
using MoleSql.QueryProviders;

namespace MoleSql.Extensions
{
    public static partial class MoleSqlQueryable
    {
        /// <summary>
        /// Asynchronously executes a query to determine the minimum of the input query (<paramref name="source"/>).
        /// </summary>
        /// <typeparam name="T">The type of the rows in the input query.</typeparam>
        /// <param name="source">The input query.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>A task that on completion returns the minimum value.</returns>
        public static async Task<T> MinAsync<T>([NotNull] this IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MinAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(MinAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<T>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously executes a query to determine the minimum of the input query (<paramref name="source"/>).
        /// </summary>
        /// <typeparam name="TSource">The type of the rows in the input query.</typeparam>
        /// <typeparam name="TResult">The result type of the projection by <paramref name="selector"/>.</typeparam>
        /// <param name="source">The input query.</param>
        /// <param name="selector">A projection to choose values from the input sequence for which the minimum should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>A task that on completion returns the minimum value.</returns>
        public static async Task<TResult> MinAsync<TSource, TResult>([NotNull] this IQueryable<TSource> source,
                                                                     [NotNull] Expression<Func<TSource, TResult>> selector,
                                                                     CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MinAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var methodInfo = GetMethodInfo(MinAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<TResult>(expression, cancellationToken).ConfigureAwait(false);
        }
    }
}
