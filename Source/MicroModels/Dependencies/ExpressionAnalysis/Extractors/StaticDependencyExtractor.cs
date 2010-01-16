using System.Linq.Expressions;
using MicroModels.Dependencies.Definitions;

namespace MicroModels.Dependencies.ExpressionAnalysis.Extractors
{
    internal sealed class StaticDependencyExtractor : DependencyExtractor
    {
        /// <summary>
        /// When overridden in a derived class, extracts the appropriate dependency from the root of the expression.
        /// </summary>
        /// <param name="rootExpression">The root expression.</param>
        /// <param name="propertyPath">The property path.</param>
        /// <returns></returns>
        protected override IDependencyDefinition ExtractFromRoot(Expression rootExpression, string propertyPath)
        {
            IDependencyDefinition result = null;
            if (rootExpression is MemberExpression)
            {
                // We are left with a member expression that does not have a source. It must 
                // use a static item
                var nextMember = (MemberExpression) rootExpression;
                if (nextMember.Expression == null)
                {
                    result = new StaticDependencyDefinition(propertyPath, nextMember.Member);
                }
            }
            return result;
        }
    }
}