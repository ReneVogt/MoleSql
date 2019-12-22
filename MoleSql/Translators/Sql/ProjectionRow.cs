namespace MoleSql.Translators.Sql {
    abstract class ProjectionRow
    {
        internal abstract object GetValue(int index);
    }
}
