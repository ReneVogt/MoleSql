/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Threading;
using MoleSql.Exceptions;

namespace MoleSql.Mapper
{
    static class SqlDataReaderMapper
    {
        /// <summary>
        /// Reads rows from a <see cref="SqlDataReader"/> and stores the values in instances of <typeparamref name="T"/>.
        /// It therefor looks for properties (not fields!) in <typeparamref name="T"/> that match the column names in the <see cref="SqlDataReader"/>.
        /// </summary>
        /// <typeparam name="T">The type of the row objects to generate.</typeparam>
        /// <param name="reader">The <see cref="SqlDataReader"/> to read from.</param>
        /// <returns>A sequence of objects of type <typeparamref name="T"/> representing the rows from the <paramref name="reader"/>.</returns>
        internal static IEnumerable<T> ReadObjects<T>(this SqlDataReader reader)
        {
            var type = typeof(T);
            return type.IsPrimitive || type == typeof(string) || Nullable.GetUnderlyingType(type)?.IsPrimitive == true
                       ? reader.ReadPrimitiveObjects<T>()
                       : reader.ReadComplexObjects<T>();
        }
        static IEnumerable<T> ReadPrimitiveObjects<T>(this SqlDataReader reader)
        {
            try
            {
                while (reader.Read())
                {
                    if (reader.VisibleFieldCount > 1)
                        throw typeof(T).CannotBeCreatedFromMultipleColumns();
                    yield return reader.IsDBNull(0) ? (T)(object)null! : reader.GetFieldValue<T>(0);
                }
            }
            finally
            {
                reader.Dispose();
            }
        }
        static IEnumerable<T> ReadComplexObjects<T>(this SqlDataReader reader)
        {
            try
            {
                var propertiesByName = typeof(T).GetProperties().Where(prop => prop.CanWrite).ToDictionary(prop => prop.Name, prop => prop);
                var propertyMappings = Enumerable.Range(0, reader.VisibleFieldCount)
                                                 .Select(i => (i, propertiesByName.TryGetValue(reader.GetName(i), out var p) ? p : null))
                                                 .Where(((int fieldIndex, PropertyInfo? property) x) => x.property != null)
                                                 .ToArray();

                while (reader.Read())
                {
                    T element = CreateInstance<T>();
                    foreach ((int fieldIndex, PropertyInfo property) in propertyMappings)
                        property.SetValue(element, reader.GetValue(fieldIndex));
                    yield return element;
                }
            }
            finally
            {
                reader.Dispose();
            }
        }

        /// <summary>
        /// Reads rows from a <see cref="SqlDataReader"/> asynchronously and stores the values in instances of <typeparamref name="T"/>.
        /// It therefor looks for properties (not fields!) in <typeparamref name="T"/> that match the column names in the <see cref="SqlDataReader"/>.
        /// </summary>
        /// <typeparam name="T">The type of the row objects to generate.</typeparam>
        /// <param name="reader">The <see cref="SqlDataReader"/> to read from.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>A sequence of objects of type <typeparamref name="T"/> representing the rows from the <paramref name="reader"/>.</returns>
        internal static IAsyncEnumerable<T> ReadObjectsAsync<T>(this SqlDataReader reader, CancellationToken cancellationToken = default)
        {
            var type = typeof(T);
            return type.IsPrimitive || type == typeof(string) || Nullable.GetUnderlyingType(type)?.IsPrimitive == true
                       ? reader.ReadPrimitiveObjectsAsync<T>(cancellationToken)
                       : reader.ReadComplexObjectsAsync<T>(cancellationToken);

        }
        static async IAsyncEnumerable<T> ReadPrimitiveObjectsAsync<T>(this SqlDataReader reader, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            try
            {
                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    if (reader.VisibleFieldCount > 1)
                        throw typeof(T).CannotBeCreatedFromMultipleColumns();
                    yield return await reader.IsDBNullAsync(0, cancellationToken).ConfigureAwait(false)
                                     ? (T)(object)null!
                                     : await reader.GetFieldValueAsync<T>(0, cancellationToken).ConfigureAwait(false);
                }
            }
            finally
            {
                reader.Dispose();
            }
        }
        static async IAsyncEnumerable<T> ReadComplexObjectsAsync<T>(this SqlDataReader reader, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            try
            {
                var propertiesByName = typeof(T).GetProperties().Where(prop => prop.CanWrite).ToDictionary(prop => prop.Name, prop => prop);
                var propertyMappings = Enumerable.Range(0, reader.VisibleFieldCount)
                                                 .Select(i => (i, propertiesByName.TryGetValue(reader.GetName(i), out var p) ? p : null))
                                                 .Where(((int fieldIndex, PropertyInfo? property) x) => x.property != null)
                                                 .ToArray();

                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    T element = CreateInstance<T>();
                    foreach ((int fieldIndex, PropertyInfo property) in propertyMappings)
                        property.SetValue(element, await reader.GetFieldValueAsync<object>(fieldIndex, cancellationToken).ConfigureAwait(false));
                    yield return element;
                }
            }
            finally
            {
                reader.Dispose();
            }
        }

        /// <summary>
        /// Reads rows from a <see cref="SqlDataReader"/> and stores the values in <see cref="ExpandoObject"/> instances
        /// to be used as dynamic objects.
        /// If the <paramref name="reader"/> contains only one column, these values are returned themselfes without wrapping
        /// them in a dynamic object.
        /// </summary>
        /// <param name="reader">The <see cref="SqlDataReader"/> to read from.</param>
        /// <returns>A sequence of dynamic objects representing the rows from the <paramref name="reader"/>.</returns>
        internal static IEnumerable ReadObjects(this SqlDataReader reader)
        {
            try
            {
                while (reader.Read())
                {
                    if (reader.VisibleFieldCount == 1)
                        yield return reader.IsDBNull(0) ? null : reader.GetValue(0);
                    else
                    {
                        IDictionary<string, object> element = new ExpandoObject();
                        for (int index = 0; index < reader.VisibleFieldCount; index++)
                            element[reader.GetName(index)] = reader.IsDBNull(index) ? null! : reader.GetValue(index);
                        yield return element;
                    }
                }
            }
            finally
            {
                reader.Dispose();
            }
        }
        /// <summary>
        /// Reads rows from a <see cref="SqlDataReader"/> asynchronously and stores the values in <see cref="ExpandoObject"/> instances
        /// to be used as dynamic objects.
        /// If the <paramref name="reader"/> contains only one column, these values are returned themselfes without wrapping
        /// them in a dynamic object.
        /// </summary>
        /// <param name="reader">The <see cref="SqlDataReader"/> to read from.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel this asynchronous operation.</param>
        /// <returns>A task that on completion returns a sequence of dynamic objects representing the rows from the <paramref name="reader"/>.</returns>
        internal static async IAsyncEnumerable<object> ReadObjectsAsync(this SqlDataReader reader, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            try
            {
                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    if (reader.VisibleFieldCount == 1)
                        yield return await reader.IsDBNullAsync(0, cancellationToken).ConfigureAwait(false)
                                         ? null!
                                         : await reader.GetFieldValueAsync<object>(0, cancellationToken).ConfigureAwait(false);
                    else
                    {
                        IDictionary<string, object> element = new ExpandoObject();
                        for (int index = 0; index < reader.VisibleFieldCount; index++)
                            element[reader.GetName(index)] = await reader.IsDBNullAsync(index, cancellationToken).ConfigureAwait(false)
                                                                 ? null!
                                                                 : await reader.GetFieldValueAsync<object>(index, cancellationToken)
                                                                               .ConfigureAwait(false);
                        yield return element;
                    }
                }
            }
            finally
            {
                reader.Dispose();
            }
        }

        static T CreateInstance<T>() => Activator.CreateInstance<T>();
    }
}
