/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */

using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using MoleSql.Expressions;
using MoleSql.Properties;

namespace MoleSql.Exceptions
{
    static class ThrowExtensions
    {
        internal static NotSupportedException DoesNotSupportDifferentQueryProvider(this string methodName) => new NotSupportedException(
            string.Format(CultureInfo.InvariantCulture, Resources.Exception_DoesNotSupportDifferentQueryProvider, methodName,
                          typeof(QueryProvider).FullName));
        internal static ObjectDisposedException CanOnlyBeIteratedOnce(this string typeName) =>
            new ObjectDisposedException(string.Format(CultureInfo.InvariantCulture, Resources.Exception_CanOnlyBeIteratedOnce, typeName));
        internal static InvalidOperationException CannotBeAppliedToNullValues(this BinaryExpression binaryExpression) =>
            new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Resources.Exception_BinaryExpressionCannotBeAppliedToNullValues, binaryExpression.NodeType));
        internal static NotSupportedException IsNotSupported(this BinaryExpression binaryExpression) =>
            new NotSupportedException(string.Format(CultureInfo.InvariantCulture, Resources.Exception_BinaryExpressionIsNotSupported, binaryExpression.NodeType));
        internal static NotSupportedException IsNotSupported(this UnaryExpression unaryExpression) =>
            new NotSupportedException(string.Format(CultureInfo.InvariantCulture, Resources.Exception_UnaryExpressionIsNotSupported, unaryExpression.NodeType));
        internal static NotSupportedException IsNotSupported(this JoinType joinType) =>
            new NotSupportedException(string.Format(CultureInfo.InvariantCulture, Resources.Exception_JoinTypeIsNotSupported, joinType));
        internal static NotSupportedException NullValuesOnLeftSideNotSupported() =>
            new NotSupportedException(Resources.Exception_NullValuesOnLeftSideNotSupported);
        internal static InvalidOperationException ReferenceToUndefinedColumn() =>
            new InvalidOperationException(Resources.Exception_ReferenceToUndefinedColumn);
        internal static ObjectDisposedException ObjectDisposed(this string typename) =>
            new ObjectDisposedException(string.Format(CultureInfo.InvariantCulture, Resources.Exception_ObjectDisposed, typename));
        internal static NotSupportedException IsNotSupported(this MethodInfo method) =>
            new NotSupportedException(string.Format(CultureInfo.InvariantCulture, Resources.Exception_MethodNotSupported,
                                                    $"{method.DeclaringType}.{method.Name}"));
        internal static NotSupportedException CanOnlyAppearOnTopOfExpressionTree(this MethodInfo method) =>
            new NotSupportedException(string.Format(CultureInfo.InvariantCulture, Resources.Exception_MethodCanOnlyAppearOnTopOfExpressionTree,
                                                    method.Name));
        internal static InvalidOperationException IsInvalid(this ExpressionType nodeType) =>
            new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Resources.Exception_NodeTypeIsInvalid, nodeType));
        internal static InvalidOperationException IsInvalidSelectSourceType(this ExpressionType nodeType) =>
            new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Resources.Exception_NodeTypeIsInvalidSelectSourceType, nodeType));
        internal static InvalidOperationException IsNotASequence(this Expression expression) =>
            new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Resources.Exception_ExpressionIsNotASequence, expression.Type, expression.NodeType));
        internal static NotSupportedException IsNotSupported(this AggregateType aggregateType) =>
            new NotSupportedException(string.Format(CultureInfo.InvariantCulture, Resources.Exception_AggregateTypeNotSupported, aggregateType));
        internal static NotSupportedException IsUnsupportedAggregateType(this string aggregateType) =>
            new NotSupportedException(string.Format(CultureInfo.InvariantCulture, Resources.Exception_AggregateTypeNotSupported, aggregateType));
    }
}
