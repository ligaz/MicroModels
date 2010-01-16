using System;
using MicroModels.Dependencies.PathNavigation.Tokens;

namespace MicroModels.Dependencies.PathNavigation
{
    /// <summary>
    /// An interface implemented by classes which can traverse a property path.
    /// </summary>
    public interface IPathNavigator
    {
        /// <summary>
        /// Creates an appropriate property monitor for the remaining property path string on the target object.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="propertyPath">The property path.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>
        /// An appropriate <see cref="IToken"/> for the property.
        /// </returns>
        IToken TraverseNext(object target, string propertyPath, Action<object, string> callback);
    }
}