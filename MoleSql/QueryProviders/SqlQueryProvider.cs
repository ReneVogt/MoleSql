using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
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
        public override object Execute(Expression expression)
        {
            using var cmd = connection.CreateCommand();

            (string sql, LambdaExpression projection) = Translate(expression);
            Log?.WriteLine(sql);
            cmd.CommandText = sql;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            Type elementType = TypeSystem.GetElementType(expression.Type);

            return SqlObjectReader.GetReader(elementType, projection, reader);
        }
        static (string sql, LambdaExpression projection) Translate(Expression expression) => new SqlQueryTranslator().Translate(expression);

    }
}
