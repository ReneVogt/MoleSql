/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */
using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using MoleSql;

namespace MoleSqlTests.TestDb
{
    [ExcludeFromCodeCoverage]
    class TestDbContext : MoleSqlDataContext
    {
        readonly bool disposeTransaction;
        internal MoleSqlQuery<Customers> Customers => GetTable<Customers>();
        internal MoleSqlQuery<Employees> Employees => GetTable<Employees>();
        internal MoleSqlQuery<Orders> Orders => GetTable<Orders>();
        internal MoleSqlQuery<Departments> Departments => GetTable<Departments>();
        internal MoleSqlQuery<Products> Products => GetTable<Products>();
        internal MoleSqlQuery<ProductsToOrders> ProductsToOrders => GetTable<ProductsToOrders>();

        public TestDbContext(bool disposeTransaction = false)
            : base(ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString)
        {
            this.disposeTransaction = disposeTransaction;
            Log = Console.Out;

            if (!disposeTransaction) return;
            Transaction = BeginTransaction();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && disposeTransaction) Transaction?.Dispose();
        }
    }
}
