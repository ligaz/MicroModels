using System;
using MicroModels.Dependencies.PathNavigation.TokenFactories;
using MicroModels.Dependencies.PathNavigation.Tokens;

namespace MicroModels.Dependencies.PathNavigation
{
    /// <summary>
    /// A factory for the construction of property monitors by detecting information about the object.
    /// </summary>
    public class PathNavigator : IPathNavigator
    {
        private readonly ITokenFactory[] _traversers;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathNavigator"/> class.
        /// </summary>
        /// <param name="traversers">The traversers.</param>
        public PathNavigator(params ITokenFactory[] traversers)
        {
            _traversers = traversers;
        }

        #region IPathNavigator Members
        /// <summary>
        /// Creates an appropriate property monitor for the remaining property path string on the target object.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="propertyPath">The property path.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>
        /// An appropriate <see cref="IToken"/> for the property.
        /// </returns>
        public IToken TraverseNext(object target, string propertyPath, Action<object, string> callback)
        {
            propertyPath = propertyPath ?? string.Empty;
            IToken result = null;
            foreach (var traverser in _traversers)
            {
                result = traverser.ParseNext(target, propertyPath, callback, this);
                if (result != null)
                {
                    break;
                }
            }
            return result;
        }
        #endregion
    }
}