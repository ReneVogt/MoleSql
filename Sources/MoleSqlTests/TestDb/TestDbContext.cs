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
    class TestDbContext : DataContext
    {
        readonly bool disposeTransaction;
        internal Query<Customers> Customers => GetTable<Customers>();
        internal Query<Employees> Employees => GetTable<Employees>();
        internal Query<Orders> Orders => GetTable<Orders>();
        internal Query<Departments> Departments => GetTable<Departments>();
        internal Query<Products> Products => GetTable<Products>();
        internal Query<ProductsToOrders> ProductsToOrders => GetTable<ProductsToOrders>();

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
