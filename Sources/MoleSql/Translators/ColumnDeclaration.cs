using System.Linq.Expressions;

namespace MoleSql.Translators
{
    sealed class ColumnDeclaration
    {
        internal string Name { get; }
        internal Expression Expression { get; }

        public ColumnDeclaration(string name, Expression expression)
        {
            Name = name;
            Expression = expression;
        }
    }
}
