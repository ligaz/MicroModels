using System;
using MicroModels.Dependencies.PathNavigation.Tokens;

namespace MicroModels.Dependencies.PathNavigation.TokenFactories
{
    /// <summary>
    /// Implemented by factories that construct <see cref="IToken">ITokens</see> from a property path.
    /// </summary>
    public interface ITokenFactory
    {
        /// <summary>
        /// Creates an appropriate property monitor for the remaining property path string on the target object.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="propertyPath">The property path.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="pathNavigator">The path navigator.</param>
        /// <returns>
        /// An appropriate <see cref="IToken"/> for the property.
        /// </returns>
        IToken ParseNext(object target, string propertyPath, Action<object, string> callback, IPathNavigator pathNavigator);
    }
}