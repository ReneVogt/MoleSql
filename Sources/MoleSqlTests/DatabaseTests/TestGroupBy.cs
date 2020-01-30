/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql.Extensions;

namespace MoleSqlTests.DatabaseTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestGroupBy : MoleSqlTestBase
    {
        [TestMethod]
        public async Task GroupBy_SimpleUsingGroupingClass_ProductsByCategory()
        {
            using var context = GetDbContext();
            var query = from product in context.Products
                        where product.Id < 10
                        group product by product.Category
                        into g
                        orderby g.Key
                        select g;

            var result = await query.ToListAsync();
            result.Should().HaveCount(3);
            result[0].Key.Should().Be(1);
            result[0].Count().Should().Be(4);
            result[1].Key.Should().Be(2);
            result[1].Count().Should().Be(2);
            result[2].Key.Should().Be(3);
            result[2].Count().Should().Be(3);

            AssertSql(context, $@"
SELECT [t7].[Category]
FROM (
  SELECT [t0].[Category]
  FROM [Products] AS t0
  WHERE ([t0].[Id] < @p0)
  GROUP BY [t0].[Category]
) AS t7
ORDER BY [t7].[Category]
-- @p0 Input Int (Size = 0; Prec = 0; Scale = 0) [10]
{context.ContextInfo}

SELECT [t3].[Id], [t3].[Name], [t3].[Price], [t3].[Category]
FROM [Products] AS t3
WHERE (([t3].[Id] < @p0) AND (([t3].[Category] IS NULL AND @p1 IS NULL) OR ([t3].[Category] = @p2)))
-- @p0 Input Int (Size = 0; Prec = 0; Scale = 0) [10]
-- @p1 Input Int (Size = 0; Prec = 0; Scale = 0) [1]
-- @p2 Input Int (Size = 0; Prec = 0; Scale = 0) [1]
{context.ContextInfo}

SELECT [t3].[Id], [t3].[Name], [t3].[Price], [t3].[Category]
FROM [Products] AS t3
WHERE (([t3].[Id] < @p0) AND (([t3].[Category] IS NULL AND @p1 IS NULL) OR ([t3].[Category] = @p2)))
-- @p0 Input Int (Size = 0; Prec = 0; Scale = 0) [10]
-- @p1 Input Int (Size = 0; Prec = 0; Scale = 0) [2]
-- @p2 Input Int (Size = 0; Prec = 0; Scale = 0) [2]
{context.ContextInfo}

SELECT [t3].[Id], [t3].[Name], [t3].[Price], [t3].[Category]
FROM [Products] AS t3
WHERE (([t3].[Id] < @p0) AND (([t3].[Category] IS NULL AND @p1 IS NULL) OR ([t3].[Category] = @p2)))
-- @p0 Input Int (Size = 0; Prec = 0; Scale = 0) [10]
-- @p1 Input Int (Size = 0; Prec = 0; Scale = 0) [3]
-- @p2 Input Int (Size = 0; Prec = 0; Scale = 0) [3]
{context.ContextInfo}
");
        }
        [TestMethod]
        public async Task GroupBy_SimpleWithOrderAndSelect_ProductsByCategory()
        {
            using var context = GetDbContext();
            var query = from product in context.Products
                        where product.Id < 10
                        group product by product.Category
                        into g
                        orderby g.Key
                        select g.Count();

            var result = await query.ToListAsync();

            result.Should().Equal(4, 2, 3);
            AssertSql(context, $@"
SELECT 
    ( 
        SELECT COUNT(*) 
        FROM [Products] AS t3 
        WHERE (([t3].[Id] < @p0) AND (([t3].[Category] IS NULL AND [t7].[Category] IS NULL) OR ([t3].[Category] = [t7].[Category]))) 
    ) AS c0 
FROM 
    ( 
        SELECT [t0].[Category] 
        FROM [Products] AS t0 
        WHERE ([t0].[Id] < @p1) 
        GROUP BY [t0].[Category] 
    ) AS t7 
-- @p0 Input Int (Size = 0; Prec = 0; Scale = 0) [10]
-- @p1 Input Int (Size = 0; Prec = 0; Scale = 0) [10]
{context.ContextInfo}
");
        }
        [TestMethod]
        public async Task GroupBy_WithAllAggregators_ProductsByCategory()
        {
            using var context = GetDbContext();
            var query = from product in context.Products
                        where product.Id < 10
                        group product by product.Category
                        into productsByCategory
                        orderby productsByCategory.Key
                        select new
                        {
                            Category = productsByCategory.Key,
                            Count = productsByCategory.Count(),
                            Expensive = productsByCategory.Count(p => p.Price > 17),
                            TotalPrice = productsByCategory.Sum(p => p.Price),
                            MaxPrice = productsByCategory.Max(p => p.Price),
                            MinPrice = productsByCategory.Select(p => p.Price).Min(),
                            AveragePrice = productsByCategory.Select(p => p.Price).Average()
                        };

            var result = await query.ToListAsync();

            result.Should().HaveCount(3);

            result[0].Category.Should().Be(1);
            result[0].Count.Should().Be(4);
            result[0].MinPrice.Should().Be(10);
            result[0].MaxPrice.Should().Be(40);
            result[0].Expensive.Should().Be(3);
            result[0].AveragePrice.Should().Be(25);
            result[0].TotalPrice.Should().Be(100);

            result[1].Category.Should().Be(2);
            result[1].Count.Should().Be(2);
            result[1].MinPrice.Should().Be(15);
            result[1].MaxPrice.Should().Be(20);
            result[1].Expensive.Should().Be(1);
            result[1].AveragePrice.Should().Be(17.5m);
            result[1].TotalPrice.Should().Be(35);

            result[2].Category.Should().Be(3);
            result[2].Count.Should().Be(3);
            result[2].MinPrice.Should().Be(12);
            result[2].MaxPrice.Should().Be(75);
            result[2].Expensive.Should().Be(2);
            result[2].AveragePrice.Should().Be(52.3333m);
            result[2].TotalPrice.Should().Be(157);

            AssertSql(context, $@"
SELECT 
    [t7].[Category], 
    ( 
        SELECT COUNT(*) 
        FROM [Products] AS t3 
        WHERE (([t3].[Id] < @p0) AND (([t3].[Category] IS NULL AND [t7].[Category] IS NULL) OR ([t3].[Category] = [t7].[Category])))
    ) AS c0, 
    ( 
        SELECT COUNT(*) 
        FROM [Products] AS t3 
        WHERE ((([t3].[Id] < @p1) AND (([t3].[Category] IS NULL AND [t7].[Category] IS NULL) OR ([t3].[Category] = [t7].[Category]))) AND ([t3].[Price] > @p2)) 
    ) AS c1, 
    ( 
        SELECT SUM([t3].[Price]) 
        FROM [Products] AS t3 
        WHERE (([t3].[Id] < @p3) AND (([t3].[Category] IS NULL AND [t7].[Category] IS NULL) OR ([t3].[Category] = [t7].[Category]))) 
    ) AS c2, 
    ( 
        SELECT MAX([t3].[Price]) 
        FROM [Products] AS t3 
        WHERE (([t3].[Id] < @p4) AND (([t3].[Category] IS NULL AND [t7].[Category] IS NULL) OR ([t3].[Category] = [t7].[Category]))) 
    ) AS c3, 
    ( 
        SELECT MIN([t3].[Price]) 
        FROM [Products] AS t3 
        WHERE (([t3].[Id] < @p5) AND (([t3].[Category] IS NULL AND [t7].[Category] IS NULL) OR ([t3].[Category] = [t7].[Category]))) 
    ) AS c4, 
    ( 
        SELECT AVG([t3].[Price]) 
        FROM [Products] AS t3 
        WHERE (([t3].[Id] < @p6) AND (([t3].[Category] IS NULL AND [t7].[Category] IS NULL) OR ([t3].[Category] = [t7].[Category]))) 
    ) AS c5 
    FROM ( 
        SELECT [t0].[Category] 
        FROM [Products] AS t0 
        WHERE ([t0].[Id] < @p7) 
        GROUP BY [t0].[Category] 
    ) AS t7 
-- @p0 Input Int (Size = 0; Prec = 0; Scale = 0) [10]
-- @p1 Input Int (Size = 0; Prec = 0; Scale = 0) [10]
-- @p2 Input Decimal (Size = 0; Prec = 0; Scale = 0) [17]
-- @p3 Input Int (Size = 0; Prec = 0; Scale = 0) [10]
-- @p4 Input Int (Size = 0; Prec = 0; Scale = 0) [10]
-- @p5 Input Int (Size = 0; Prec = 0; Scale = 0) [10]
-- @p6 Input Int (Size = 0; Prec = 0; Scale = 0) [10]
-- @p7 Input Int (Size = 0; Prec = 0; Scale = 0) [10]
{context.ContextInfo}
");
        }
        [TestMethod]
        public async Task GroupBy_TotalSalesOfEmployeeId2()
        {
            using var context = GetDbContext();
            var query = from employee in context.Employees
                        where employee.Id == 2
                        join order in context.Orders on employee.Id equals order.EmployeeId
                        join pto in context.ProductsToOrders on order.Id equals pto.OrderId
                        join product in context.Products on pto.ProductId equals product.Id
                        group new {employee.Name, product.Price} by new {employee.Id, employee.Name}
                        into x
                        select new {x.Key.Name, Total = x.Sum(product => product.Price)};
            
            var result = await query.ToListAsync();
            result.Should().HaveCount(1);
            result[0].Name.Should().Be("Marc");
            result[0].Total.Should().Be(130);

            AssertSql(context, $@"
SELECT [t8].[Name], SUM([t9].[Price]) AS agg2 
FROM ( 
    SELECT [t5].[Id], [t5].[Name], [t6].[ProductId] 
    FROM ( 
        SELECT [t2].[Id], [t2].[Name], [t3].[Id] AS Id1 
        FROM ( 
            SELECT [t0].[Id], [t0].[Name] 
            FROM [Employees] AS t0 
            WHERE ([t0].[Id] = @p0) 
        ) AS t2 
    INNER JOIN [Orders] AS t3 
        ON ([t2].[Id] = [t3].[EmployeeId]) 
    ) AS t5 
    INNER JOIN [ProductsToOrders] AS t6 
        ON ([t5].[Id1] = [t6].[OrderId]) 
    ) AS t8 
    INNER JOIN [Products] AS t9 
        ON ([t8].[ProductId] = [t9].[Id]) 
    GROUP BY [t8].[Id], [t8].[Name] 
-- @p0 Input Int (Size = 0; Prec = 0; Scale = 0) [2]
{context.ContextInfo}
");
        }
    }
}
