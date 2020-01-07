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
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MoleSql.Helpers;
using MoleSql.Mapper;
using MoleSql.Translators;

namespace MoleSql.QueryProviders 
{
    sealed class MoleSqlQueryProvider : QueryProvider, IDisposable
    {
        readonly SqlConnection connection;
        readonly bool disposeConnection;

        bool disposed;

        internal SqlTransaction Transaction { get; set; }
        internal TextWriter Log { get; set; }

        internal MoleSqlQueryProvider(string connectionString) : this(new SqlConnection(connectionString), true)
        {
        }
        internal MoleSqlQueryProvider([NotNull] SqlConnection connection) : this (connection, false)
        {
        }
        internal MoleSqlQueryProvider([NotNull] SqlConnection connection, bool ownConnection)
        {
            disposeConnection = ownConnection;
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }
        public void Dispose()
        {
            if (disposed) return;
            if (disposeConnection) connection.Dispose();
            disposed = true;
        }

        public SqlTransaction BeginTransaction()
        {
            OpenConnection();
            return connection.BeginTransaction();
        }
        public SqlTransaction BeginTransaction(string transactionName)
        {
            OpenConnection();
            return connection.BeginTransaction(transactionName);
        }
        public SqlTransaction BeginTransaction(IsolationLevel iso)
        {
            OpenConnection();
            return connection.BeginTransaction(iso);
        }
        public SqlTransaction BeginTransaction(IsolationLevel iso, string transactionName)
        {
            OpenConnection();
            return connection.BeginTransaction(iso, transactionName);
        }

        public IEnumerable<T> ExecuteQuery<T>(FormattableString query) where T : class, new()
        {
            CheckDisposed();

            using var cmd = connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            OpenConnection();
            return cmd.ExecuteReader().ReadObjects<T>();
        }
        public async IAsyncEnumerable<T> ExecuteQueryAsync<T>(FormattableString query, [EnumeratorCancellation] CancellationToken cancellationToken = default) where T : class, new()
        {
            CheckDisposed();

            using var cmd = connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
            await foreach (var element in (await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false)).ReadObjectsAsync<T>(
                cancellationToken))
                yield return element;
        }
        public IEnumerable ExecuteQuery(FormattableString query)
        {
            CheckDisposed();

            using var cmd = connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            OpenConnection();
            return cmd.ExecuteReader().ReadObjects();
        }
        public async IAsyncEnumerable<dynamic> ExecuteQueryAsync(FormattableString query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            using var cmd = connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
            await foreach (var element in (await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false)).ReadObjectsAsync(cancellationToken))
                yield return element;
        }
        public object ExecuteScalar(FormattableString query)
        {
            CheckDisposed();

            using var cmd = connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            OpenConnection();
            return cmd.ExecuteScalar();
        }
        public T ExecuteScalar<T>(FormattableString query)
        {
            CheckDisposed();

            using var cmd = connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            OpenConnection();
            return (T)cmd.ExecuteScalar();
        }
        public async Task<object> ExecuteScalarAsync(FormattableString query, CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            using var cmd = connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
            return await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        }
        public async Task<T> ExecuteScalarAsync<T>(FormattableString query, CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            using var cmd = connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
            return (T)await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        }
        public int ExecuteNonQuery(FormattableString query)
        {
            CheckDisposed();

            using var cmd = connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            OpenConnection();
            return cmd.ExecuteNonQuery();
        }
        public async Task<int> ExecuteNonQueryAsync(FormattableString query, CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            using var cmd = connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
            return await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        [SuppressMessage("Microsoft.Security", "CA2100", Justification = "This is what this is all about.")]
        public override object Execute(Expression expression)
        {
            CheckDisposed();

            using var cmd = connection.CreateCommand();
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

            using var cmd = connection.CreateCommand();
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

            using var cmd = connection.CreateCommand();
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
            if (disposed) throw new ObjectDisposedException(nameof(MoleSqlQueryProvider), "The query provider has already been disposed of.");
        }
        void OpenConnection()
        {
            CheckDisposed();
            if (connection.State == ConnectionState.Closed)
                connection.Open();
        }
        async ValueTask OpenConnectionAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();
            if (connection.State == ConnectionState.Closed)
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
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
