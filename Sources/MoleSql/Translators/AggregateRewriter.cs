/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MoleSql.Expressions;

namespace MoleSql.Translators
{
    sealed class AggregateRewriter : DbExpressionVisitor
    {
        sealed class AggregateGatherer : DbExpressionVisitor
        {
            readonly List<AggregateSubQueryExpression> aggregates = new List<AggregateSubQueryExpression>();
            
            AggregateGatherer() { }
            
            protected override Expression VisitAggregateSubQuery(AggregateSubQueryExpression aggregateSubQueryExpression)
            {
                aggregates.Add(aggregateSubQueryExpression);
                return base.VisitAggregateSubQuery(aggregateSubQueryExpression);
            }

            internal static List<AggregateSubQueryExpression> Gather(Expression expression)
            {
                AggregateGatherer gatherer = new AggregateGatherer();
                gatherer.Visit(expression);
                return gatherer.aggregates;
            }
        }

        readonly ILookup<string, AggregateSubQueryExpression> lookup;
        readonly Dictionary<AggregateSubQueryExpression, Expression> map;

        AggregateRewriter(Expression expression)
        {
            map = new Dictionary<AggregateSubQueryExpression, Expression>(); 
            lookup = AggregateGatherer.Gather(expression).ToLookup(a => a.GroupByAlias);
        }

        protected override Expression VisitSelect(SelectExpression selectExpression)
        {
            selectExpression = (SelectExpression)base.VisitSelect(selectExpression);
            if (!lookup.Contains(selectExpression.Alias)) return selectExpression;

            List<ColumnDeclaration> aggregateColumns = new List<ColumnDeclaration>(selectExpression.Columns);
            foreach (AggregateSubQueryExpression aggregateSubQueryExpression in lookup[selectExpression.Alias])
            {
                string name = "agg" + aggregateColumns.Count;
                ColumnDeclaration columnDeclaration = new ColumnDeclaration(name, aggregateSubQueryExpression.AggregateInGroupSelect);
                map.Add(aggregateSubQueryExpression, new ColumnExpression(aggregateSubQueryExpression.Type, aggregateSubQueryExpression.GroupByAlias, name));
                aggregateColumns.Add(columnDeclaration);
            }

            return new SelectExpression(
                selectExpression.Type, 
                selectExpression.Alias, 
                aggregateColumns, 
                selectExpression.From, 
                selectExpression.Where, 
                selectExpression.OrderBy, 
                selectExpression.GroupBy);
        }
        protected override Expression VisitAggregateSubQuery(AggregateSubQueryExpression aggregateSubQueryExpression) =>
            map.TryGetValue(aggregateSubQueryExpression, out var mapped)
                ? mapped
                : Visit(aggregateSubQueryExpression.AggregateAsSubQuery);

        internal static Expression Rewrite(Expression expression) => new AggregateRewriter(expression).Visit(expression);
    }
}
