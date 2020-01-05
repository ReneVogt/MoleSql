/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;
using MoleSql.Expressions;
using MoleSql.Mapper;
using MoleSql.Translators;

namespace MoleSql.Helpers 
{
    /// <summary>
    /// This base class is used to bind columns. It is used as a constant expression in the projection
    /// lambda expression to read columns by their ordinal (the index parameter of the <see cref="GetValue"/> method)
    /// from the <see cref="SqlDataReader"/>.<br/>
    /// It also declares a <see cref="ExecuteSubQuery{TSubQuery}"/> method that exeuctes sub queries (<see cref="ProjectionExpression"/>) in selectors.
    /// The lambda expressions will be created by the <see cref="ProjectionBuilder"/> and the reading will be done in the <see cref="ProjectionReader{T}"/>.
    /// </summary>
    abstract class ProjectionRow
    {
        internal abstract object GetValue(int index);
        internal abstract IEnumerable<TSubQuery> ExecuteSubQuery<TSubQuery>(LambdaExpression subQueryExpression);
    }
}
