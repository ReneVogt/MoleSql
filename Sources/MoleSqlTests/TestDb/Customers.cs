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
    class Customers
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public override string ToString() => $"Custoemr '{Name}'";
    }
}
