using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using MoleSql.Mapper;
using MoleSql.Translators.Sql;

namespace MoleSql.QueryProviders {
    sealed class SqlQueryProvider : MoleQueryProvider
    {
        readonly SqlConnection connection;

        public SqlQueryProvider([NotNull] SqlConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public override string GetQueryText(Expression expression) => Translate(expression).sql;
        [SuppressMessage("Microsoft.Security", "CA2100", Justification = "This is what this is all about.")]
        public override object Execute(Expression expression)
        {
            using var cmd = connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)Transaction;

            (string sql, LambdaExpression projection) = Translate(expression);
            cmd.CommandText = sql;

            LogCommand(cmd);

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            Type elementType = TypeSystem.GetElementType(expression.Type);

            return SqlObjectReader.GetReader(elementType, projection, reader);
        }
        public override IEnumerable<T> ExecuteQuery<T>(FormattableString query)
        {
            using var cmd = connection.CreateParameterizedCommand(query);
            cmd.Transaction = (SqlTransaction)Transaction;
            LogCommand(cmd);
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            return (IEnumerable<T>)SqlObjectReader.GetReader(typeof(T), null, cmd.ExecuteReader());
        }
        public override IEnumerable ExecuteQuery(FormattableString query)
        {
            using var cmd = connection.CreateParameterizedCommand(query);
            cmd.Transaction = (SqlTransaction)Transaction;
            LogCommand(cmd);
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            return (IEnumerable)SqlObjectReader.GetReader(null, null, cmd.ExecuteReader());
        }
        public override int ExecuteNonQuery(FormattableString query)
        {
            using var cmd = connection.CreateParameterizedCommand(query);
            cmd.Transaction = (SqlTransaction)Transaction;
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
                logbuilder.AppendLine($"- {parameter.ParameterName} {parameter.SqlDbType} {parameter.Direction} [{parameter.Value}]");

            log.Write(logbuilder.ToString());
        }
        static (string sql, LambdaExpression projection) Translate(Expression expression) => new SqlQueryTranslator().Translate(expression);

    }
}
