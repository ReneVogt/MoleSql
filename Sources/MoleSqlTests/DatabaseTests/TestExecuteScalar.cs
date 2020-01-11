﻿/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql;

namespace MoleSqlTests.DatabaseTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestExecuteScalar : MoleSqlTestBase
    {
        [TestMethod]
        public void ExecuteScalar_NonGeneric_AfterDispose_ObjectDisposedException()
        {
            var context = GetDbContext();
            context.Dispose();
            context.Invoking(ctx => ctx.ExecuteScalar($""))
                   .Should()
                   .Throw<ObjectDisposedException>()
                   .Where(e => e.ObjectName == nameof(QueryProvider));
        }
        [TestMethod]
        public void ExecuteScalarAsync_NonGeneric_AfterDispose_ObjectDisposedException()
        {
            var context = GetDbContext();
            context.Dispose();
            context.Awaiting(async ctx => await ctx.ExecuteScalarAsync($""))
                   .Should()
                   .Throw<ObjectDisposedException>()
                   .Where(e => e.ObjectName == nameof(QueryProvider));
        }
        [TestMethod]
        public void ExecuteScalar_Generic_AfterDispose_ObjectDisposedException()
        {
            var context = GetDbContext();
            context.Dispose();
            context.Invoking(ctx => ctx.ExecuteScalar<int>($""))
                   .Should()
                   .Throw<ObjectDisposedException>()
                   .Where(e => e.ObjectName == nameof(QueryProvider));
        }
        [TestMethod]
        public void ExecuteScalarAsync_Generic_AfterDispose_ObjectDisposedException()
        {
            var context = GetDbContext();
            context.Dispose();
            context.Awaiting(async ctx => await ctx.ExecuteScalarAsync<int>($""))
                   .Should()
                   .Throw<ObjectDisposedException>()
                   .Where(e => e.ObjectName == nameof(QueryProvider));
        }
        [TestMethod]
        public void ExecuteScalar_NonGeneric_CorrectCount()
        {
            using var context = GetDbContext();
            var result = context.ExecuteScalar($"SELECT COUNT([Id]) FROM (SELECT TOP 4 [Id] FROM Employees) AS X");
            result.Should().Be(4);
        }
        [TestMethod]
        public void ExecuteScalar_Generic_CorrectCount()
        {
            using var context = GetDbContext();
            var result = context.ExecuteScalar<int>($"SELECT COUNT([Id]) FROM (SELECT TOP 4 [Id] FROM Employees) AS X");
            result.Should().Be(4);
        }
        [TestMethod]
        public async Task ExecuteScalarAsync_NonGeneric_CorrectCount()
        {
            using var context = GetDbContext();
            var result = await context.ExecuteScalarAsync($"SELECT COUNT([Id]) FROM (SELECT TOP 4 [Id] FROM Employees) AS X");
            result.Should().Be(4);
        }
        [TestMethod]
        public async Task ExecuteScalarAsync_Generic_CorrectCount()
        {
            using var context = GetDbContext();
            var result = await context.ExecuteScalarAsync<int>($"SELECT COUNT([Id]) FROM (SELECT TOP 4 [Id] FROM Employees) AS X");
            result.Should().Be(4);
        }
        [TestMethod]
        public async Task ExecuteScalarAsync_StringParameter_CorrectResults()
        {
            const string departmentName = "Sales";
            using var context = GetDbContext();
            // ReSharper disable once InterpolatedStringExpressionIsNotIFormattable
            var result = await context.ExecuteScalarAsync<int>($"SELECT [Id] FROM Departments WHERE [Name] = {departmentName:NVarChar}");
            result.Should().Be(2);
            AssertSql(context, @"
SELECT [Id] FROM Departments WHERE [Name] = @p0
-- @p0 NVarChar Input [Sales]");
        }
        [TestMethod]
        public async Task ExecuteScalarAsync_IntParameter_CorrectResults()
        {
            using var context = GetDbContext();
            var result = await context.ExecuteScalarAsync<int>($"SELECT [Id] FROM Departments WHERE [Id] = {2:Int}");
            result.Should().Be(2);
            AssertSql(context, @"
SELECT [Id] FROM Departments WHERE [Id] = @p0
-- @p0 Int Input [2]");
        }
    }
}
