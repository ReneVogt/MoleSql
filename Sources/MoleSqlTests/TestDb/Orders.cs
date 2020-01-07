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
    class Orders
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public decimal Discount { get; set; }
        public override string ToString() => $"Order {Id}: Customer {CustomerId}, Employee {EmployeeId} at {Date.Date} ({Discount})";
    }
}
