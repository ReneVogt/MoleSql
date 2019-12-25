using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MoleSql.Translators;

#pragma warning disable CA1812

namespace MoleSql.Mapper
{
    static class SqlObjectReader
    {
        [SuppressMessage("Design", "CA1001", Justification = "The enumerator will be disposed by user code.")]
        class DynamicReader : IEnumerable
        {
            class Enumerator : IEnumerator, IDisposable
            {
                readonly SqlDataReader reader;

                internal Enumerator(SqlDataReader reader)
                {
                    this.reader = reader;
                }

                public object Current { get; private set; }
                public bool MoveNext()
                {
                    if (!reader.Read()) return false;
                    IDictionary<string, object> element = new ExpandoObject();
                    for (int index = 0; index < reader.FieldCount; index++)
                        element[reader.GetName(index)] = reader.IsDBNull(index) ? null : reader.GetValue(index);
                    Current = element;
                    return true;
                }
                public void Reset() { }
                public void Dispose()
                {
                    reader.Dispose();
                }
            }
            Enumerator enumerator;
            internal DynamicReader(SqlDataReader reader)
            {
                enumerator = new Enumerator(reader);
            }

            public IEnumerator GetEnumerator()
            {
                var e = enumerator;
                enumerator = null;
                if (e == null)
                    throw new ObjectDisposedException(nameof(DynamicReader), "Cannot enumerate the SqlDataReader more than once.");
                return e;
            }
        }
        [SuppressMessage("Design", "CA1001", Justification = "The enumerator will be disposed by user code.")]
        class ObjectReader<T> : IEnumerable<T> where T : class, new()
        {
            class Enumerator : IEnumerator<T>
            {
                readonly SqlDataReader reader;
                readonly (int index, PropertyInfo property)[] fieldMappings;

                internal Enumerator(SqlDataReader reader)
                {
                    this.reader = reader;
                    var propertiesByName = typeof(T).GetProperties().ToDictionary(prop => prop.Name, prop => prop);
                    fieldMappings = Enumerable.Range(0, reader.FieldCount)
                                              .Select(i => (i, propertiesByName.TryGetValue(reader.GetName(i), out var p) ? p : null))
                                              .Where(((int fieldIndex, PropertyInfo property) x) => x.property != null)
                                              .ToArray();
                }

                public T Current { get; private set; }
                object IEnumerator.Current => Current;
                public bool MoveNext()
                {
                    if (!reader.Read()) return false;
                    T element = new T();
                    foreach ((int fieldIndex, PropertyInfo property) in fieldMappings)
                        property.SetValue(element, reader.GetValue(fieldIndex));
                    Current = element;
                    return true;
                }
                public void Reset() { }
                public void Dispose()
                {
                    reader.Dispose();
                }
            }
            Enumerator enumerator;
            internal ObjectReader(SqlDataReader reader)
            {
                enumerator = new Enumerator(reader);
            }

            public IEnumerator<T> GetEnumerator()
            {
                var e = enumerator;
                enumerator = null;
                if (e == null)
                    throw new ObjectDisposedException(nameof(ObjectReader<T>), "Cannot enumerate the SqlDataReader more than once.");
                return e;
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [SuppressMessage("Design", "CA1001", Justification = "The enumerator will be disposed by user code.")]
        class ProjectionReader<T> : IEnumerable<T>
        {
            class Enumerator : ProjectionRow, IEnumerator<T>
            {
                readonly SqlDataReader reader;
                readonly Func<ProjectionRow, T> projector;

                internal Enumerator(SqlDataReader reader, Func<ProjectionRow, T> projector)
                {
                    this.reader = reader;
                    this.projector = projector;
                }

                internal override object GetValue(int index) => reader.IsDBNull(index) ? null : reader.GetValue(index);

                public T Current { get; private set; }
                object IEnumerator.Current => Current;
                public bool MoveNext()
                {
                    if (!reader.Read()) return false;
                    Current = projector(this);
                    return true;
                }
                public void Reset() { }
                public void Dispose()
                {
                    reader.Dispose();
                }
            }
            Enumerator enumerator;
            internal ProjectionReader(SqlDataReader reader, Func<ProjectionRow, T> projector)
            {
                enumerator = new Enumerator(reader, projector);
            }

            public IEnumerator<T> GetEnumerator()
            {
                var e = enumerator;
                enumerator = null;
                if (e == null)
                    throw new ObjectDisposedException(nameof(ProjectionReader<T>), "Cannot enumerate the SqlDataReader more than once.");
                return e;
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        internal static object GetReader(Type elementType, LambdaExpression projector, SqlDataReader reader)
        {
            if (elementType == null) return new DynamicReader(reader);
            if (projector == null)
                return Activator.CreateInstance(
                    typeof(ObjectReader<>).MakeGenericType(elementType),
                    BindingFlags.Instance | BindingFlags.NonPublic, null,
                    new object[] {reader},
                    null);
            return Activator.CreateInstance(
                typeof(ProjectionReader<>).MakeGenericType(elementType),
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] {reader, projector.Compile()},
                null);
        }
    }
}
