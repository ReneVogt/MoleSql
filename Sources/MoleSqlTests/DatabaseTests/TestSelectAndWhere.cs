/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MoleSqlTests.DatabaseTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestSelectAndWhere : MoleSqlTestBase
    {
        [TestMethod]
        public void SelectAndWhere_WithNullCheck()
        {
            using var context = GetDbContext();
            var query = from customer in context.Customers
                        where customer.Id < 4 && customer.Name != null
                        select customer.Id;
            var result = query.AsEnumerable().OrderBy(i => i).ToList();
            result.Should().Equal(1, 2, 3);
            AssertSql(context, $"SELECT [t0].[Id] FROM [Customers] AS t0 WHERE (([t0].[Id] < @p0) AND ([t0].[Name] IS NOT NULL)) -- @p0 Input Int (Size = 0; Prec = 0; Scale = 0) [4] {context.ContextInfo}");
        }
        [TestMethod]
        public void SelectAndWhere_ValueTypes()
        {
            using var context = GetDbContext();
            var query = from customer in context.Customers
                        where customer.Id < 4
                        select customer.Id;
            var result = query.AsEnumerable().OrderBy(i => i).ToList();
            result.Should().Equal(1, 2, 3);
            AssertSql(context, $"SELECT [t0].[Id] FROM [Customers] AS t0 WHERE ([t0].[Id] < @p0) -- @p0 Input Int (Size = 0; Prec = 0; Scale = 0) [4] {context.ContextInfo}");
        }

        sealed class TestTable
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public TestTable() { }
            public TestTable(int id, string name)
            {
                ID = id;
                Name = name;
            }
        }
        [TestMethod]
        public void Select_ProjectionWithConstructor()
        {
            using var context = GetDbContext();
            var query = from customer in context.Customers
                        where customer.Id < 2
                        select new TestTable(customer.Id, customer.Name);
            var result = query.AsEnumerable().Single();
            result.ID.Should().Be(1);
            result.Name.Should().Be("Alfons Allerlei");
            AssertSql(context, $"SELECT [t0].[Id], [t0].[Name] FROM [Customers] AS t0 WHERE ([t0].[Id] < @p0) -- @p0 Input Int (Size = 0; Prec = 0; Scale = 0) [2] {context.ContextInfo}");
        }
        [TestMethod]
        public void Select_ProjectionWithMemberInit()
        {
            using var context = GetDbContext();
            var query = from customer in context.Customers
                        where customer.Id < 2
                        select new TestTable { ID = customer.Id, Name = customer.Name };
            var result = query.AsEnumerable().Single();
            result.ID.Should().Be(1);
            result.Name.Should().Be("Alfons Allerlei");
            AssertSql(context, $"SELECT [t0].[Id], [t0].[Name] FROM [Customers] AS t0 WHERE ([t0].[Id] < @p0) -- @p0 Input Int (Size = 0; Prec = 0; Scale = 0) [2] {context.ContextInfo}");
        }
        [TestMethod]
        public void Select_Coalesce()
        {
            using var context = GetDbContext();
            var query = from customer in context.Customers
                        where customer.Name == "WithNullAddress"
                        select new {customer.Name, City = customer.City ?? "ThisCityWasNull"};
            var result = query.AsEnumerable().Single();
            result.Name.Should().Be("WithNullAddress");
            result.City.Should().Be("ThisCityWasNull");
            AssertSql(context, $"SELECT [t0].[Name], [t0].[City] FROM [Customers] AS t0 WHERE ([t0].[Name] = @p0) -- @p0 Input NVarChar (Size = 15; Prec = 0; Scale = 0) [WithNullAddress] {context.ContextInfo}");
        }
        [TestMethod]
        public void Select_WhereWithMethodCall()
        {
            using var context = GetDbContext();
            var query = from customer in context.Customers
                        where customer.Name == GetName()
                        select customer.Name;
            var result = query.AsEnumerable().Single();
            result.Should().Be("WithNullAddress");

            AssertSql(context, $"SELECT [t0].[Name] FROM [Customers] AS t0 WHERE ([t0].[Name] = @p0) -- @p0 Input NVarChar (Size = 15; Prec = 0; Scale = 0) [WithNullAddress] {context.ContextInfo}");
        }
        [TestMethod]
        public void Select_WithMethodCall()
        {
            using var context = GetDbContext();
            var query = from customer in context.Customers
                        where customer.Name == GetName()
                        select GetLength(customer.Name);
            var result = query.AsEnumerable().Single();
            result.Should().Be(15);

            AssertSql(context, $"SELECT [t0].[Name] FROM [Customers] AS t0 WHERE ([t0].[Name] = @p0) -- @p0 Input NVarChar (Size = 15; Prec = 0; Scale = 0) [WithNullAddress] {context.ContextInfo}");
        }
        static string GetName() => "WithNullAddress";
        static int GetLength(string s) => s.Length;
    }
}
