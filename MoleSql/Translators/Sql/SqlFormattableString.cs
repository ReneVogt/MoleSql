using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace MoleSql.Translators.Sql
{
    static class SqlFormattableString
    {
        sealed class FormatCapturingParameter : IFormattable
        {
            readonly SqlParameter parameter;

            internal FormatCapturingParameter(SqlParameter parameter)
            {
                this.parameter = parameter;
            }
            public string ToString(string format, IFormatProvider formatProvider)
            {
                if (!string.IsNullOrEmpty(format))
                    parameter.SqlDbType =
                        (SqlDbType)Enum.Parse(typeof(SqlDbType), format, true);
                return parameter.ParameterName;
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2100", Justification = "This is what this is all about.")]
        internal static SqlCommand CreateParameterizedCommand(this SqlConnection connection, FormattableString formattableString)
        {
            SqlParameter[] sqlParameters = formattableString.GetArguments()
                                                            .Select((value, position) =>
                                                                        new SqlParameter(FormattableString.Invariant($"@p{position}"), value))
                                                            .ToArray();
            object[] formatArguments = sqlParameters
                                       .Select(p => (object)new FormatCapturingParameter(p))
                                       .ToArray();
            string sql = string.Format(CultureInfo.InvariantCulture, formattableString.Format, formatArguments);
            var command = new SqlCommand(sql, connection);
            command.Parameters.AddRange(sqlParameters);
            return command;
        }
    }
}
