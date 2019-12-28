/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using MoleSql.Mapper;

namespace MoleSql.Translators 
{
    /// <summary>
    /// This base class is used to bind columns. It is used as a constant expression in the projection
    /// lambda expression to read columns by their ordinal (the index parameter of the <see cref="GetValue"/> method)
    /// from the <see cref="SqlDataReader"/>.<br/><br/>
    ///
    /// The lambda expression will be created by the <see cref="ProjectionBuilder"/> and the reading will be done in the <see cref="ProjectionReader{T}"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    abstract class ProjectionRow
    {
        internal abstract object GetValue(int index);
    }
}
