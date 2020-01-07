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
    class Departments
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public override string ToString() => $"Department {Id}: '{Name}'";
    }
}
