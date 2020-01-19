/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System.Linq.Expressions;

namespace MoleSql.Expressions
{
    sealed class OrderClause
    {
        internal OrderType OrderType { get; }
        internal Expression Expression { get; }

        public OrderClause(OrderType orderType, Expression expression)
        {
            OrderType = orderType;
            Expression = expression;
        }

        public override string ToString() => $"({Expression}) {OrderType}";
    }
}
