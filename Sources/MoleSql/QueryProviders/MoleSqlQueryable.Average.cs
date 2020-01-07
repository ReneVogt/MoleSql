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

namespace MoleSql.QueryProviders
{
    /// <summary>
    /// Provides special operators IQueryables based on <see cref="MoleSqlQueryProvider"/> instances.
    /// </summary>
    public static partial class MoleSqlQueryable
    {
        public static async Task<double> AverageAsync(this IQueryable<int> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MaxAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double>(expression, cancellationToken).ConfigureAwait(false);
        }
        public static async Task<double?> AverageAsync(this IQueryable<int?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MaxAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double?>(expression, cancellationToken).ConfigureAwait(false);
        }
        public static async Task<double> AverageAsync(this IQueryable<long> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MaxAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double>(expression, cancellationToken).ConfigureAwait(false);
        }
        public static async Task<double?> AverageAsync(this IQueryable<long?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MaxAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double?>(expression, cancellationToken).ConfigureAwait(false);
        }
        public static async Task<float> AverageAsync(this IQueryable<float> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MaxAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<float>(expression, cancellationToken).ConfigureAwait(false);
        }
        public static async Task<float?> AverageAsync(this IQueryable<float?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MaxAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<float?>(expression, cancellationToken).ConfigureAwait(false);
        }
        public static async Task<double> AverageAsync(this IQueryable<double> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MaxAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double>(expression, cancellationToken).ConfigureAwait(false);
        }
        public static async Task<double?> AverageAsync(this IQueryable<double?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MaxAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double?>(expression, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<decimal> AverageAsync(this IQueryable<decimal> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MaxAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<decimal>(expression, cancellationToken).ConfigureAwait(false);
        }
        public static async Task<decimal?> AverageAsync(this IQueryable<decimal?> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MaxAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<decimal?>(expression, cancellationToken).ConfigureAwait(false);
        }
        public static async Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MaxAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double>(expression, cancellationToken).ConfigureAwait(false);
        }
        public static async Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MaxAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double?>(expression, cancellationToken).ConfigureAwait(false);
        }
        public static async Task<float> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MaxAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<float>(expression, cancellationToken).ConfigureAwait(false);
        }
        public static async Task<float?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MaxAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<float?>(expression, cancellationToken).ConfigureAwait(false);
        }
        public static async Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MaxAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double>(expression, cancellationToken).ConfigureAwait(false);
        }
        public static async Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MaxAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double?>(expression, cancellationToken).ConfigureAwait(false);
        }
        public static async Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MaxAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double>(expression, cancellationToken).ConfigureAwait(false);
        }
        public static async Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MaxAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<double?>(expression, cancellationToken).ConfigureAwait(false);
        }
        public static async Task<decimal> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MaxAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<decimal>(expression, cancellationToken).ConfigureAwait(false);
        }
        public static async Task<decimal?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MaxAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var methodInfo = GetMethodInfo(AverageAsync, source, selector, cancellationToken);
            Expression expression = Expression.Call(null, methodInfo,
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            return await provider.ExecuteAggregateAsync<decimal?>(expression, cancellationToken).ConfigureAwait(false);
        }
    }
}
