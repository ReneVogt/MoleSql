namespace MoleSql.Translators {
    abstract class ProjectionRow
    {
        internal abstract object GetValue(int index);
    }
}
