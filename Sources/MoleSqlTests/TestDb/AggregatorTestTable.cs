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
    class AggregatorTestTable
    {
        public int IntValue { get; set; }
        public long LongValue { get; set; }
        public float FloatValue { get; set; }
        public double DoubleValue { get; set; }
        public decimal DecimalValue { get; set; }

        public int? NullableIntValue { get; set; }
        public long? NullableLongValue { get; set; }
        public float? NullableFloatValue { get; set; }
        public double? NullableDoubleValue { get; set; }
        public decimal? NullableDecimalValue { get; set; }

        public override string ToString() => IntValue.ToString(CultureInfo.InvariantCulture);
    }
}
