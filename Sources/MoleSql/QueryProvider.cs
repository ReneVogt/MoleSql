/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MoleSql.Helpers;
using MoleSql.Mapper;
using MoleSql.Translators;

namespace MoleSql 
{
    sealed class QueryProvider : IQueryProvider, IDisposable
    {
        readonly bool disposeConnection;

        bool disposed;

        internal SqlConnection Connection { get; }
        internal SqlTransaction Transaction { get; set; }
        internal TextWriter Log { get; set; }

        internal QueryProvider(SqlConnection connection, bool ownConnection)
        {
            disposeConnection = ownConnection;
            Connection = connection;
        }
        public void Dispose()
        {
            if (disposed) return;
            if (disposeConnection) Connection.Dispose();
            disposed = true;
        }
        IQueryable<S> IQueryProvider.CreateQuery<S>(Expression expression)
        {
            return new Query<S>(this, expression);
        }
        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = TypeSystemHelper.GetElementType(expression.Type);

            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(Query<>).MakeGenericType(elementType), this, expression);
            }
            catch (TargetInvocationException tie) when (tie.InnerException != null)
            {
                throw tie.InnerException;
            }
        }
        S IQueryProvider.Execute<S>(Expression expression)
        {
            return (S)Execute(expression);
        }

        internal SqlTransaction BeginTransaction()
        {
            OpenConnection();
            return Connection.BeginTransaction();
        }
        internal SqlTransaction BeginTransaction(string transactionName)
        {
            OpenConnection();
            return Connection.BeginTransaction(transactionName);
        }
        internal SqlTransaction BeginTransaction(IsolationLevel iso)
        {
            OpenConnection();
            return Connection.BeginTransaction(iso);
        }
        internal SqlTransaction BeginTransaction(IsolationLevel iso, string transactionName)
        {
            OpenConnection();
            return Connection.BeginTransaction(iso, transactionName);
        }

        internal IEnumerable<T> ExecuteQuery<T>(FormattableString query) where T : class, new()
        {
            CheckDisposed();

            using var cmd = Connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            OpenConnection();
            return cmd.ExecuteReader().ReadObjects<T>();
        }
        internal async IAsyncEnumerable<T> ExecuteQueryAsync<T>(FormattableString query, [EnumeratorCancellation] CancellationToken cancellationToken = default) where T : class, new()
        {
            CheckDisposed();

            using var cmd = Connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
            await foreach (var element in (await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false)).ReadObjectsAsync<T>(
                cancellationToken))
                yield return element;
        }
        internal IEnumerable ExecuteQuery(FormattableString query)
        {
            CheckDisposed();

            using var cmd = Connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            OpenConnection();
            return cmd.ExecuteReader().ReadObjects();
        }
        internal async IAsyncEnumerable<dynamic> ExecuteQueryAsync(FormattableString query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            using var cmd = Connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
            await foreach (var element in (await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false)).ReadObjectsAsync(cancellationToken))
                yield return element;
        }
        internal object ExecuteScalar(FormattableString query)
        {
            CheckDisposed();

            using var cmd = Connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            OpenConnection();
            return cmd.ExecuteScalar();
        }
        internal T ExecuteScalar<T>(FormattableString query)
        {
            CheckDisposed();

            using var cmd = Connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            OpenConnection();
            return (T)cmd.ExecuteScalar();
        }
        internal async Task<object> ExecuteScalarAsync(FormattableString query, CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            using var cmd = Connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
            return await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        }
        internal async Task<T> ExecuteScalarAsync<T>(FormattableString query, CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            using var cmd = Connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
            return (T)await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        }
        internal Int32 ExecuteNonQuery(FormattableString query)
        {
            CheckDisposed();

            using var cmd = Connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            OpenConnection();
            return cmd.ExecuteNonQuery();
        }
        internal async Task<Int32> ExecuteNonQueryAsync(FormattableString query, CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            using var cmd = Connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
            return await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        [SuppressMessage("Microsoft.Security", "CA2100", Justification = "This is what this is all about.")]
        public object Execute(Expression expression)
        {
            CheckDisposed();

            using var cmd = Connection.CreateCommand();
            cmd.Transaction = Transaction;

            var (sql, projection, parameters, isTopLevelAggregation) = SqlQueryTranslator.Translate(this, expression);

            cmd.CommandText = sql;
            parameters.ForEach(p => cmd.Parameters.AddWithValue(p.name, p.value));

            LogCommand(cmd);

            OpenConnection();
            if (isTopLevelAggregation)
                return ChangeType(cmd.ExecuteScalar(), expression.Type);

            SqlDataReader reader = cmd.ExecuteReader();
            Type elementType = TypeSystemHelper.GetElementType(expression.Type);
            var projector = projection.Compile();

            return (IEnumerable)Activator.CreateInstance(
                typeof(ProjectionReader<>).MakeGenericType(elementType),
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] { reader, projector, this },
                null);
        }
        [SuppressMessage("Microsoft.Security", "CA2100", Justification = "This is what this is all about.")]
        internal async IAsyncEnumerable<T> ExecuteAsync<T>(Expression expression, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            using var cmd = Connection.CreateCommand();
            cmd.Transaction = Transaction;

            var (sql, projection, parameters, _) = SqlQueryTranslator.Translate(this, expression);

            cmd.CommandText = sql;
            parameters.ForEach(p => cmd.Parameters.AddWithValue(p.name, p.value));

            LogCommand(cmd);

            await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
            SqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            var projector = (Func<ProjectionRow, T>)projection.Compile();
            var sequence = new ProjectionReader<T>(reader, projector, this);
            await foreach (var element in sequence)
                yield return element;
        }
        [SuppressMessage("Microsoft.Security", "CA2100", Justification = "This is what this is all about.")]
        internal async Task<T> ExecuteAggregateAsync<T>(Expression expression, CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            using var cmd = Connection.CreateCommand();
            cmd.Transaction = Transaction;

            var (sql, _, parameters, isTopLevelAggregation) = SqlQueryTranslator.Translate(this, expression);
            if (!isTopLevelAggregation)
                throw new InvalidOperationException("Invalid call: this expression must be a top level aggregation.");

            cmd.CommandText = sql;
            parameters.ForEach(p => cmd.Parameters.AddWithValue(p.name, p.value));

            LogCommand(cmd);

            await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
            return (T)ChangeType(await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false), typeof(T));
        }

        void LogCommand(SqlCommand command)
        {
            var log = Log;
            if (log == null) return;

            StringBuilder logbuilder = new StringBuilder();
            logbuilder.AppendLine(command.CommandText);

            foreach (SqlParameter parameter in command.Parameters)
                logbuilder.AppendLine($"-- {parameter.ParameterName} {parameter.SqlDbType} {parameter.Direction} [{parameter.SqlValue}]");
            logbuilder.AppendLine();

            log.Write(logbuilder.ToString());
        }
        void CheckDisposed()
        {
            if (disposed) throw new ObjectDisposedException(nameof(QueryProvider), "The query provider has already been disposed of.");
        }
        void OpenConnection()
        {
            CheckDisposed();
            if (Connection.State == ConnectionState.Closed)
                Connection.Open();
        }
        async ValueTask OpenConnectionAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();
            if (Connection.State == ConnectionState.Closed)
                await Connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        }

        static object ChangeType(object value, Type type)
        {
            if (value is null || value.GetType() == type) return value;

            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Nullable<>))
                return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);

            return Activator.CreateInstance(typeof(Nullable<>).MakeGenericType(Nullable.GetUnderlyingType(type)), value);
        }
    }
}
