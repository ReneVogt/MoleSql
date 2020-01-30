/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleSql;

namespace MoleSqlTests.TestDb
{
    [ExcludeFromCodeCoverage]
    class TestDbContext : DataContext
    {
        static readonly string ProviderInfo = typeof(QueryProvider).FullName + " v" + typeof(QueryProvider).Assembly.GetName().Version;
        internal readonly string ContextInfo;

        internal static string ConnectionString => ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString;
        readonly bool disposeTransaction;
        internal Query<Customers> Customers => GetTable<Customers>();
        internal Query<Employees> Employees => GetTable<Employees>();
        internal Query<Orders> Orders => GetTable<Orders>();
        internal Query<Departments> Departments => GetTable<Departments>();
        internal Query<Products> Products => GetTable<Products>();
        internal Query<ProductsToOrders> ProductsToOrders => GetTable<ProductsToOrders>();
        internal Query<AggregatorTestTable> AggregatorTest => GetTable<AggregatorTestTable>();
        internal Query<NullableTestTable> NullableTest => GetTable<NullableTestTable>();

        internal StringBuilder LogBuilder { get; } = new StringBuilder();

        public TestDbContext(TestContext testContext, bool disposeTransaction = false)
            : base(ConnectionString)
        {
            ContextInfo = $"-- Context: {ProviderInfo}, {Connection.DataSource}\\{Connection.Database}";
            this.disposeTransaction = disposeTransaction;
            Log = new SqlLogger(testContext, LogBuilder);
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
