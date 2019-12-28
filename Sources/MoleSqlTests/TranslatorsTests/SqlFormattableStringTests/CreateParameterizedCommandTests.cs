using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql.Translators;

namespace MoleSqlTests.TranslatorsTests.SqlFormattableStringTests
{
    public partial class SqlFormattableStringTests
    {
        [TestMethod]
        public void CreateParameterizedCommand_NoParameters_PlainText()
        {
            const string sql = "SELECT * FROM TableName";
            using var connection = new SqlConnection();
            using var command = connection.CreateParameterizedCommand(FormattableStringFactory.Create(sql));
            command.CommandText.Should().Be(sql);
            command.Parameters.Should().HaveCount(0);
        }
    }
}
