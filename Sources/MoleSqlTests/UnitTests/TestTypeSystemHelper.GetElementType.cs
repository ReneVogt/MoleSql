/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Unit-Tests for the MoleSql.Helpers.TypeSystemhelper.GetSequenceType.
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        class TestTypeWithIQueryableInterface : IQueryable<decimal>
        {
            IEnumerator<decimal> IEnumerable<decimal>.GetEnumerator() => null;
            IEnumerator IEnumerable.GetEnumerator() => null;
            Expression IQueryable.Expression { get; }
            Type IQueryable.ElementType { get; }
            IQueryProvider IQueryable.Provider { get; }
        }
        class DerivedFromBaseWithIQueryableInterface : TestTypeWithIQueryableInterface
        { }

        [TestMethod]
        public void GetElementType_Null_Null() => Test(null, null);
        [TestMethod]
        public void GetElementType_String_String() => Test(typeof(string), typeof(string));
        [TestMethod]
        public void GetElementType_ValueTypeNoSequence_Same() => Test(typeof(int), typeof(int));
        [TestMethod]
        public void GetElementType_ReferenceTypeNoSequence_Same() => Test(typeof(TestTypeSystemHelper), typeof(TestTypeSystemHelper));
        [TestMethod]
        public void GetElementType_ArrayOfValueType_ValueType() => Test(typeof(int[]), typeof(int));
        [TestMethod]
        public void GetElementType_ArrayOfReferenceType_ReferenceType() => Test(typeof(TestTypeSystemHelper[]), typeof(TestTypeSystemHelper));
        [TestMethod]
        public void GetElementType_IEnumerableOfValueType_ValueType() => Test(typeof(IEnumerable<int>), typeof(int));
        [TestMethod]
        public void GetElementType_IEnumerableOfReferenceType_ReferenceType() => Test(typeof(IEnumerable<TestTypeSystemHelper>), typeof(TestTypeSystemHelper));
        [TestMethod]
        public void GetElementType_IQueryableOfValueType_ValueType() => Test(typeof(IQueryable<int>), typeof(int));
        [TestMethod]
        public void GetElementType_IQueryableOfReferenceType_ReferenceType() => Test(typeof(IQueryable<TestTypeSystemHelper>), typeof(TestTypeSystemHelper));
        [TestMethod]
        public void GetElementType_TestTypeWithIQueryableInterface_Decimal() => Test(typeof(TestTypeWithIQueryableInterface), typeof(decimal));
        [TestMethod]
        public void GetElementType_DerivedFromBaseeWithIQueryableInterface_Decimal() => Test(typeof(DerivedFromBaseWithIQueryableInterface), typeof(decimal));

        static void Test(Type input, Type expected) => TypeSystemHelper.GetElementType(input).Should().Be(expected);
    }
}
