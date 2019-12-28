/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using MoleSql.Translators;

namespace MoleSql.Mapper
{
    [SuppressMessage("Design", "CA1001", Justification = "The enumerator will be disposed by user code.")]
    [SuppressMessage("Microsoft.Performance", "CA1812", Justification = "This class is instantiated via Activator.CreateInstance.")]
    [ExcludeFromCodeCoverage]
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
}