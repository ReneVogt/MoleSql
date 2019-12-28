/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System.Linq.Expressions;

namespace MoleSql.Expressions
{
    /// <summary>
    /// Extends the original <see cref="ExpressionType"/> with new sql expression types to create
    /// SQL/CLR-hybrid expression trees.
    /// </summary>
    enum DbExpressionType
    {
        Table = 10000,
        Column = 10001,
        Select = 10002,
        Projection = 10003
    }
}
