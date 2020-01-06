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
        internal MoleQuery<Customers> Customers => GetTable<Customers>();

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
