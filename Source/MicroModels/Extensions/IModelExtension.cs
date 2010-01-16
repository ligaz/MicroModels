using MicroModels.Description;

namespace MicroModels.Extensions
{
    /// <summary>
    /// Implemented by objects and attributes that extend the capabilities of an object.
    /// </summary>
    public interface IModelExtension
    {
        /// <summary>
        /// Gets the priority level of the property. This value indicates the relative order in which the 
        /// extension should run. Lower values run first, with higher values running later.
        /// </summary>
        /// <value>The priority.</value>
        int Priority { get; }

        /// <summary>
        /// Applies this extension to the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        void Apply(IMicroModel model);
    }
}


