using System.Collections.Generic;
using System.Linq.Expressions;

namespace MoleSql.Translators
{
    internal struct TranslationResult
    {
        internal string CommandText { get; set; }
        internal LambdaExpression Projection { get; set; }
        internal List<(string name, object value)> Parameters { get; set; }
        internal TranslationResult(string commandText, LambdaExpression projection, List<(string name, object value)> parameters)
        {
            CommandText = commandText;
            Projection = projection;
            Parameters = parameters;
        }
        internal void Deconstruct(out string commmandText, out LambdaExpression projection, out List<(string name, object value)> parameters)
        {
            commmandText = CommandText;
            projection = Projection;
            parameters = Parameters;
        }
    }
}
