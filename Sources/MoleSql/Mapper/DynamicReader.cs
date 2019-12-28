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
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;

namespace MoleSql.Mapper 
{
    /// <summary>
    /// Reads rows from a <see cref="SqlDataReader"/> and stores the values in <see cref="ExpandoObject"/> instances
    /// to be used as dynamic objects.
    /// </summary>
    [SuppressMessage("Design", "CA1001", Justification = "The enumerator will be disposed by user code.")]
    [ExcludeFromCodeCoverage]
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
}
