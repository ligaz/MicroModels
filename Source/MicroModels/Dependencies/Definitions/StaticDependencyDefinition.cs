using System;
using System.Reflection;
using MicroModels.Dependencies.PathNavigation;

namespace MicroModels.Dependencies.Definitions
{
    /// <summary>
    /// Defines a dependency on a static property or member.
    /// </summary>
    public sealed class StaticDependencyDefinition : IDependencyDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticDependencyDefinition"/> class.
        /// </summary>
        /// <param name="propertyPath">The property path.</param>
        /// <param name="member">The member.</param>
        public StaticDependencyDefinition(string propertyPath, MemberInfo member)
        {
            Member = member;
            PropertyPath = propertyPath;
        }

        /// <summary>
        /// Gets or sets the member.
        /// </summary>
        /// <value>The member.</value>
        public MemberInfo Member { get; set; }

        /// <summary>
        /// Gets or sets the property path.
        /// </summary>
        /// <value>The property path.</value>
        public string PropertyPath { get; set; }

        /// <summary>
        /// Constructs a dependency for a single element.
        /// </summary>
        /// <param name="pathNavigator">The path navigator.</param>
        /// <returns></returns>
        public IDependency Attach(IPathNavigator pathNavigator)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}: '{1}' on '{2}'", GetType().Name, PropertyPath, Member.DeclaringType.Name);
        }
    }
}