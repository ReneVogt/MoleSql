namespace MoleSqlTests.TestDb
{
    class Customers
    {
        public string Name { get; set; }
        public override string ToString() => $"Custoemr '{Name}'";
    }
}
