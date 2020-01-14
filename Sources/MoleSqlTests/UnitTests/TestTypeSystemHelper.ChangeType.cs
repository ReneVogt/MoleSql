/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Unit-Tests for the MoleSql.Helpers.TypeSystemhelper.GetSequenceType.
 *
 */

using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql.Helpers;
// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace MoleSqlTests.UnitTests
{
    public partial class TestTypeSystemHelper : MoleSqlTestBase
    {
        [TestMethod]
        public void ChangeType_Null_Null()
        {
            TypeSystemHelper.ChangeType(null, typeof(int?)).Should().BeNull();
            TypeSystemHelper.ChangeType(null, typeof(object)).Should().BeNull();
        }
        [TestMethod]
        public void ChangeType_Null_NonNullableValueType_Exception()
        {
            typeof(int).Invoking(t => TypeSystemHelper.ChangeType(null, t))
                       .Should()
                       .Throw<InvalidCastException>()
                       .WithMessage($"*{typeof(int).FullName}*");
        }
        [TestMethod]
        public void ChangeType_AlreadyCorrectValueType_Same()
        {
            const decimal d = 5;
            object result = TypeSystemHelper.ChangeType(d, typeof(decimal));
            result.Should().BeOfType<decimal>();
            result.Should().Be(5);
        }
        [TestMethod]
        public void ChangeType_AlreadyCorrectReferenceType_Same()
        {
            var test = new TestTypeSystemHelper();
            object result = TypeSystemHelper.ChangeType(test, typeof(TestTypeSystemHelper));
            result.Should().BeSameAs(test);
        }
        [TestMethod]
        public void ChangeType_NonGeneric_ChangedType()
        {
            object result = TypeSystemHelper.ChangeType(5, typeof(decimal));
            result.Should().Be(5);
            result.Should().BeOfType<decimal>();
        }
        [TestMethod]
        public void ChangeType_ToNullable_Works()
        {
            const decimal test = 5;
            decimal? result = TypeSystemHelper.ChangeType<decimal?>(test);
            result.Should().Be(5);
        }
        [TestMethod]
        public void ChangeType_ToOtherNullable_Works()
        {
            const decimal test = 5;
            int? result = TypeSystemHelper.ChangeType<int?>(test);
            result.Should().Be(5);
        }
        [TestMethod]
        public void ChangeType_FromNullable_Works()
        {
            decimal? test = 5;
            object result = TypeSystemHelper.ChangeType(test, typeof(decimal));
            result.Should().Be(5);
            result.Should().BeOfType<decimal>();
        }
        [TestMethod]
        public void ChangeType_FromOtherNullable_Works()
        {
            decimal? test = 5;
            object result = TypeSystemHelper.ChangeType(test, typeof(int));
            result.Should().Be(5);
            result.Should().BeOfType<int>();
        }
    }
}
