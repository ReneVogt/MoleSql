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

namespace MoleSql.Extensions
{
    public static partial class MoleSqlQueryable
    {
        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Int32"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<Double> AverageAsync([NotNull] this IQueryable<Int32> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Double>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Int32"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<Double?> AverageAsync([NotNull] this IQueryable<Int32?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Double?>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Int64"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<Double> AverageAsync([NotNull] this IQueryable<Int64> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Double>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Int64"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<Double?> AverageAsync([NotNull] this IQueryable<Int64?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Double?>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Single"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<Single> AverageAsync([NotNull] this IQueryable<Single> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Single>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Single"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<Single?> AverageAsync([NotNull] this IQueryable<Single?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Single?>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Double"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<Double> AverageAsync([NotNull] this IQueryable<Double> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Double>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Double"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<Double?> AverageAsync([NotNull] this IQueryable<Double?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Double?>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Decimal"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<Decimal> AverageAsync([NotNull] this IQueryable<Decimal> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Decimal>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Decimal"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<Decimal?> AverageAsync([NotNull] this IQueryable<Decimal?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Decimal?>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Int32"/> values
        /// that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<Double> AverageAsync<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] Expression<Func<TSource, Int32>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Double>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Int32"/> values
        /// that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<Double?> AverageAsync<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] Expression<Func<TSource, Int32?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Double?>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Single"/> values
        /// that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<Single> AverageAsync<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] Expression<Func<TSource, Single>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Single>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Single"/> values
        /// that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<Single?> AverageAsync<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] Expression<Func<TSource, Single?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Single?>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Int64"/> values
        /// that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<Double> AverageAsync<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] Expression<Func<TSource, Int64>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Double>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Int64"/> values
        /// that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<Double?> AverageAsync<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] Expression<Func<TSource, Int64?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Double?>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Double"/> values
        /// that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<Double> AverageAsync<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] Expression<Func<TSource, Double>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Double>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Double"/> values
        /// that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<Double?> AverageAsync<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] Expression<Func<TSource, Double?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Double?>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Decimal"/> values
        /// that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<Decimal> AverageAsync<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] Expression<Func<TSource, Decimal>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Decimal>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Decimal"/> values
        /// that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="QueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<Decimal?> AverageAsync<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] Expression<Func<TSource, Decimal?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (!(source.Provider is QueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(QueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<Decimal?>(expression, cancellationToken).ConfigureAwait(false);
        }
    }
}
