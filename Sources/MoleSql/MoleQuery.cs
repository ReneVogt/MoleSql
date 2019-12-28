/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using MoleSql.QueryProviders;

namespace MoleSql
{
    /// <summary>
    /// The root element for MoleSql queries.
    /// </summary>
    /// <typeparam name="T">The type of the table.</typeparam>
    public class MoleQuery<T> : IOrderedQueryable<T>
    {
        readonly QueryProvider provider;

        [ExcludeFromCodeCoverage]
        internal MoleQuery([NotNull] QueryProvider provider)
        {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
            Expression = Expression.Constant(this);
        }

        [ExcludeFromCodeCoverage]
        internal MoleQuery([NotNull] QueryProvider provider, [NotNull] Expression expression)
        {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));

            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
                throw new ArgumentOutOfRangeException(nameof(expression));
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public Expression Expression { get; }
        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public Type ElementType => typeof(T);
        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public IQueryProvider Provider => provider;

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)provider.Execute(Expression)).GetEnumerator();
        }
        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
