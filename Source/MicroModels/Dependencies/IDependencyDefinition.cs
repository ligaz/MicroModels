using MicroModels.Dependencies.PathNavigation;

namespace MicroModels.Dependencies
{
    /// <summary>
    /// This interface is implemented by items that represent a dependency.
    /// </summary>
    public interface IDependencyDefinition
    {
        /// <summary>
        /// Constructs a dependency for a single element.
        /// </summary>
        /// <param name="pathNavigator">The path navigator.</param>
        /// <returns></returns>
        IDependency Attach(IPathNavigator pathNavigator);
    }
}