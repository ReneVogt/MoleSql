using System;

namespace MoleSql.Mapper
{
    /// <summary>
    /// Marks a class or struct as a database table and can define
    /// an alternative table name or schema.
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, Inherited = false)]
    public sealed class TableAttribute : Attribute
    {
        /// <summary>
        /// The database schema the table is declared in.
        /// If this is not defined, no schema information will be used.
        /// </summary>
        public string? Schema { get; set; }
        /// <summary>
        /// The table name to use when querying objects. If this is not
        /// defined, the type's name will be used as table name.
        /// </summary>
        public string? Name { get; set; }
    }
}
