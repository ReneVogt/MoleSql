using System.Diagnostics.CodeAnalysis;

namespace MoleSqlTests.TestDb
{
    [ExcludeFromCodeCoverage]
    class Customers
    {
        public string Name { get; set; }
        public override string ToString() => $"Custoemr '{Name}'";
    }
}
