using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MoleSql.QueryProviders {
    abstract class QueryProvider : IQueryProvider
    {
        protected static class TypeSystem
        {
            internal static Type GetElementType(Type seqType)
            {
                Type ienum = FindIEnumerable(seqType);
                if (ienum == null) return seqType;
                return ienum.GetGenericArguments()[0];
            }

            static Type FindIEnumerable(Type seqType)
            {
                if (seqType == null || seqType == typeof(string))
                    return null;

                if (seqType.IsArray)
                    return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());

                Type enumerationType;
                if (seqType.IsGenericType)
                {
                    var ienumTypes = from arg in seqType.GetGenericArguments()
                                     select typeof(IEnumerable<>).MakeGenericType(arg);
                    enumerationType = ienumTypes.FirstOrDefault(t => t.IsAssignableFrom(seqType));
                    if (enumerationType != null) return enumerationType;
                }

                enumerationType = seqType.GetInterfaces().Select(FindIEnumerable).FirstOrDefault(t => t != null);
                if (enumerationType != null) return enumerationType;

                if (seqType.BaseType != null && seqType.BaseType != typeof(object))
                    return FindIEnumerable(seqType.BaseType);

                return null;
            }
        }

        IQueryable<S> IQueryProvider.CreateQuery<S>(Expression expression)
        {
            return new MoleQuery<S>(this, expression);
        }
        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);

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
