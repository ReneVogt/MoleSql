using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using JetBrains.Annotations;
using MoleSql.Mapper;
using MoleSql.Translators;

namespace MoleSql.QueryProviders {
    sealed class SqlQueryProvider : MoleQueryProvider
    {
        readonly SqlConnection connection;

        public SqlQueryProvider([NotNull] SqlConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public override string GetQueryText(Expression expression) => Translate(expression);
        public override object Execute(Expression expression)
        {
            using var cmd = connection.CreateCommand();

            string sql = Translate(expression);
            Log?.WriteLine(sql);
            cmd.CommandText = sql;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            Type elementType = TypeSystem.GetElementType(expression.Type);

            return SqlObjectReader.GetReaderMethod(elementType).Invoke(null, new object[] {reader});
        }

        static string Translate(Expression expression) => new SqlQueryTranslator().Translate(expression);
    }
}
