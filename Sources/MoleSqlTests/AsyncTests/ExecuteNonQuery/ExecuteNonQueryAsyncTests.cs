﻿using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MoleSqlTests.AsyncTests.ExecuteNonQuery
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ExecuteNonQueryAsyncTests
    {
        [TestMethod]
        public async Task ExecuteNonQueryAsync_YieldsCorrectResult()
        {
            using var context = MoleSqlTestContext.GetDbContextWithTransaction();
            
            int result = await context.ExecuteNonQueryAsync($"DELETE FROM Customers WHERE Name = 'Alfred'");
            result.Should().Be(1);
        }
    }
}
