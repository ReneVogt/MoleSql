using System.Linq.Expressions;
using MoleSql.Expressions;

namespace MoleSql.Translators
{
    static class SqlQueryTranslator
    {
        internal static TranslationResult Translate(Expression expression)
        {
            expression = expression.EvaluateLocally();
            ProjectionExpression projection = (ProjectionExpression)new QueryBinder().Bind(expression);
            (string commandText, var parameters) = new SqlQueryFormatter().Format(projection.Source);
            LambdaExpression projector = new ProjectionBuilder().Build(projection.Projector);
            
            return new TranslationResult(commandText, projector, parameters);
        }
    }
}
