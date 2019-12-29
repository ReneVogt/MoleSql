/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
        [TestMethod]
        public void CreateParameterizedCommand_ParametersNoFormat_CorrectlyParameterizedQuery()
        {
            const string text = "Hello World!";
            const long id = 42;
            int[] ids = { 1, 2, 3 };
            using var connection = new SqlConnection();
            using var command = connection.CreateParameterizedCommand($"SELECT TestColumn FROM TestTable WHERE Text = {text} OR ID = {id} OR ID IN {ids}");
            command.CommandText.Should().Be("SELECT TestColumn FROM TestTable WHERE Text = @p0 OR ID = @p1 OR ID IN (@p2, @p3, @p4)");
            command.Parameters.Should().HaveCount(5);
            var results = command.Parameters.Cast<SqlParameter>().ToDictionary(p => p.ParameterName, p => (p.Value, p.SqlDbType));
            results["@p0"].Value.Should().Be(text);
            results["@p0"].SqlDbType.Should().Be(SqlDbType.NVarChar);
            results["@p1"].Value.Should().Be(id);
            results["@p1"].SqlDbType.Should().Be(SqlDbType.BigInt);
            results["@p2"].Value.Should().Be(1);
            results["@p2"].SqlDbType.Should().Be(SqlDbType.Int);
            results["@p3"].Value.Should().Be(2);
            results["@p3"].SqlDbType.Should().Be(SqlDbType.Int);
            results["@p4"].Value.Should().Be(3);
            results["@p4"].SqlDbType.Should().Be(SqlDbType.Int);
        }
        [TestMethod]
        public void CreateParameterizedCommand_ParametersWithFormat_CorrectlyParameterizedQuery()
        {
            const string text = "Hello World!";
            const long id = 42;
            int[] ids = { 1, 2, 3 };
            using var connection = new SqlConnection();
            // ReSharper disable once InterpolatedStringExpressionIsNotIFormattable
            using var command = connection.CreateParameterizedCommand($"SELECT TestColumn FROM TestTable WHERE Text = {text:Text} OR ID = {id:Int} OR ID IN {ids:BigInt}");
            command.CommandText.Should().Be("SELECT TestColumn FROM TestTable WHERE Text = @p0 OR ID = @p1 OR ID IN (@p2, @p3, @p4)");
            command.Parameters.Should().HaveCount(5);
            var results = command.Parameters.Cast<SqlParameter>().ToDictionary(p => p.ParameterName, p => (p.Value, p.SqlDbType));
            results["@p0"].Value.Should().Be(text);
            results["@p0"].SqlDbType.Should().Be(SqlDbType.Text);
            results["@p1"].Value.Should().Be(id);
            results["@p1"].SqlDbType.Should().Be(SqlDbType.Int);
            results["@p2"].Value.Should().Be(1);
            results["@p2"].SqlDbType.Should().Be(SqlDbType.BigInt);
            results["@p3"].Value.Should().Be(2);
            results["@p3"].SqlDbType.Should().Be(SqlDbType.BigInt);
            results["@p4"].Value.Should().Be(3);
            results["@p4"].SqlDbType.Should().Be(SqlDbType.BigInt);
        }
    }
}
