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
        /// Asynchronously returns the number of elements in the specified sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the input sequence.</typeparam>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The number of elements in the sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        /// <exception cref="OverflowException">The number of matching elements in the sequence is larger than <see cref="Int32.MaxValue"/>.</exception>
        public static async Task<Int32> CountAsync<TSource>([NotNull] this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw nameof(CountAsync).DoesNotSupportDifferentQueryProvider();

            var methodInfo = GetMethodInfo(CountAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Int32>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously returns the number of elements in the specified sequence that satisfies a condition.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the input sequence.</typeparam>
        /// <param name="source">The input sequence.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The number of elements in the sequence that satisfies the condition in the predicate function.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> were <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        /// <exception cref="OverflowException">The number of matching elements in the sequence is larger than <see cref="Int32.MaxValue"/>.</exception>
        public static async Task<Int32> CountAsync<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (!(source.Provider is QueryProvider provider))
                throw nameof(CountAsync).DoesNotSupportDifferentQueryProvider();

            var methodInfo = GetMethodInfo(CountAsync, source, predicate, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(predicate), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Int32>(expression, cancellationToken).ConfigureAwait(false);
        }
    }
}
