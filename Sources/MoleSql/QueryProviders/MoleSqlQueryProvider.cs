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
using JetBrains.Annotations;
using MoleSql.Mapper;
using MoleSql.Translators;

namespace MoleSql.QueryProviders {
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

        [SuppressMessage("Microsoft.Security", "CA2100", Justification = "This is what this is all about.")]
        public override object Execute(Expression expression)
        {
            CheckDisposed();

            using var cmd = connection.CreateCommand();
            cmd.Transaction = Transaction;

            (string sql, var projection, var parameters) = SqlQueryTranslator.Translate(expression);
            
            cmd.CommandText = sql;
            parameters.ForEach(p => cmd.Parameters.AddWithValue(p.name, p.value));

            LogCommand(cmd);

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            Type elementType = TypeSystem.GetElementType(expression.Type);
            var projector = projection.Compile();

            return Activator.CreateInstance(
                typeof(ProjectionReader<>).MakeGenericType(elementType),
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] { reader, projector },
                null);
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
        public IEnumerable ExecuteQuery(FormattableString query)
        {
            CheckDisposed();

            using var cmd = connection.CreateParameterizedCommand(query);
            cmd.Transaction = Transaction;
            LogCommand(cmd);
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            return new DynamicReader(cmd.ExecuteReader());
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

        void LogCommand(SqlCommand command)
        {
            var log = Log;
            if (log == null) return;

            StringBuilder logbuilder = new StringBuilder();
            logbuilder.AppendLine("Executing SQL:");
            logbuilder.AppendLine(command.CommandText);

            foreach (SqlParameter parameter in command.Parameters)
                logbuilder.AppendLine($"- {parameter.ParameterName} {parameter.SqlDbType} {parameter.Direction} [{parameter.SqlValue}]");

            log.Write(logbuilder.ToString());
        }
        void CheckDisposed()
        {
            if (disposed) throw new ObjectDisposedException(nameof(MoleSqlQueryProvider), "The query provider has already been disposed of.");
        }
    }
}
