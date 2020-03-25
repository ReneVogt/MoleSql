/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MoleSql.Exceptions;
using MoleSql.Expressions;
using MoleSql.Extensions;
using MoleSql.Helpers;
using MoleSql.Mapper;

namespace MoleSql.Translators
{
    sealed class QueryBinder : DbExpressionVisitor
    {
        sealed class GroupByInfo
        {
            internal string Alias { get; }
            internal Expression Element { get; }

            internal GroupByInfo(string alias, Expression element)
            {
                Alias = alias;
                Element = element;
            }
        }

        readonly Dictionary<Type, Func<MethodCallExpression, Expression>> methodTypeHandlers;
        readonly Dictionary<ParameterExpression, Expression> map = new Dictionary<ParameterExpression, Expression>();
        readonly Dictionary<Expression, GroupByInfo> groupByMap = new Dictionary<Expression, GroupByInfo>();
        readonly Expression root;
        List<OrderClause>? thenBys;
        Int32 aliasCount;
        Expression? currentGroupElement;

        QueryBinder(Expression root)
        {
            this.root = root;
            methodTypeHandlers = new Dictionary<Type, Func<MethodCallExpression, Expression>>
            {
                [typeof(Queryable)] = VisitQueryableMethodCall,
                [typeof(Enumerable)] = VisitEnumerableMethodCall,
                [typeof(MoleSqlQueryable)] = VisitMoleSqlQueryableMethodCall
            };
        }

        protected override Expression VisitMethodCall(MethodCallExpression callExpression) =>
            callExpression.Method.DeclaringType != null &&
            methodTypeHandlers.TryGetValue(callExpression.Method.DeclaringType, out var handler)
                ? handler(callExpression)
                : base.VisitMethodCall(callExpression);
        Expression VisitQueryableMethodCall(MethodCallExpression callExpression)
        {
            return callExpression.Method.Name switch
            {
                nameof(Queryable.Where) => BindWhere(callExpression.Type, callExpression.Arguments[0],
                                                     (LambdaExpression)callExpression.Arguments[1].StripQuotes()!),
                nameof(Queryable.Select) => BindSelect(callExpression.Type, callExpression.Arguments[0],
                                                       (LambdaExpression)callExpression.Arguments[1].StripQuotes()!),
                nameof(Queryable.Join) => BindJoin(callExpression.Type,
                                                   callExpression.Arguments[0],
                                                   callExpression.Arguments[1],
                                                   (LambdaExpression)callExpression.Arguments[2].StripQuotes()!,
                                                   (LambdaExpression)callExpression.Arguments[3].StripQuotes()!,
                                                   (LambdaExpression)callExpression.Arguments[4].StripQuotes()!),
                nameof(Queryable.SelectMany) => BindSelectMany(callExpression.Type,
                                                               callExpression.Arguments[0],
                                                               (LambdaExpression)callExpression.Arguments[1].StripQuotes()!,
                                                               callExpression.Arguments.Count > 2
                                                                   ? (LambdaExpression)callExpression.Arguments[2].StripQuotes()!
                                                                   : null),
                nameof(Queryable.OrderBy) => BindOrderBy(callExpression.Type,
                                                         callExpression.Arguments[0],
                                                         (LambdaExpression)callExpression.Arguments[1].StripQuotes()!,
                                                         OrderType.Ascending),
                nameof(Queryable.OrderByDescending) => BindOrderBy(callExpression.Type,
                                                                   callExpression.Arguments[0],
                                                                   (LambdaExpression)callExpression.Arguments[1].StripQuotes()!,
                                                                   OrderType.Descending),
                nameof(Queryable.ThenBy) => BindThenBy(callExpression.Arguments[0],
                                                       (LambdaExpression)callExpression.Arguments[1].StripQuotes()!,
                                                       OrderType.Ascending),
                nameof(Queryable.ThenByDescending) => BindThenBy(callExpression.Arguments[0],
                                                                 (LambdaExpression)callExpression.Arguments[1].StripQuotes()!,
                                                                 OrderType.Descending),
                nameof(Queryable.GroupBy) => BindGroupBy(callExpression.Arguments[0],
                                                         (LambdaExpression)callExpression.Arguments[1].StripQuotes()!,
                                                         callExpression.Arguments.Count > 2
                                                             ? (LambdaExpression?)callExpression.Arguments[2].StripQuotes()
                                                             : null,
                                                         callExpression.Arguments.Count > 3
                                                             ? (LambdaExpression?)callExpression.Arguments[3].StripQuotes()
                                                             : null),
                var name when Enum.GetNames(typeof(AggregateType)).Contains(name) => BindAggregate(
                    callExpression.Arguments[0], callExpression.Method.DeclaringType, GetAggregateType(name), callExpression.Method.ReturnType,
                    callExpression.Method.GetGenericArguments(),
                    callExpression.Arguments.Count > 1 ? (LambdaExpression?)callExpression.Arguments[1].StripQuotes() : null,
                    callExpression == root),
                _ => throw callExpression.Method.IsNotSupported()
            };

        }
        Expression VisitEnumerableMethodCall(MethodCallExpression callExpression) => VisitQueryableMethodCall(callExpression);
        Expression VisitMoleSqlQueryableMethodCall(MethodCallExpression callExpression)
        {
            if (!IsAsyncAggregateMethod(callExpression.Method.Name))
                throw callExpression.Method.IsNotSupported();
            if (callExpression != root)
                throw callExpression.Method.CanOnlyAppearOnTopOfExpressionTree();

            var source = callExpression.Arguments[0];
            var declaringType = callExpression.Method.DeclaringType;
            var aggregateType = GetAggregateType(callExpression.Method.Name.Substring(0, callExpression.Method.Name.Length - 5));
            var returnType = callExpression.Method.ReturnType.GetGenericArguments().First(); // must be Task<T>
            var genericArguments = callExpression.Method.GetGenericArguments();

            return BindAggregate(source, declaringType, aggregateType, returnType, genericArguments,
                                 GetSelectorFromAsyncAggregate(callExpression.Arguments), true);
        }
        protected override Expression VisitConstant(ConstantExpression constant) => IsTable(constant.Value) ? (Expression)GetTableProjection(constant.Value) : constant;
        protected override Expression VisitParameter(ParameterExpression parameter) =>
            map.TryGetValue(parameter, out var expression) ? expression : parameter;
        protected override Expression VisitMember(MemberExpression member)
        {
            Expression? source = Visit(member.Expression);
            if (source == null) return member;

            switch (source.NodeType)
            {
                case ExpressionType.MemberInit:
                    MemberInitExpression memberInit = (MemberInitExpression)source;
                    var memberAssignment = memberInit.Bindings.OfType<MemberAssignment>()
                                                     .FirstOrDefault(assignment => MembersMatch(assignment.Member, member.Member));
                    if (memberAssignment != null) return memberAssignment.Expression;
                    break;
                case ExpressionType.New:
                    NewExpression newExpression = (NewExpression)source;
                    var result = newExpression.Members?.Select((m, i) => new {m, i})
                                              .FirstOrDefault(x => MembersMatch(x.m, member.Member));
                    if (result != null) return newExpression.Arguments[result.i];
                    if (newExpression.Type.IsGenericType && 
                        newExpression.Type.GetGenericTypeDefinition() == typeof(Grouping<,>) &&
                        member.Member.Name == "Key")
                        return newExpression.Arguments[0];
                    var arg = newExpression.Arguments.OfType<ColumnExpression>().FirstOrDefault(column => column.Name == member.Member.Name);
                    if (arg != null) return arg;
                    break;
            }

            if (source == member.Expression) return member;
            return MakeMemberAccess(source, member.Member);
        }

        Expression BindWhere(Type resultType, Expression source, LambdaExpression predicate)
        {
            ProjectionExpression projection = VisitSequence(source)!;
            map[predicate.Parameters[0]] = projection.Projector;
            Expression where = Visit(predicate.Body)!; 
            
            string alias = GetNextAlias();
            var (projector, columns) = ColumnProjector.ProjectColumns(projection.Projector, alias, GetExpressionAlias(projection.Source));

            return new ProjectionExpression(
                new SelectExpression(resultType, alias, columns, projection.Source, where),
                projector);
        }
        Expression BindSelect(Type resultType, Expression source, LambdaExpression selector)
        {
            ProjectionExpression projection = VisitSequence(source);

            map[selector.Parameters[0]] = projection.Projector; 
            Expression expression = Visit(selector.Body)!;

            string alias = GetNextAlias();
            var (projector, columns) = ColumnProjector.ProjectColumns(expression, alias, projection.Source.Alias);

            return new ProjectionExpression(
                new SelectExpression(resultType, alias, columns, projection.Source, null),
                projector);
        }
        Expression BindJoin(Type resultType, Expression outerSource, Expression innerSource, LambdaExpression outerKey, LambdaExpression innerKey, LambdaExpression resultSelector)
        {
            ProjectionExpression outerProjection = VisitSequence(outerSource)!;
            ProjectionExpression innerProjection = VisitSequence(innerSource)!;

            map[outerKey.Parameters[0]] = outerProjection.Projector;
            Expression outerKeyExpression = Visit(outerKey.Body)!;
            Debug.Assert(outerKeyExpression != null);

            map[innerKey.Parameters[0]] = innerProjection.Projector;
            Expression innerKeyExpression = Visit(innerKey.Body)!;
            Debug.Assert(innerKeyExpression != null);

            map[resultSelector.Parameters[0]] = outerProjection.Projector;
            map[resultSelector.Parameters[1]] = innerProjection.Projector;
            Expression resultExpression = Visit(resultSelector.Body)!;

            JoinExpression join = new JoinExpression(
                resultType, 
                JoinType.InnerJoin, 
                outerProjection.Source, 
                innerProjection.Source, 
                Expression.Equal(outerKeyExpression, innerKeyExpression));

            string alias = GetNextAlias();

            var (projector, columns) = ColumnProjector.ProjectColumns(resultExpression, alias, outerProjection.Source.Alias, innerProjection.Source.Alias);

            return new ProjectionExpression(new SelectExpression(resultType, alias, columns, join, null), projector);
        }
        Expression BindSelectMany(Type resultType, Expression source, LambdaExpression collectionSelector, LambdaExpression? resultSelector)
        {
            ProjectionExpression projection = VisitSequence(source)!;
            map[collectionSelector.Parameters[0]] = projection.Projector;

            ProjectionExpression collectionProjection = (ProjectionExpression)Visit(collectionSelector.Body)!;

            JoinType joinType = IsTable(collectionSelector.Body) ? JoinType.CrossJoin : JoinType.CrossApply;
            // ReSharper disable once PossibleNullReferenceException - compiler knows better
            JoinExpression joinExpression = new JoinExpression(resultType, joinType, projection.Source, collectionProjection.Source, null);

            string alias = GetNextAlias();

            Expression projector;
            IReadOnlyCollection<ColumnDeclaration> columns;

            if (resultSelector != null)
            {
                map[resultSelector.Parameters[0]] = projection.Projector;
                map[resultSelector.Parameters[1]] = collectionProjection.Projector;
                Expression resultExpression = Visit(resultSelector.Body)!;

                (projector, columns) = ColumnProjector.ProjectColumns(resultExpression, alias, projection.Source.Alias, collectionProjection.Source.Alias);
            }
            else
                (projector, columns) = ColumnProjector.ProjectColumns(collectionProjection.Projector, alias, projection.Source.Alias,
                                                      collectionProjection.Source.Alias);
            
            return new ProjectionExpression(new SelectExpression(resultType, alias, columns, joinExpression, null), projector);
        }
        Expression BindOrderBy(Type resultType, Expression source, LambdaExpression orderSelector, OrderType orderType)
        {
            var oldThenBys = thenBys;
            thenBys = null;
            oldThenBys?.Reverse();

            ProjectionExpression projection = VisitSequence(source)!;
            map[orderSelector.Parameters[0]] = projection.Projector;

            List<OrderClause> orderings = new List<OrderClause>
            {
                new OrderClause(orderType, Visit(orderSelector.Body)!)
            };

            if (oldThenBys != null)
            {
                foreach(var thenBy in oldThenBys)
                {
                    LambdaExpression lambda = (LambdaExpression)thenBy.Expression;
                    map[lambda.Parameters[0]] = projection.Projector;
                    orderings.Add(new OrderClause(thenBy.OrderType, Visit(lambda.Body)!));
                }
            }

            string alias = GetNextAlias();

            var (projector, columns) = ColumnProjector.ProjectColumns(projection.Projector, alias, projection.Source.Alias);
            return new ProjectionExpression(new SelectExpression(
                                                resultType, alias, 
                                                columns, 
                                                projection.Source, 
                                                null, 
                                                orderings.AsReadOnly()),
                                            projector);
        }
        Expression BindThenBy(Expression source, LambdaExpression orderSelector, OrderType orderType)
        {
            thenBys ??=  new List<OrderClause>();
            thenBys.Add(new OrderClause(orderType, orderSelector));
            return Visit(source)!;
        }
        Expression BindGroupBy(Expression source, LambdaExpression keySelector, LambdaExpression? elementSelector, LambdaExpression? resultSelector)
        {
            ProjectionExpression projection = VisitSequence(source);
            map[keySelector.Parameters[0]] = projection.Projector;
            
            Expression keyExpression = Visit(keySelector.Body)!;
            Expression elementExpression = projection.Projector;

            if (elementSelector != null)
            {
                map[elementSelector.Parameters[0]] = projection.Projector;
                elementExpression = Visit(elementSelector.Body)!;
            }
            
            var (keyProjector, keyColumns) = ColumnProjector.ProjectColumns(keyExpression, projection.Source.Alias, projection.Source.Alias);
            var groupExpressions = keyColumns.Select(c => c.Expression).ToArray();

            ProjectionExpression subqueryBasis = VisitSequence(source);

            map[keySelector.Parameters[0]] = subqueryBasis.Projector;
            Expression subqueryKey = Visit(keySelector.Body)!;
            
            var (_, subqueryKeyColumns) = ColumnProjector.ProjectColumns(subqueryKey, subqueryBasis.Source.Alias, subqueryBasis.Source.Alias);
            IEnumerable<Expression> subqueryGroupExpressions = subqueryKeyColumns.Select(c => c.Expression);

            Expression? subqueryCorrelation = BuildPredicateWithNullsEqual(subqueryGroupExpressions, groupExpressions);
            
            Expression subqueryElementExpression = subqueryBasis.Projector;
            if (elementSelector != null)
            {
                map[elementSelector.Parameters[0]] = subqueryBasis.Projector;
                subqueryElementExpression = Visit(elementSelector.Body)!;
            }

            string elementAlias = GetNextAlias();
            var (elementProjector, elementColumns) = ColumnProjector.ProjectColumns(subqueryElementExpression, elementAlias, subqueryBasis.Source.Alias);

            // ReSharper disable once PossibleNullReferenceException - compiler knows better
            ProjectionExpression elementSubquery = new ProjectionExpression(
                new SelectExpression(TypeSystemHelper.GetSequenceType(subqueryElementExpression.Type), elementAlias, elementColumns, subqueryBasis.Source, subqueryCorrelation), 
                elementProjector);

            string alias = GetNextAlias();
            GroupByInfo info = new GroupByInfo(alias, elementExpression);
            groupByMap.Add(elementSubquery, info);

            Expression resultExpression;
            if (resultSelector != null)
            {
                Expression? saveGroupElement = currentGroupElement;
                currentGroupElement = elementSubquery;
                
                map[resultSelector.Parameters[0]] = keyProjector;
                map[resultSelector.Parameters[1]] = elementSubquery;
                
                resultExpression = Visit(resultSelector.Body)!;
                currentGroupElement = saveGroupElement;
            }
            else
                // ReSharper disable once PossibleNullReferenceException - compiler knows better
                resultExpression = Expression.New(
                    typeof(Grouping<,>).MakeGenericType(keyExpression.Type, subqueryElementExpression.Type)
                                       .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0], keyExpression, elementSubquery);

            var (projector, columns) = ColumnProjector.ProjectColumns(resultExpression, alias, projection.Source.Alias);

            Expression projectedElementSubquery = ((NewExpression)projector).Arguments[1];
            groupByMap.Add(projectedElementSubquery, info);

            // ReSharper disable once PossibleNullReferenceException - compiler knows better
            return new ProjectionExpression(
                new SelectExpression(TypeSystemHelper.GetSequenceType(resultExpression.Type), alias, columns, projection.Source, null, null, groupExpressions),
                projector);
        }
        Expression BindAggregate(Expression source, Type declaringType, AggregateType aggregateType, Type returnType, Type[] genericArguments, LambdaExpression? argument, bool isRoot)
        {
            bool hasPredicateArgument = HasPredicateArgument(aggregateType);
            bool argumentHasPredicate = false;
            
            if (argument != null && hasPredicateArgument)
            {
                declaringType = declaringType == typeof(Enumerable)
                                    ? typeof(Enumerable)
                                    : typeof(Queryable);
                source = Expression.Call(declaringType, nameof(Queryable.Where), genericArguments, source, argument);
                argument = null;
                argumentHasPredicate = true;
            }

            ProjectionExpression projection = VisitSequence(source);
            Expression? argumentExpression = null;
            if (argument != null)
            {
                map[argument.Parameters[0]] = projection.Projector;
                argumentExpression = Visit(argument.Body)!;
            }
            else if (!hasPredicateArgument)
                argumentExpression = projection.Projector;

            string alias = GetNextAlias();
            ColumnProjector.ProjectColumns(projection.Projector, alias, projection.Source.Alias);
            
            Expression aggregateExpression = new AggregateExpression(returnType, aggregateType, argumentExpression);
            Type selectType = typeof(IEnumerable<>).MakeGenericType(returnType);
            SelectExpression selectExpression = new SelectExpression(selectType, alias,
                                                                     new[] {new ColumnDeclaration(string.Empty, aggregateExpression)},
                                                                     projection.Source, null);

            if (isRoot)
                return new ProjectionExpression(selectExpression, new ColumnExpression(returnType, alias, string.Empty), true);

            var subQuery = new SubQueryExpression(returnType, selectExpression);

            if (!argumentHasPredicate && groupByMap.TryGetValue(projection, out var groupByInfo))
            {
                if (argument != null)
                {
                    map[argument.Parameters[0]] = groupByInfo.Element;
                    argumentExpression = Visit(argument.Body);
                }
                else
                    argumentExpression = groupByInfo.Element;
                
                aggregateExpression = new AggregateExpression(returnType, aggregateType, argumentExpression);

                if (projection == currentGroupElement)
                    return aggregateExpression;


                return new AggregateSubQueryExpression(groupByInfo.Alias, aggregateExpression, subQuery);
            }

            return subQuery;
        }
        ProjectionExpression VisitSequence(Expression source) => ConvertToSequence(Visit(source)!);
        
        ProjectionExpression GetTableProjection(object value)
        {
            IQueryable table = (IQueryable)value;
            string tableAlias = GetNextAlias();
            string selectAlias = GetNextAlias();

            List<MemberBinding> bindings = new List<MemberBinding>();
            List<ColumnDeclaration> columns = new List<ColumnDeclaration>();

            foreach (PropertyInfo propertyInfo in GetMappedMembers(table.ElementType))
            {
                string columnName = GetColumnName(propertyInfo);
                Type columnType = GetColumnType(propertyInfo);

                bindings.Add(Expression.Bind(propertyInfo, new ColumnExpression(columnType, selectAlias, columnName)));
                columns.Add(new ColumnDeclaration(columnName, new ColumnExpression(columnType, tableAlias, columnName)));
            }

            Expression projector = Expression.MemberInit(Expression.New(table.ElementType), bindings);
            Type resultType = typeof(IEnumerable<>).MakeGenericType(table.ElementType);

            return new ProjectionExpression(
                new SelectExpression(resultType, selectAlias, columns,
                                     new TableExpression(resultType, tableAlias, GetTableName(table)),
                                     null),
                projector);
        }
        string GetNextAlias() => $"t{aliasCount++}";

        static string GetExpressionAlias(Expression source) => source switch
        {
            SelectExpression selectExpression => selectExpression.Alias,
            TableExpression tableExpression => tableExpression.Alias,
            _ => throw source.NodeType.IsInvalid()
        };
        static bool IsTable(Expression expression) => IsTable((expression as ConstantExpression)?.Value);
        static bool IsTable(object? value) => value is IQueryable query && query.Expression.NodeType == ExpressionType.Constant;
        static string GetTableName(object table) => TypeMapper.GetTableName(((IQueryable)table).ElementType);
        static string GetColumnName(PropertyInfo member) => TypeMapper.GetColumnName(member);
        static Type GetColumnType(PropertyInfo member) => member.PropertyType;
        static IEnumerable<PropertyInfo> GetMappedMembers(Type rowType) => TypeMapper.GetMappedMembers(rowType);
        static bool MembersMatch(MemberInfo a, MemberInfo b)
        {
            if (a == b) return true;
            if (a is MethodInfo && b is PropertyInfo propertyB) return a == propertyB.GetGetMethod();
            if (a is PropertyInfo propertyA && b is MethodInfo) return propertyA.GetGetMethod() == b;

            return false;
        }
        static Expression MakeMemberAccess(Expression source, MemberInfo mi)
        {
            PropertyInfo pi = (PropertyInfo)mi;
            return Expression.Property(source, pi);
        }
        static ProjectionExpression ConvertToSequence(Expression expression) => expression.NodeType == (ExpressionType)DbExpressionType.Projection
                                                                                    ? (ProjectionExpression)expression
                                                                                    : expression.NodeType == ExpressionType.New &&
                                                                                      expression is NewExpression newExpression &&
                                                                                      expression.Type.IsGenericType &&
                                                                                      expression.Type.GetGenericTypeDefinition() ==
                                                                                      typeof(Grouping<,>)
                                                                                        ? (ProjectionExpression)newExpression.Arguments[1]
                                                                                        : throw expression.IsNotASequence();
        static Expression? BuildPredicateWithNullsEqual(IEnumerable<Expression> source1, IEnumerable<Expression> source2)
        {
            // ReSharper disable once GenericEnumeratorNotDisposed - r# mistake
            using var enumerator1 = source1.GetEnumerator();
            // ReSharper disable once GenericEnumeratorNotDisposed - r# mistake
            using var enumerator2 = source2.GetEnumerator();
            Expression? result = null;
            while (enumerator1.MoveNext() && enumerator2.MoveNext())
            {
                // ReSharper disable AssignNullToNotNullAttribute - compiler knows better
                Expression compare =
                    Expression.Or(
                        Expression.And(new IsNullExpression(enumerator1.Current), new IsNullExpression(enumerator2.Current)),
                        Expression.Equal(enumerator1.Current!, enumerator2.Current!)
                    );
                // ReSharper restore AssignNullToNotNullAttribute
                result = result == null ? compare : Expression.And(result, compare);
            }


            return result;
        }
        static AggregateType GetAggregateType(string methodName) => Enum.TryParse(methodName, out AggregateType type)
                                                                        ? type
                                                                        : throw methodName.IsUnsupportedAggregateType();
        static bool IsAsyncAggregateMethod(string methodName)
        {
            if (!methodName.EndsWith("Async", StringComparison.InvariantCulture)) return false;
            var name = methodName.Substring(0, methodName.Length - 5);
            return Enum.GetNames(typeof(AggregateType)).Contains(name);
        }
        static LambdaExpression? GetSelectorFromAsyncAggregate(ReadOnlyCollection<Expression> arguments) =>
            arguments.Count == 3 ? (LambdaExpression?)arguments[1].StripQuotes() : null;
        static bool HasPredicateArgument(AggregateType aggregateType) => aggregateType == AggregateType.Count;
        
        internal static ProjectionExpression Bind(Expression expression) => (ProjectionExpression)new QueryBinder(root: expression).Visit(expression)!;
    }
}
