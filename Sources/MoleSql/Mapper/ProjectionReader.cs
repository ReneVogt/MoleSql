﻿/*
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
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MoleSql.Expressions;
using MoleSql.Helpers;
using MoleSql.QueryProviders;
using MoleSql.Translators;

namespace MoleSql.Mapper
{
    [SuppressMessage("Microsoft.Performance", "CA1812", Justification = "This class is instantiated via Activator.CreateInstance.")]
    class ProjectionReader<T> : IAsyncEnumerable<T>, IEnumerable<T>
    {
        class Enumerator : ProjectionRow, IAsyncEnumerator<T>, IEnumerator<T>
        {
            readonly SqlDataReader reader;
            readonly Func<ProjectionRow, T> projector;
            readonly QueryProvider queryProvider;
            readonly CancellationToken cancellationToken;

            List<object> values;

            internal Enumerator(SqlDataReader reader, Func<ProjectionRow, T> projector, QueryProvider queryProvider, CancellationToken cancellationToken)
            {
                this.reader = reader;
                this.projector = projector;
                this.queryProvider = queryProvider;
                this.cancellationToken = cancellationToken;
            }

            internal override object GetValue(int index) => values == null
                                                                ? reader.IsDBNull(index) ? null : reader.GetValue(index)
                                                                : values[index];
            internal override IEnumerable<TSubQuery> ExecuteSubQuery<TSubQuery>(LambdaExpression subQueryExpression)
            {
                var projection = (ProjectionExpression)subQueryExpression.Body;
                var projectionWithReplacedOuterColumnReferences = ExpressionReplacer.Replace(projection, subQueryExpression.Parameters[0], Expression.Constant(this));
                var projectionWithEvaluatedOuterColumnReferences = projectionWithReplacedOuterColumnReferences.EvaluateLocally();

                var result = ((IEnumerable<TSubQuery>)queryProvider.Execute(projectionWithEvaluatedOuterColumnReferences)).ToList();
                return typeof(IQueryable<TSubQuery>).IsAssignableFrom(subQueryExpression.Body.Type)
                           ? result.AsQueryable()
                           : (IEnumerable<TSubQuery>)result;
            }
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
            public async ValueTask<bool> MoveNextAsync()
            {
                if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    await DisposeAsync();
                    return false;
                }

                values = new List<object>();
                for (int i = 0; i < reader.FieldCount; i++)
                    values.Add(await reader.IsDBNullAsync(i, cancellationToken).ConfigureAwait(false)
                                   ? null
                                   : await reader.GetFieldValueAsync<object>(i, cancellationToken).ConfigureAwait(false));

                Current = projector(this);
                values = null;
                return true;
            }
            public void Reset() { }
            public void Dispose()
            {
                reader.Dispose();
            }
            public ValueTask DisposeAsync()
            {
                reader.Dispose();
                return new ValueTask(Task.CompletedTask);
            }
        }

        readonly SqlDataReader reader;
        readonly Func<ProjectionRow, T> projector;
        readonly QueryProvider queryProvider;

        bool used;

        internal ProjectionReader(SqlDataReader reader, Func<ProjectionRow, T> projector, QueryProvider queryProvider)
        {
            this.reader = reader;
            this.projector = projector;
            this.queryProvider = queryProvider;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (used)
                throw new ObjectDisposedException(nameof(ProjectionReader<T>), "Cannot enumerate the SqlDataReader more than once.");
            used = true;
            return new Enumerator(reader, projector, queryProvider, CancellationToken.None);
        }
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken)
        {
            if (used)
                throw new ObjectDisposedException(nameof(ProjectionReader<T>), "Cannot enumerate the SqlDataReader more than once.");
            used = true;
            return new Enumerator(reader, projector, queryProvider, cancellationToken);
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}