/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql.Extensions;

// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace MoleSqlTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public partial class TestAggregators : MoleSqlTestBase
    {
        [TestMethod]
        public void AsyncAggregator_AfterDisposed_ObjectDisposedException()
        {
            var context = GetDbContext();
            var table = context.AggregatorTest;
            context.Dispose();
            table.Invoking(t => t.MaxAsync()).Should().Throw<ObjectDisposedException>();
        }
    }
}
