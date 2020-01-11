/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Unit-Tests for the MoleSql.Helpers.TypeSystemhelper.GetSequenceType.
 *
 */

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql.Helpers;

namespace MoleSqlTests.UnitTests
{
    public partial class TestTypeSystemHelper : MoleSqlTestBase
    {
        [TestMethod]
        public void GetSequenceType_ValueType_ReturnsIEnumerable()
        {
            Type result = TypeSystemHelper.GetSequenceType(typeof(int));
            result.Should().Be(typeof(IEnumerable<int>));

        }
        [TestMethod]
        public void GetSequenceType_ReferenceType_ReturnsIEnumerable()
        {
            Type result = TypeSystemHelper.GetSequenceType(typeof(TestTypeSystemHelper));
            result.Should().Be(typeof(IEnumerable<TestTypeSystemHelper>));

        }
    }
}
