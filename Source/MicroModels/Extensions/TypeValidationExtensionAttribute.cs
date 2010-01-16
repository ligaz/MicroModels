using System;
using System.Linq;
using MicroModels.Description;

namespace MicroModels.Extensions
{
    /// <summary>
    /// A model extension that validates a model to ensure it conforms to expectations around property 
    /// settings.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TypeValidationExtensionAttribute : Attribute, IModelExtension
    {
        /// <summary>
        /// Gets the priority. Since this extension checks the validity of the object, it should generally 
        /// be the last extension run.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority
        {
            get { return int.MaxValue; }
        }

        /// <summary>
        /// Applies this extension to the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        public void Apply(IMicroModel model)
        {
            var properties = model.GetProperties().ToList();
            if (properties.Count != properties.Select(x => x.Name).Distinct().Count())
            {
                Fail("One or more properties on a dynamic object have duplicate names. The properties are: {0}{1}", 
                    Environment.NewLine,
                    string.Join(Environment.NewLine, properties.Select(x => " - " + x.Name).ToArray())
                    );
            }
        }

        private static void Fail(string message, params object[] args)
        {
            throw new InvalidOperationException(string.Format(message, args));
        }
    }
}
