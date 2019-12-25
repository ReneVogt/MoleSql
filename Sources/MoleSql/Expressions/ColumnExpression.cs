using System;
using System.Linq.Expressions;

namespace MoleSql.Expressions
{
    sealed class ColumnExpression : Expression
    {
        internal string Alias { get; }
        internal string Name { get; }
        internal int Ordinal { get; }

        public override Type Type { get; }
        public override ExpressionType NodeType { get; }

        internal ColumnExpression(Type type, string alias, string name, int ordinal)
        {
            Alias = alias;
            Name = name;
            Ordinal = ordinal;
            Type = type;
            NodeType = (ExpressionType)DbExpressionType.Column;
        }
    }
}
