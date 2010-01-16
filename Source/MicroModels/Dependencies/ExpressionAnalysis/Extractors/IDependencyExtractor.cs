using System.Collections.Generic;
using System.Linq.Expressions;

namespace MicroModels.Dependencies.ExpressionAnalysis.Extractors
{
    /// <summary>
    /// Implemented by objects which analyse expressions and extract dependencies.
    /// </summary>
    public interface IDependencyExtractor
    {
        /// <summary>
        /// Extracts any dependencies within the specified LINQ expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        IEnumerable<IDependencyDefinition> Extract(Expression expression);
    }
}