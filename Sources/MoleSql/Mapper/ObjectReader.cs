/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
  *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
*
 */
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace MoleSql.Mapper
{
    static class ObjectReader
    {
        /// <summary>
        /// Reads rows from a <see cref="SqlDataReader"/> and stores the values in instances of <typeparamref name="T"/>.
        /// It therefor looks for properties (not fields!) in <typeparamref name="T"/> that match the column names in the <see cref="SqlDataReader"/>.
        /// </summary>
        /// <typeparam name="T">The type of the row objects to generate.</typeparam>
        /// <param name="reader">The <see cref="SqlDataReader"/> to read from.</param>
        /// <returns>A sequence of objects of type <typeparamref name="T"/> representing the rows from the <paramref name="reader"/>.</returns>
        internal static IEnumerable<T> ReadObjects<T>(this SqlDataReader reader) where T : class, new()
        {
            var propertiesByName = typeof(T).GetProperties().ToDictionary(prop => prop.Name, prop => prop);
            var fieldMappings = Enumerable.Range(0, reader.FieldCount)
                                      .Select(i => (i, propertiesByName.TryGetValue(reader.GetName(i), out var p) ? p : null))
                                      .Where(((int fieldIndex, PropertyInfo property) x) => x.property != null)
                                      .ToArray();

            while (reader.Read())
            {
                T element = new T();
                foreach ((int fieldIndex, PropertyInfo property) in fieldMappings)
                    property.SetValue(element, reader.GetValue(fieldIndex));
                yield return element;
            }

            reader.Dispose();
        }
    }
}
