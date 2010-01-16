using System;

namespace MicroModels.Dependencies
{    
    /// <summary>
    /// An interface implemented by all dependencies. 
    /// </summary>
    public interface IDependency : IDisposable
    {
        /// <summary>
        /// Sets the callback action the dependency should invoke when the dependent object has a property that changes.
        /// </summary>
        /// <param name="action">The callback action to invoke.</param>
        void SetReevaluateElementCallback(Action<object, string> action);

        /// <summary>
        /// Sets the callback action the dependency should invoke when the dependent object changes, signalling the 
        /// whole collection should re-evaluate.
        /// </summary>
        /// <param name="action">The callback action to invoke.</param>
        void SetReevaluateCallback(Action<object> action);
    }
}