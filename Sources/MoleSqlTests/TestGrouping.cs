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

namespace MoleSqlTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestGrouping : MoleSqlTestBase
    {
        [TestMethod]
        public async Task SimpleGroup_ProductsByCategory()
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
            AssertAndLogSql(context, @"
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
-- @p0 Int Input [10] -- @p1 Int Input [10]");
        }
        [TestMethod]
        public async Task GroupWithAllAggregators_ProductsByCategory()
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

            AssertAndLogSql(context, @"
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
-- @p0 Int Input [10] 
-- @p1 Int Input [10] 
-- @p2 Decimal Input [17] 
-- @p3 Int Input [10]
-- @p4 Int Input [10]
-- @p5 Int Input [10]
-- @p6 Int Input [10]
-- @p7 Int Input [10]");
        }
    }
}
