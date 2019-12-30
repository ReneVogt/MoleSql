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
using MoleSql.Helpers;

namespace MoleSql.Mapper
{
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
                if (!reader.Read())
                {
                    Dispose();
                    return false;
                }
                Current = projector(this);
                return true;
            }
            public void Reset() { }
            public void Dispose()
            {
                reader.Dispose();
            }
        }

        readonly SqlDataReader reader;
        readonly Func<ProjectionRow, T> projector;

        bool used;

        internal ProjectionReader(SqlDataReader reader, Func<ProjectionRow, T> projector)
        {
            this.reader = reader;
            this.projector = projector;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (used)
                throw new ObjectDisposedException(nameof(ProjectionReader<T>), "Cannot enumerate the SqlDataReader more than once.");
            used = true;
            return new Enumerator(reader, projector);
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}