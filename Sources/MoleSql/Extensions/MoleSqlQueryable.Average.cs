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
using MoleSql.QueryProviders;

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
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="MoleSqlQueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<double> AverageAsync(this IQueryable<int> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Int32"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="MoleSqlQueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<double?> AverageAsync(this IQueryable<int?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double?>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Int64"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="MoleSqlQueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<double> AverageAsync(this IQueryable<long> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Int64"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="MoleSqlQueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<double?> AverageAsync(this IQueryable<long?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double?>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Single"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="MoleSqlQueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<float> AverageAsync(this IQueryable<float> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<float>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Single"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="MoleSqlQueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<float?> AverageAsync(this IQueryable<float?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<float?>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Double"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="MoleSqlQueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<double> AverageAsync(this IQueryable<double> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Double"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="MoleSqlQueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<double?> AverageAsync(this IQueryable<double?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double?>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of <see cref="Decimal"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="MoleSqlQueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<decimal> AverageAsync(this IQueryable<decimal> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<decimal>(expression, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously computes the average of a sequence of nullable <see cref="Decimal"/> values.
        /// </summary>
        /// <param name="source">The input sequence.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>The average of the sequence values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> was <code>null</code>.</exception>
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="MoleSqlQueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<decimal?> AverageAsync(this IQueryable<decimal?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<decimal?>(expression, cancellationToken).ConfigureAwait(false);
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
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="MoleSqlQueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double>(expression, cancellationToken).ConfigureAwait(false);
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
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="MoleSqlQueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double?>(expression, cancellationToken).ConfigureAwait(false);
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
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="MoleSqlQueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<float> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<float>(expression, cancellationToken).ConfigureAwait(false);
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
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="MoleSqlQueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<float?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<float?>(expression, cancellationToken).ConfigureAwait(false);
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
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="MoleSqlQueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double>(expression, cancellationToken).ConfigureAwait(false);
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
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="MoleSqlQueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double?>(expression, cancellationToken).ConfigureAwait(false);
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
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="MoleSqlQueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double>(expression, cancellationToken).ConfigureAwait(false);
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
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="MoleSqlQueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double?>(expression, cancellationToken).ConfigureAwait(false);
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
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="MoleSqlQueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<decimal> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<decimal>(expression, cancellationToken).ConfigureAwait(false);
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
        /// <exception cref="NotSupportedException">This method can only be used with a <see cref="MoleSqlQueryProvider"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
        public static async Task<decimal?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(AverageAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<decimal?>(expression, cancellationToken).ConfigureAwait(false);
        }
    }
}
