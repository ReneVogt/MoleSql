/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace MoleSql.Expressions
{
    sealed class SubQueryExpression : Expression
    {
        internal SelectExpression SelectExpression { get; }

        public override Type Type { get; }
        public override ExpressionType NodeType { get; }

        internal SubQueryExpression(Type type, SelectExpression selectExpression)
        {
            SelectExpression = selectExpression;
            Type = type;
            NodeType = (ExpressionType)DbExpressionType.SubQuery;
        }

        [ExcludeFromCodeCoverage]
        public override string ToString() => $"SubQuery: ({SelectExpression})";
    }
}
