using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSqlTests.TestDb;

namespace MoleSqlTests.ExecuteNonQuery
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ExecuteNonQueryTests
    {
        TestDbContext context;

        [TestInitialize]
        public void TestInitialize()
        {
            context = new TestDbContext();
            context.Transaction = context.BeginTransaction();
        }
        [TestCleanup]
        public void TestCleanUp()
        {
            context.Transaction.Dispose();
            context.Dispose();
        }

        [TestMethod]
        public void DeleteRows_CorrectRowCount()
        {
            context.ExecuteNonQuery($"DELETE FROM Customers WHERE [Name] = 'Alfred'").Should().Be(1);
        }
    }
}
