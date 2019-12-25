using System;
using System.Linq.Expressions;

namespace MoleSql.Expressions
{
    sealed class TableExpression : Expression
    {
        internal string Alias { get; }
        internal string Name { get; }

        public override Type Type { get; }
        public override ExpressionType NodeType { get; }

        internal TableExpression(Type type, string alias, string name)
        {
            Alias = alias;
            Name = name;
            Type = type;
            NodeType = (ExpressionType)DbExpressionType.Table;
        }
    }
}
