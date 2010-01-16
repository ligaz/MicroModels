namespace MicroModels.Helpers
{
    /// <summary>
    /// This interface is implemented by classes which read property values.
    /// </summary>
    /// <typeparam name="TCast">The type to cast the return type to.</typeparam>
    internal interface IPropertyReader<TCast>
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        TCast GetValue(object input);
    }
}