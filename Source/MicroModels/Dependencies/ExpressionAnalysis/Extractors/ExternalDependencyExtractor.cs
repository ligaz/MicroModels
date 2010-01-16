using System.ComponentModel;
using System.Linq.Expressions;
using MicroModels.Dependencies.Definitions;

namespace MicroModels.Dependencies.ExpressionAnalysis.Extractors
{
    /// <summary>
    /// A dependency extractor that finds dependencies on external resources. 
    /// </summary>
    internal sealed class ExternalDependencyExtractor : DependencyExtractor
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
            if (rootExpression is ConstantExpression)
            {
                var constantExpression = (ConstantExpression) rootExpression;
                if (propertyPath != null || (propertyPath == null && constantExpression.Value is INotifyPropertyChanged))
                {
                    result = new ExternalDependencyDefinition(propertyPath, constantExpression.Value);
                }
            }
            return result;
        }
    }
}