using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MoleSqlTests.ExecuteNonQuery
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ExecuteNonQueryTests
    {
        [TestMethod]
        public void DeleteRows_CorrectRowCount()
        {
            const string Name = "Alfred";
            using var context = MoleSqlTestContext.GetDbContext();
            using var transaction = context.Transaction = context.BeginTransaction();
            context.Transaction = transaction;
            context.ExecuteNonQuery($"DELETE FROM Customers WHERE [Name] = {Name}").Should().Be(1);
            MoleSqlTestContext.AssertSqlDump(context,"DELETE FROM Customers WHERE [Name] = @p0 -- @p0 NVarChar Input [Alfred]");
        }
    }
}
