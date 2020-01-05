/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MoleSql.Helpers;

namespace MoleSql.QueryProviders 
{
    /// <summary>
    /// An abstract base class for query providers, handling the generic and non-generic <see cref="IQueryProvider.CreateQuery"/> calls
    /// and directing the <see cref="IQueryProvider.Execute"/> calls to the abstract <see cref="QueryProvider.Execute"/> method.
    /// </summary>
    abstract class QueryProvider : IQueryProvider
    {
        IQueryable<S> IQueryProvider.CreateQuery<S>(Expression expression)
        {
            return new MoleQuery<S>(this, expression);
        }
        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = TypeSystemHelper.GetElementType(expression.Type);

            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(MoleQuery<>).MakeGenericType(elementType), this, expression);
            }
            catch (TargetInvocationException tie) when (tie.InnerException != null)
            {
                throw tie.InnerException;
            }
        }
        S IQueryProvider.Execute<S>(Expression expression)
        {
            return (S)Execute(expression);
        }
        object IQueryProvider.Execute(Expression expression)
        {
            return Execute(expression);
        }

        public abstract object Execute(Expression expression);
    }
}
