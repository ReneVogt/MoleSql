/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace MoleSqlTests.TestDb
{
    [ExcludeFromCodeCoverage]
    class NullableTestTable
    {
        public int? IntValue { get; set; }
        public long? LongValue { get; set; }
        public float? FloatValue { get; set; }
        public double? DoubleValue { get; set; }
        public decimal? DecimalValue { get; set; }

        public override string ToString() => IntValue?.ToString(CultureInfo.InvariantCulture) ?? "null";
    }
}
