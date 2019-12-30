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
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MoleSql.Expressions;

namespace MoleSql.Translators
{
    sealed class QueryBinder : DbExpressionVisitor
    {
        readonly ColumnProjector columnProjector = new ColumnProjector();
        readonly Dictionary<ParameterExpression, Expression> map = new Dictionary<ParameterExpression, Expression>();
        int aliasCount;

        QueryBinder()
        {
        }

        protected override Expression VisitMethodCall(MethodCallExpression callExpression)
        {
            if (callExpression.Method.DeclaringType != typeof(Queryable) && callExpression.Method.DeclaringType != typeof(Enumerable))
                return base.VisitMethodCall(callExpression);

            return callExpression.Method.Name switch
            {
                nameof(Queryable.Where) => BindWhere(callExpression.Type, callExpression.Arguments[0],
                                                     (LambdaExpression)callExpression.Arguments[1].StripQuotes()),
                nameof(Queryable.Select) => BindSelect(callExpression.Type, callExpression.Arguments[0],
                                                       (LambdaExpression)callExpression.Arguments[1].StripQuotes()),
                _ => throw new NotSupportedException($"The method '{callExpression.Method.Name}' is not supported.")
            };
        }
        protected override Expression VisitConstant(ConstantExpression constant) => IsTable(constant.Value) ? (Expression)GetTableProjection(constant.Value) : constant;
        protected override Expression VisitParameter(ParameterExpression parameter) =>
            map.TryGetValue(parameter, out var expression) ? expression : parameter;
        protected override Expression VisitMember(MemberExpression member)
        {
            Expression source = Visit(member.Expression);

            switch (source?.NodeType)
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
                    break;
            }

            if (source == member.Expression) return member;
            return MakeMemberAccess(source, member.Member);
        }

        Expression BindWhere(Type resultType, Expression source, LambdaExpression predicate)
        {
            ProjectionExpression projection = (ProjectionExpression)Visit(source);
            Debug.Assert(projection != null);

            map[predicate.Parameters[0]] = projection.Projector;
            Expression where = Visit(predicate.Body); 
            
            string alias = GetNextAlias();
            var (projector, columns) = ProjectColumns(projection.Projector, alias, GetExpressionAlias(projection.Source));

            return new ProjectionExpression(
                new SelectExpression(resultType, alias, columns, projection.Source, where),
                projector);
        }
        Expression BindSelect(Type resultType, Expression source, LambdaExpression selector)
        {
            ProjectionExpression projection = (ProjectionExpression)Visit(source);
            Debug.Assert(projection != null);

            map[selector.Parameters[0]] = projection.Projector; 
            Expression expression = Visit(selector.Body);

            string alias = GetNextAlias();
            var (projector, columns) = ProjectColumns(expression, alias, GetExpressionAlias(projection.Source));

            return new ProjectionExpression(
                new SelectExpression(resultType, alias, columns, projection.Source, null),
                projector);
        }

        (Expression projector, IReadOnlyCollection<ColumnDeclaration> columns) ProjectColumns(Expression expression, string newAlias, string existingAlias) => columnProjector.ProjectColumns(expression, newAlias, existingAlias);
        ProjectionExpression GetTableProjection(object value)
        {
            IQueryable table = (IQueryable)value;
            string tableAlias = GetNextAlias();
            string selectAlias = GetNextAlias();

            List<MemberBinding> bindings = new List<MemberBinding>();
            List<ColumnDeclaration> columns = new List<ColumnDeclaration>();

            foreach (MemberInfo mi in GetMappedMembers(table.ElementType))
            {
                string columnName = GetColumnName(mi);
                Type columnType = GetColumnType(mi);
                int ordinal = columns.Count;

                bindings.Add(Expression.Bind(mi, new ColumnExpression(columnType, selectAlias, columnName, ordinal)));
                columns.Add(new ColumnDeclaration(columnName, new ColumnExpression(columnType, tableAlias, columnName, ordinal)));
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
            _ => throw new InvalidOperationException($"Invalid source node type '{source.NodeType}'")
        };
        static bool IsTable(object value) => value is IQueryable query && query.Expression.NodeType == ExpressionType.Constant;
        static string GetTableName(object table) => ((IQueryable)table).ElementType.Name;
        static string GetColumnName(MemberInfo member) => member.Name;
        static Type GetColumnType(MemberInfo member) => ((PropertyInfo)member).PropertyType;
        static IEnumerable<MemberInfo> GetMappedMembers(Type rowType) => rowType.GetProperties().Where(prop => prop.CanWrite);
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

        internal static ProjectionExpression Bind(Expression expression) => (ProjectionExpression)new QueryBinder().Visit(expression);
    }
}
