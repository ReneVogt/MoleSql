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
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
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

        public SqlTransaction BeginTransaction() => connection.BeginTransaction();
        public SqlTransaction BeginTransaction(string transactionName) => connection.BeginTransaction(transactionName);
        public SqlTransaction BeginTransaction(IsolationLevel iso) => connection.BeginTransaction(iso);
        public SqlTransaction BeginTransaction(IsolationLevel iso, string transactionName) => connection.BeginTransaction(iso, transactionName);

        [SuppressMessage("Microsoft.Security", "CA2100", Justification = "This is what this is all about.")]
        public override object Execute(Expression expression)
        {
            CheckDisposed();

            using var cmd = connection.CreateCommand();
            cmd.Transaction = Transaction;

            var (sql, projection, parameters, aggregator) = SqlQueryTranslator.Translate(expression);
            
            cmd.CommandText = sql;
            parameters.ForEach(p => cmd.Parameters.AddWithValue(p.name, p.value));

            LogCommand(cmd);

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            Type elementType = TypeSystemHelper.GetElementType(expression.Type);
            var projector = projection.Compile();

            IEnumerable sequence = (IEnumerable)Activator.CreateInstance(
                typeof(ProjectionReader<>).MakeGenericType(elementType),
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] { reader, projector, this },
                null);

            if (aggregator == null) return sequence;

            Delegate aggregatorDelegate = aggregator.Compile();
            AggregateReader aggReader = (AggregateReader)Activator.CreateInstance(
                typeof(AggregateReader<,>).MakeGenericType(elementType, aggregator.Body.Type),
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] { aggregatorDelegate },
                null
            );

            return aggReader.Read(sequence);
        }
        public IEnumerable<T> ExecuteQuery<T>(FormattableString query) where T : class, new()
        {
            CheckDisposed();

            using var cmd = connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            return cmd.ExecuteReader().ReadObjects<T>();
        }
        public async Task<IAsyncEnumerable<T>> ExecuteQueryAsync<T>(FormattableString query, CancellationToken cancellationToken = default) where T : class, new()
        {
            CheckDisposed();

            using var cmd = connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            if (connection.State == ConnectionState.Closed)
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            return (await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false)).ReadObjectsAsync<T>(cancellationToken);
        }
        public IEnumerable ExecuteQuery(FormattableString query)
        {
            CheckDisposed();

            using var cmd = connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            return cmd.ExecuteReader().ReadObjects();
        }
        public int ExecuteNonQuery(FormattableString query)
        {
            CheckDisposed();

            using var cmd = connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            return cmd.ExecuteNonQuery();
        }
        public async Task<int> ExecuteNonQueryAsync(FormattableString query, CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            using var cmd = connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            if (connection.State == ConnectionState.Closed)
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            return await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
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
    }
}
