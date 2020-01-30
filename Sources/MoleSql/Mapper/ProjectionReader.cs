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
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MoleSql.Exceptions;
using MoleSql.Expressions;
using MoleSql.Helpers;
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

            internal override TColumn GetValue<TColumn>(int index) => values == null
                                                                ? reader.IsDBNull(index) ? default : TypeSystemHelper.ChangeType<TColumn>(reader.GetValue(index))
                                                                : TypeSystemHelper.ChangeType<TColumn>(values[index]);
            internal override IEnumerable<TSubQuery> ExecuteSubQuery<TSubQuery>(LambdaExpression subQueryExpression)
            {
                var replacement = Expression.Constant(this);
                var projection = (ProjectionExpression)subQueryExpression.Body;
                var projectionWithReplacedOuterColumnReferences = ExpressionReplacer.Replace(projection, ReplaceWith);
                var projectionWithEvaluatedOuterColumnReferences = projectionWithReplacedOuterColumnReferences.EvaluateLocally(CanExpressionBeEvaluatedLocally);

                var result = ((IEnumerable<TSubQuery>)queryProvider.Execute(projectionWithEvaluatedOuterColumnReferences)).ToList();
                return typeof(IQueryable<TSubQuery>).IsAssignableFrom(subQueryExpression.Body.Type)
                           ? result.AsQueryable()
                           : (IEnumerable<TSubQuery>)result;

                Expression ReplaceWith(Expression expression) =>
                    expression == subQueryExpression.Parameters[0] ||
                    typeof(ProjectionRow).IsAssignableFrom(expression.Type) 
                        ? replacement 
                        : null; 
            }
            public T Current { get; private set; }
            [ExcludeFromCodeCoverage]
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
                for (Int32 i = 0; i < reader.FieldCount; i++)
                    values.Add(await reader.IsDBNullAsync(i, cancellationToken).ConfigureAwait(false)
                                   ? null
                                   : await reader.GetFieldValueAsync<object>(i, cancellationToken).ConfigureAwait(false));

                Current = projector(this);
                values = null;
                return true;
            }
            [ExcludeFromCodeCoverage]
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

            static bool CanExpressionBeEvaluatedLocally(Expression expression) =>
                expression.NodeType != ExpressionType.Parameter &&
                !expression.IsDbExpression() &&
                !IsExecuteSubQueryExpression(expression);

            static bool IsExecuteSubQueryExpression(Expression expression) =>
                expression is MethodCallExpression methodCall &&
                methodCall.Method.DeclaringType == typeof(ProjectionRow) &&
                methodCall.Method.Name == nameof(ExecuteSubQuery);
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
                throw nameof(ProjectionReader<T>).CanOnlyBeIteratedOnce();
            used = true;
            return new Enumerator(reader, projector, queryProvider, CancellationToken.None);
        }
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken)
        {
            if (used)
                throw nameof(ProjectionReader<T>).CanOnlyBeIteratedOnce();
            used = true;
            return new Enumerator(reader, projector, queryProvider, cancellationToken);
        }
        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}