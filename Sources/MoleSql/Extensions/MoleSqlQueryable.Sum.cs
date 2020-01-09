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

namespace MoleSql.Extensions
{
    public static partial class MoleSqlQueryable
    {
        /// <summary>
        /// Asynchronously computes the sum of the sequence of <see cref="Int32"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <code>null</code>.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Int32.MaxValue"/>.</exception>
        public static async Task<Int32> SumAsync(this IQueryable<Int32> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(SumAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(SumAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Int32>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the sum of the sequence of nullable <see cref="Int32"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <code>null</code>.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Int32.MaxValue"/>.</exception>
        public static async Task<Int32?> SumAsync(this IQueryable<Int32?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(SumAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(SumAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Int32?>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the sum of the sequence of <see cref="Int64"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <code>null</code>.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Int64.MaxValue"/>.</exception>
        public static async Task<Int64> SumAsync(this IQueryable<Int64> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(SumAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(SumAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Int64>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the sum of the sequence of nullable <see cref="Int64"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <code>null</code>.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Int64.MaxValue"/>.</exception>
        public static async Task<Int64?> SumAsync(this IQueryable<Int64?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(SumAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(SumAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Int64?>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the sum of the sequence of <see cref="Single"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <code>null</code>.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Single.MaxValue"/>.</exception>
        public static async Task<Single> SumAsync(this IQueryable<Single> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(SumAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(SumAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Single>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the sum of the sequence of nullable <see cref="Single"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <code>null</code>.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Single.MaxValue"/>.</exception>
        public static async Task<Single?> SumAsync(this IQueryable<Single?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(SumAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(SumAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Single?>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the sum of the sequence of <see cref="Double"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <code>null</code>.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Double.MaxValue"/>.</exception>
        public static async Task<Double> SumAsync(this IQueryable<Double> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(SumAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(SumAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Double>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the sum of the sequence of nullable <see cref="Double"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <code>null</code>.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Double.MaxValue"/>.</exception>
        public static async Task<Double?> SumAsync(this IQueryable<Double?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(SumAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(SumAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Double?>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the sum of the sequence of <see cref="Decimal"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <code>null</code>.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Decimal.MaxValue"/>.</exception>
        public static async Task<Decimal> SumAsync(this IQueryable<Decimal> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(SumAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(SumAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Decimal>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the sum of the sequence of nullable <see cref="Decimal"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <code>null</code>.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Decimal.MaxValue"/>.</exception>
        public static async Task<Decimal?> SumAsync(this IQueryable<Decimal?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(SumAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(SumAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Decimal?>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the sum of the sequence of <see cref="Int32"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> are <code>null</code>.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Int32.MaxValue"/>.</exception>
        public static async Task<Int32> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, Int32>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(SumAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(SumAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, selector, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Int32>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the sum of the sequence of nullable <see cref="Int32"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> are <code>null</code>.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Int32.MaxValue"/>.</exception>
        public static async Task<Int32?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, Int32?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(SumAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(SumAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, selector, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Int32?>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the sum of the sequence of <see cref="Int64"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> are <code>null</code>.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Int64.MaxValue"/>.</exception>
        public static async Task<Int64> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, Int64>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(SumAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(SumAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, selector, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Int64>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the sum of the sequence of nullable <see cref="Int64"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> are <code>null</code>.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Int64.MaxValue"/>.</exception>
        public static async Task<Int64?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, Int64?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(SumAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(SumAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, selector, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Int64?>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the sum of the sequence of <see cref="Single"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> are <code>null</code>.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Single.MaxValue"/>.</exception>
        public static async Task<Single> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, Single>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(SumAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(SumAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, selector, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Single>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the sum of the sequence of nullable <see cref="Single"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> are <code>null</code>.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Single.MaxValue"/>.</exception>
        public static async Task<Single?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, Single?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(SumAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(SumAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, selector, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Single?>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the sum of the sequence of <see cref="Double"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> are <code>null</code>.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Double.MaxValue"/>.</exception>
        public static async Task<Double> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, Double>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(SumAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(SumAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, selector, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Double>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the sum of the sequence of nullable <see cref="Double"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> are <code>null</code>.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Double.MaxValue"/>.</exception>
        public static async Task<Double?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, Double?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(SumAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(SumAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, selector, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Double?>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the sum of the sequence of <see cref="Decimal"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> are <code>null</code>.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Decimal.MaxValue"/>.</exception>
        public static async Task<Decimal> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, Decimal>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(SumAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(SumAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, selector, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Decimal>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the sum of the sequence of nullable <see cref="Decimal"/> values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values of type <typeparamref name="TSource"/>.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> are <code>null</code>.</exception>
        /// <exception cref="OverflowException">The sum is larger than <see cref="Decimal.MaxValue"/>.</exception>
        public static async Task<Decimal?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, Decimal?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(SumAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(SumAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, selector, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Decimal?>(expression, cancellationToken).ConfigureAwait(false);
        }
    }
}
