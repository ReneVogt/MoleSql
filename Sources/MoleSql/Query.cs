﻿/*
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

namespace MoleSql
{
    /// <summary>
    /// The root element for MoleSql queries.
    /// </summary>
    /// <typeparam name="T">The type of the table.</typeparam>
    public class Query<T> : IQueryable<T>, IQueryable, IOrderedQueryable<T>, IOrderedQueryable, IEnumerable<T>, IEnumerable
    {
        readonly QueryProvider provider;

        internal Query([NotNull] QueryProvider provider)
        {
            this.provider = provider;
            Expression = Expression.Constant(this);
        }

        internal Query([NotNull] QueryProvider provider, [NotNull] Expression expression)
        {
            this.provider = provider;
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));

            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
                throw new ArgumentOutOfRangeException(nameof(expression));
        }

        /// <inheritdoc />
        public Expression Expression { get; }
        /// <inheritdoc />
        public Type ElementType => typeof(T);
        /// <inheritdoc />
        public IQueryProvider Provider => provider;

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)provider.Execute(Expression)).GetEnumerator();
        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
