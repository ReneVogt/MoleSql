using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace MoleSql.Mapper
{
    static class ObjectReader
    {
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
