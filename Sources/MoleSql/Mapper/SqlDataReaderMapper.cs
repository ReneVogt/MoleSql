/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Dynamic;

namespace MoleSql.Mapper
{
    [ExcludeFromCodeCoverage]
    static class SqlDataReaderMapper
    {
        /// <summary>
        /// Reads rows from a <see cref="SqlDataReader"/> and stores the values in instances of <typeparamref name="T"/>.
        /// It therefor looks for properties (not fields!) in <typeparamref name="T"/> that match the column names in the <see cref="SqlDataReader"/>.
        /// </summary>
        /// <typeparam name="T">The type of the row objects to generate.</typeparam>
        /// <param name="reader">The <see cref="SqlDataReader"/> to read from.</param>
        /// <returns>A sequence of objects of type <typeparamref name="T"/> representing the rows from the <paramref name="reader"/>.</returns>
        [ExcludeFromCodeCoverage]
        internal static IEnumerable<T> ReadObjects<T>(this SqlDataReader reader) where T : class, new()
        {
            try
            {
                var propertiesByName = typeof(T).GetProperties().Where(prop => prop.CanWrite).ToDictionary(prop => prop.Name, prop => prop);
                var propertyMappings = Enumerable.Range(0, reader.FieldCount)
                                                 .Select(i => (i, propertiesByName.TryGetValue(reader.GetName(i), out var p) ? p : null))
                                                 .Where(((int fieldIndex, PropertyInfo property) x) => x.property != null)
                                                 .ToArray();

                while (reader.Read())
                {
                    T element = new T();
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
        /// Reads rows from a <see cref="SqlDataReader"/> and stores the values in <see cref="ExpandoObject"/> instances
        /// to be used as dynamic objects.
        /// </summary>
        /// <param name="reader">The <see cref="SqlDataReader"/> to read from.</param>
        /// <returns>A sequence of dynamic objects representing the rows from the <paramref name="reader"/>.</returns>
        [ExcludeFromCodeCoverage]
        internal static IEnumerable ReadObjects(this SqlDataReader reader)
        {
            try
            {
                while (reader.Read())
                {
                    IDictionary<string, object> element = new ExpandoObject();
                    for (int index = 0; index < reader.FieldCount; index++)
                        element[reader.GetName(index)] = reader.IsDBNull(index) ? null : reader.GetValue(index);
                    yield return element;
                }
            }
            finally
            {
                reader.Dispose();
            }
        }
    }
}
