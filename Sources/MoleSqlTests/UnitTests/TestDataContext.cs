/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Unit-Tests for the MoleSql.DataContext class.
 *
 */

using System;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql;

namespace MoleSqlTests.UnitTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestDataContext : MoleSqlTestBase
    {
        [TestMethod]
        public void DataContext_ConstructorNullConnection_ArgumentNullException()
        {
            SqlConnection connection = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            connection.Invoking(c => new DataContext(c)).Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "connection");
        }
}
}
