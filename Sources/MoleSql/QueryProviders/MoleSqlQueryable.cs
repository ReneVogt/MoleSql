/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MoleSql.QueryProviders
{
    /// <summary>
    /// Provides special operators IQueryables based on <see cref="MoleSqlQueryProvider"/> instances.
    /// </summary>
    public static class MoleSqlQueryable
    {
        static readonly MethodInfo maxWithSelector = typeof(MoleSqlQueryable)
                                                     .GetMethods(BindingFlags.Static | BindingFlags.Public)
                                                     .Single(m => m.Name == nameof(MaxAsync) && m.GetParameters().Length == 3);
        static readonly MethodInfo maxWithoutSelector = typeof(MoleSqlQueryable)
                                                        .GetMethods(BindingFlags.Static | BindingFlags.Public)
                                                        .Single(m => m.Name == nameof(MaxAsync) && m.GetParameters().Length == 2);
        static readonly MethodInfo minWithSelector = typeof(MoleSqlQueryable)
                                                     .GetMethods(BindingFlags.Static | BindingFlags.Public)
                                                     .Single(m => m.Name == nameof(MinAsync) && m.GetParameters().Length == 3);
        static readonly MethodInfo minWithoutSelector = typeof(MoleSqlQueryable)
                                                        .GetMethods(BindingFlags.Static | BindingFlags.Public)
                                                        .Single(m => m.Name == nameof(MinAsync) && m.GetParameters().Length == 2);

        /// <summary>
        /// Lets the query provider execute the <paramref name="source"/> query asynchronously and stores the rows into a list.
        /// </summary>
        /// <typeparam name="T">The type of the resulting rows.</typeparam>
        /// <param name="source">The source query to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>A task that on completion returns a list of the resulting rows.</returns>
        public static async Task<List<T>> ToListAsync<T>([NotNull] this IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(ToListAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            var query = (IAsyncEnumerable<T>)await provider.ExecuteAsync(source.Expression, cancellationToken).ConfigureAwait(false);
            List<T> list = new List<T>();
            
            await foreach(var element in query.WithCancellation(cancellationToken))
                list.Add(element);
            
            return list;
        }

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

            Expression expression = Expression.Call(null, minWithoutSelector.MakeGenericMethod(typeof(T)),
                                                    source.Expression,Expression.Constant(cancellationToken));

            var result = await provider.ExecuteAsync(expression, cancellationToken).ConfigureAwait(false);
            if (!(result is Task<T> task)) throw new InvalidOperationException("Unexpected result type.");
            return await task.ConfigureAwait(false);
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
        public static async Task<TResult> MinAsync<TSource, TResult>([NotNull] this IQueryable<TSource> source, [NotNull] Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MinAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            Expression expression = Expression.Call(null, minWithSelector.MakeGenericMethod(typeof(TSource), typeof(TResult)),
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            var result = await provider.ExecuteAsync(expression, cancellationToken).ConfigureAwait(false);
            if (!(result is Task<TResult> task)) throw new InvalidOperationException("Unexpected result type.");
            return await task.ConfigureAwait(false);
        }
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
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MaxAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");

            Expression expression = Expression.Call(null, maxWithoutSelector.MakeGenericMethod(typeof(T)),
                                                    source.Expression, Expression.Constant(cancellationToken));

            var result = await provider.ExecuteAsync(expression, cancellationToken).ConfigureAwait(false);
            if (!(result is Task<T> task)) throw new InvalidOperationException("Unexpected result type.");
            return await task.ConfigureAwait(false);
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
        public static async Task<TResult> MaxAsync<TSource, TResult>([NotNull] this IQueryable<TSource> source, [NotNull] Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source.Provider is MoleSqlQueryProvider provider))
                throw new NotSupportedException($"{nameof(MaxAsync)} only supports queries based on a {nameof(MoleSqlQueryProvider)}.");
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            Expression expression = Expression.Call(null, maxWithSelector.MakeGenericMethod(typeof(TSource), typeof(TResult)),
                                                    source.Expression, Expression.Quote(selector), Expression.Constant(cancellationToken));

            var result = await provider.ExecuteAsync(expression, cancellationToken).ConfigureAwait(false);
            if (!(result is Task<TResult> task)) throw new InvalidOperationException("Unexpected result type.");
            return await task.ConfigureAwait(false);
        }

        internal static async Task<T> SingleAsync<T>(IAsyncEnumerable<T> source, CancellationToken cancellationToken = default)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            
            IAsyncEnumerator<T> enumerator = null;
            try
            {
                enumerator = source.GetAsyncEnumerator(cancellationToken);
                if (!await enumerator.MoveNextAsync())
                    throw new InvalidOperationException("The sequence was empty.");
                T element = enumerator.Current;
                if (await enumerator.MoveNextAsync())
                    throw new InvalidOperationException("The sequence contained more than one element.");
                return element;
            }
            finally
            {
                enumerator?.DisposeAsync();
            }
        }
    }
}
