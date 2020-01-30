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
using MoleSql.Expressions;

namespace MoleSql.Translators
{
    class ExpressionReplacer : DbExpressionVisitor
    {
        readonly Func<Expression, Expression> replaceWith;

        ExpressionReplacer(Func<Expression, Expression> replaceWith)
        {
            this.replaceWith = replaceWith;
        }

        public override Expression Visit(Expression node) => replaceWith(node) ?? base.Visit(node);

        internal static Expression Replace(Expression expression, Func<Expression, Expression> replaceWith) => new ExpressionReplacer(replaceWith).Visit(expression);
    }
}
