using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace MoleSql.Translators
{
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
