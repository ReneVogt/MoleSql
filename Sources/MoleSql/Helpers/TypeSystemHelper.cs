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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using MoleSql.Exceptions;

namespace MoleSql.Helpers 
{
    /// <summary>
    /// Provides helper methods (extensions) for common type system questions:
    /// <see cref="GetElementType"/> finds the type of an element
    /// in a sequential type (<see cref="IEnumerable{T}"/>) and <see cref="GetSequenceType"/>
    /// creates an <see cref="IEnumerable{T}"/> for the given element type.
    /// </summary>
    static class TypeSystemHelper
    {
        static readonly MethodInfo changeTypeMethod = typeof(TypeSystemHelper)
            .GetMethod(
                nameof(ChangeType),
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new[] {typeof(object)}, null);

        /// <summary>
        /// Tries to convert an object into another type.
        /// This method is used to convert for example an <see cref="Int32"/> into an <see cref="Nullable{Int32}"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="type">The <see cref="Type"/> to convert the <paramref name="value"/> to.</param>
        /// <returns>The <paramref name="value"/> represented as <paramref name="type"/>.</returns>
        internal static object ChangeType(object value, Type type)
        {
            try
            {
                var method = changeTypeMethod.MakeGenericMethod(type);
                return method.Invoke(null, new[] {value});
            }
            catch (TargetInvocationException tie) when (tie.InnerException != null)
            {
                throw tie.InnerException;
            }
            //if (value is null || value.GetType() == type) return value;

            //if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Nullable<>))
            //    return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);

            //var underlyingType = Nullable.GetUnderlyingType(type);
            //Debug.Assert(underlyingType?.IsValueType == true);
            //return Activator.CreateInstance(
            //    typeof(Nullable<>).MakeGenericType(underlyingType),
            //    Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture));
        }
        internal static T ChangeType<T>(object value)
        {
            Type type = typeof(T);

            if (value == null)
            {
                if (type.IsValueType && Nullable.GetUnderlyingType(type) == null)
                    throw type.CannotBeConvertedToFromNull();

                return default;
            }
            if (value.GetType() == typeof(T)) return (T)value;

            type = Nullable.GetUnderlyingType(type) ?? type;
            Debug.Assert(type != null);

            return (T)Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Determines if the given <paramref name="sequenceType"/> is in any way
        /// assignable to an <see cref="IEnumerable{T}"/> and if so, returns the
        /// generic type argument for this interface.
        /// It therefor uses the private <see cref="FindIEnumerable"/> method which
        /// determines if the given type or any of it's base types implements the
        /// <see cref="IEnumerable{T}"/> interface.
        /// </summary>
        /// <param name="sequenceType">The <see cref="Type"/> to investigate.</param>
        /// <returns>The <paramref name="sequenceType"/> itself if it does not implement any
        /// kind of <see cref="IEnumerable{T}"/> or that "T" if one is found.</returns>
        internal static Type GetElementType(Type sequenceType)
        {
            Type enumerable = FindIEnumerable(sequenceType);
            if (enumerable == null) return sequenceType;
            return enumerable.GetGenericArguments()[0];
        }
       
        /// <summary>
        /// Creates the <see cref="IEnumerable{T}"/> type with <paramref name="elementType"/>
        /// as generic argument.
        /// </summary>
        /// <param name="elementType">The element type of the created sequence type.</param>
        /// <returns>A <see cref="Type"/> instance representing the <see cref="IEnumerable{T}"/> of <paramref name="elementType"/> type.</returns>
        internal static Type GetSequenceType(Type elementType) => typeof(IEnumerable<>).MakeGenericType(elementType);

        /// <summary>
        /// Checks if the given <paramref name="sequenceType"/> or one of its base
        /// types represents or implements or derives from a type implementing the
        /// <see cref="IEnumerable{T}"/> interface and if so, returns this constructed
        /// <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="sequenceType">The <see cref="Type"/> to investigate.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that is in any way implemented by <paramref name="sequenceType"/> or
        /// <code>null</code> <paramref name="sequenceType"/> is not assignable to any version of <see cref="IEnumerable{T}"/>.</returns>
        static Type FindIEnumerable(Type sequenceType)
        {
            if (sequenceType == null || sequenceType == typeof(string))
                return null;

            if (sequenceType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(sequenceType.GetElementType());

            Type enumerationType;
            if (sequenceType.IsGenericType)
            {
                var ienumTypes = from arg in sequenceType.GetGenericArguments()
                                 select typeof(IEnumerable<>).MakeGenericType(arg);
                enumerationType = ienumTypes.FirstOrDefault(t => t.IsAssignableFrom(sequenceType));
                if (enumerationType != null) return enumerationType;
            }

            enumerationType = sequenceType.GetInterfaces().Select(FindIEnumerable).FirstOrDefault(t => t != null);
            if (enumerationType != null) return enumerationType;

            if (sequenceType.BaseType != null && sequenceType.BaseType != typeof(object))
                return FindIEnumerable(sequenceType.BaseType);

            return null;
        }
    }
}
