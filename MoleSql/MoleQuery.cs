using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        readonly MoleQueryProvider provider;

        internal MoleQuery([NotNull] MoleQueryProvider provider)
        {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
            Expression = Expression.Constant(this);
        }

        internal MoleQuery([NotNull] MoleQueryProvider provider, [NotNull] Expression expression)
        {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
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
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)provider.Execute(Expression)).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public override string ToString() => Debugger.IsAttached ? base.ToString() : provider.GetQueryText(Expression);
    }
}
