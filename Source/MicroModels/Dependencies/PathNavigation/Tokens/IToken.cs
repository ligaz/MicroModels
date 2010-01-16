using System;

namespace MicroModels.Dependencies.PathNavigation.Tokens
{
    /// <summary>
    /// This interface is implemented by objects which can monitor the property of a given object.
    /// </summary>
    /// <remarks>
    /// Property monitors are designed to be chained together to observe changes to objects and their properties.
    /// </remarks>
    public interface IToken : IDisposable
    {
        /// <summary>
        /// Gets the next monitor in the chain.
        /// </summary>
        /// <value>The next monitor.</value>
        IToken NextToken { get; }

        /// <summary>
        /// Acquires the target.
        /// </summary>
        /// <param name="target">The target.</param>
        void AcquireTarget(object target);
    }
}