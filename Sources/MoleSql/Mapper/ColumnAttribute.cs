using System;

namespace MoleSql.Mapper
{
    /// <summary>
    /// Marks a property as a table column and can define
    /// an alternative column name or mark the property as ignored.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ColumnAttribute : Attribute
    {
        /// <summary>
        /// The column name to use when querying objects. If this is not
        /// defined, the property's name will be used as table name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Set to <code>true</code> if the property should not be included in SQL queries.
        /// </summary>
        public bool Ignore { get; set; }
    }
}
