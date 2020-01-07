/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System;
using System.Diagnostics.CodeAnalysis;

namespace MoleSqlTests.TestDb
{
    [ExcludeFromCodeCoverage]
    class Employees
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? LastSeen { get; set; }
        public decimal Salary { get; set; }
        public override string ToString() => $"Employee {Id}: '{Name}' [{Salary}] ({DateOfBirth}) ({LastSeen})";
    }
}
