using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace MoleSql.Mapper
{
    static class SqlObjectReader
    {
        internal static MethodInfo GetReaderMethod(Type elementType) => typeof(SqlObjectReader).GetMethod(
                                                                                                   nameof(SqlObjectReader.ReadObjects),
                                                                                                   BindingFlags.Static | BindingFlags.NonPublic,
                                                                                                   null,
                                                                                                   new[] {typeof(SqlDataReader)},
                                                                                                   null)
                                                                                               ?.MakeGenericMethod(elementType);

        internal static IEnumerable<T> ReadObjects<T>([NotNull] this SqlDataReader reader) where T : class, new()
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            return ObjectsIterator();

            IEnumerable<T> ObjectsIterator()
            {
                using(reader)
                {
                    var fieldsToProps = CreatePropertyMapping<T>(reader);
                    while (reader.Read())
                    {
                        T element = new T();
                        foreach((int fieldIndex, PropertyInfo property) in fieldsToProps)
                            property.SetValue(element, reader.GetValue(fieldIndex));

                        yield return element;
                    }
                }
            }
        }
        private static (int fieldIndex, PropertyInfo property)[] CreatePropertyMapping<T>(SqlDataReader reader)
        {
            var propertiesByName = typeof(T).GetProperties().ToDictionary(prop => prop.Name, prop => prop);
            return Enumerable.Range(0, reader.FieldCount)
                             .Select(i => (i, propertiesByName.TryGetValue(reader.GetName(i), out var p) ? p : null))
                             .Where(((int fieldIndex, PropertyInfo property) x) => x.property != null)
                             .ToArray();
        }
    }
}
