/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System.Linq.Expressions;
using MoleSql.Expressions;

namespace MoleSql.Translators
{
    class ExpressionReplacer : DbExpressionVisitor
    {
        readonly Expression searchFor, replaceWith;

        ExpressionReplacer(Expression searchFor, Expression replaceWith)
        {
            this.searchFor = searchFor;
            this.replaceWith = replaceWith;
        }

        public override Expression Visit(Expression node) => node == searchFor ? replaceWith : base.Visit(node);

        internal static Expression Replace(Expression expression, Expression searchFor, Expression replaceWith) => new ExpressionReplacer(searchFor, replaceWith).Visit(expression);
    }
}
