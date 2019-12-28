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
using System.Linq;
using System.Linq.Expressions;
using MoleSql.Translators;

namespace MoleSql.Expressions
{
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
    }
}
