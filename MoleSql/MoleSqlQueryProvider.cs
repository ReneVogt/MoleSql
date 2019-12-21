using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace MoleSql {
    sealed class MoleSqlQueryProvider : QueryProvider
    {
        readonly SqlConnection connection;

        public TextWriter Log { get; set; }

        public MoleSqlQueryProvider([NotNull] SqlConnection connection)
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

            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            Type elementType = TypeSystem.GetElementType(expression.Type);

            var translatorMethod = typeof(ObjectReader).GetMethod(
                                                           nameof(ObjectReader.ReadObjects),
                                                           BindingFlags.Static | BindingFlags.NonPublic,
                                                           null,
                                                           new[] {typeof(SqlDataReader)},
                                                           null)
                                                       ?.MakeGenericMethod(elementType);
            return translatorMethod?.Invoke(null, new object[] {reader});
        }

        static string Translate(Expression expression) => new QueryTranslator().Translate(expression);
    }
}
