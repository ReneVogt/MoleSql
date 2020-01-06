using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MoleSql.QueryProviders
{
    /// <summary>
    /// Provides special operators IQueryables based on <see cref="MoleSqlQueryProvider"/> instances.
    /// </summary>
    public static class MoleSqlQueryProviderExtensions
    {
        /// <summary>
        /// Lets the query provider execute the <paramref name="source"/> query asynchronously and stores the rows into a list.
        /// </summary>
        /// <typeparam name="T">The type of the resulting rows.</typeparam>
        /// <param name="source">The source query to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>A task that on completion returns a list of the resulting rows.</returns>
        public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
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
    }
}
