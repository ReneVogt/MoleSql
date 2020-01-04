/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System;
using System.Linq.Expressions;

namespace MoleSql.Expressions
{
    sealed class AggregateSubQueryExpression : Expression
    {
        internal string GroupByAlias { get; }
        internal Expression AggregateInGroupSelect { get; }
        internal SubQueryExpression AggregateAsSubQuery { get; }

        public override Type Type { get; }
        public override ExpressionType NodeType { get; }

        internal AggregateSubQueryExpression(string groupByAlias, Expression aggregateInGroupSelect, SubQueryExpression aggregateAsSubQuery)
        {
            AggregateInGroupSelect = aggregateInGroupSelect;
            GroupByAlias = groupByAlias;
            AggregateAsSubQuery = aggregateAsSubQuery;
            Type = aggregateAsSubQuery.Type;
            NodeType = (ExpressionType)DbExpressionType.AggregateSubQuery;
        }

        public override string ToString() => $"AggregateSubQuery '{GroupByAlias}' SELECT: ({AggregateInGroupSelect}) SubQuery: ({AggregateAsSubQuery})";
    }
}
