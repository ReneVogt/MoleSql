using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace MoleSql
{
    sealed class Query<T> : IOrderedQueryable<T>
    {
        readonly QueryProvider provider;
        readonly Expression expression;

        public Query([NotNull] QueryProvider provider)
        {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));

            expression = Expression.Constant(this);
        }

        public Query([NotNull] QueryProvider provider, [NotNull] Expression expression)
        {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
            this.expression = expression ?? throw new ArgumentNullException(nameof(expression));

            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
                throw new ArgumentOutOfRangeException(nameof(expression));
        }

        Expression IQueryable.Expression => expression;
        Type IQueryable.ElementType => typeof(T);
        IQueryProvider IQueryable.Provider => provider;

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)provider.Execute(expression)).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => provider.GetQueryText(expression);
    }
}
