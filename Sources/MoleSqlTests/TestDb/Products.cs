/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */
using System.Diagnostics.CodeAnalysis;

namespace MoleSqlTests.TestDb
{
    [ExcludeFromCodeCoverage]
    class Products
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Category { get; set; }
        public override string ToString() => $"Product {Id}: '{Name}' ({Category}) ({Price})";
    }
}
