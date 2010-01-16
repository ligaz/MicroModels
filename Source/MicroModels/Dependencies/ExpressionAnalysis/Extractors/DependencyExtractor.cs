using System.Collections.Generic;
using System.Linq.Expressions;

namespace MicroModels.Dependencies.ExpressionAnalysis.Extractors
{
    /// <summary>
    /// Serves as a base class for dependency extractors that create dependencies against properties. 
    /// These dependencies have one thing in common: They only look for MemberAccess expressions, and 
    /// the type of dependency depends on what the root of the expression is.
    /// </summary>
    internal abstract class DependencyExtractor : IDependencyExtractor
    {
        /// <summary>
        /// Extracts any dependencies within the specified LINQ expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public IEnumerable<IDependencyDefinition> Extract(Expression expression)
        {
            var results = new List<IDependencyDefinition>();

            // Find the root member access expressions
            var analyser = new ExpressionFlattener(expression, ExpressionType.MemberAccess);
            
            // Turn each one into the appropriate dependency
            foreach (var childExpression in analyser.Expressions)
            {
                var traverse = false;
                var currentExpression = childExpression;
                string propertyPath = null;

                if (childExpression is MemberExpression)
                {
                    var childMemberExpression = (MemberExpression) childExpression;
                    propertyPath = childMemberExpression.Member.Name;
                    currentExpression = childMemberExpression.Expression;
                    traverse = true;
                    while (true)
                    {
                        if (currentExpression is MemberExpression)
                        {
                            var nextMemberExpression = (MemberExpression) currentExpression;
                            propertyPath = nextMemberExpression.Member.Name + "." + propertyPath;
                            if (nextMemberExpression.Expression != null)
                            {
                                currentExpression = nextMemberExpression.Expression;
                                continue;
                            }
                        }
                        break;
                    }
                }

                if (currentExpression != null)
                {
                    var dependency = ExtractFromRoot(currentExpression, propertyPath);
                    if (dependency != null)
                    {
                        results.Add(dependency);
                    }
                    else if (traverse)
                    {
                        results.AddRange(Extract(currentExpression));
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// When overridden in a derived class, extracts the appropriate dependency from the root of the expression.
        /// </summary>
        /// <param name="rootExpression">The root expression.</param>
        /// <param name="propertyPath">The property path.</param>
        /// <returns></returns>
        protected abstract IDependencyDefinition ExtractFromRoot(Expression rootExpression, string propertyPath);
    }
}