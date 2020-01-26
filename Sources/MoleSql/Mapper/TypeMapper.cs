using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MoleSql.Mapper
{
    static class TypeMapper
    {
        static readonly Dictionary<Type, string> tableNames = new Dictionary<Type, string>();
        static readonly Dictionary<Type, List<PropertyInfo>> mappedProperties = new Dictionary<Type, List<PropertyInfo>>();
        static readonly Dictionary<PropertyInfo, string> columnNames = new Dictionary<PropertyInfo, string>();
        
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal static string GetTableName(Type type)
        {
            if (tableNames.TryGetValue(type, out var name)) return name;

            var attribute = type.GetCustomAttribute<TableAttribute>();
            string table = $"[{attribute?.Name ?? type.Name}]"; 
            string schema = attribute?.Schema;
            name = schema == null ? table : $"[{schema}].{table}";
            tableNames[type] = name;
            return name;
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal static IEnumerable<PropertyInfo> GetMappedMembers(Type type)
        {
            if (mappedProperties.TryGetValue(type, out var props))
                return props;

            var properties =
                from property in type.GetProperties()
                where property.CanWrite
                let attribute = property.GetCustomAttribute<ColumnAttribute>()
                where attribute?.Ignore != true
                select property;

            var propList = properties.ToList();
            mappedProperties[type] = propList;
            return propList;
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal static string GetColumnName(PropertyInfo propertyInfo)
        {
            if (columnNames.TryGetValue(propertyInfo, out var name))
                return name;

            name = propertyInfo.GetCustomAttribute<ColumnAttribute>()?.Name ?? propertyInfo.Name;
            columnNames[propertyInfo] = name;
            return name;
        }
    }
}
