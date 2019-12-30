﻿/*
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace MoleSql.Expressions
{
    [ExcludeFromCodeCoverage]
    sealed class SelectExpression : Expression
    {
        internal string Alias { get; }
        internal Expression From { get; }
        internal Expression Where { get; }
        internal ReadOnlyCollection<ColumnDeclaration> Columns { get; }

        public override Type Type { get; }
        public override ExpressionType NodeType { get; }

        internal SelectExpression(Type type, string alias, IEnumerable<ColumnDeclaration> columns, Expression from, Expression where)
        {
            Alias = alias;
            From = from;
            Where = where;
            Columns = columns as ReadOnlyCollection<ColumnDeclaration> ?? columns.ToList().AsReadOnly();
            Type = type;
            NodeType = (ExpressionType)DbExpressionType.Select;
        }

        public override string ToString() => $"SELECT ({Type}) ({string.Join(", ", Columns)} FROM ({From}) WHERE ({Where}) AS '{Alias}'";
    }
}
