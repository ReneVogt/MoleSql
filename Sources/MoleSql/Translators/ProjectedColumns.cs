/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using MoleSql.Expressions;

namespace MoleSql.Translators
{
    [ExcludeFromCodeCoverage]
    sealed class ProjectedColumns
    {
        internal Expression Projector { get; }
        internal ReadOnlyCollection<ColumnDeclaration> Columns { get; }
        
        internal ProjectedColumns(Expression projector, ReadOnlyCollection<ColumnDeclaration> columns)
        {
            Projector = projector;
            Columns = columns;
        }
    }
}
