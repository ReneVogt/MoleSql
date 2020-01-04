/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace MoleSql.Helpers 
{
    static class TypeSystemHelper
    {
        internal static Type GetElementType(Type seqType)
        {
            Type ienum = FindIEnumerable(seqType);
            if (ienum == null) return seqType;
            return ienum.GetGenericArguments()[0];
        }
        internal static Type GetSequenceType(Type elementType) => typeof(IEnumerable<>).MakeGenericType(elementType);

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
}
