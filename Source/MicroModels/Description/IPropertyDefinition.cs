using System;
using System.ComponentModel;
using MicroModels.Dependencies;

namespace MicroModels.Description
{
    public interface IPropertyDefinition
    {
        IMicroModel Model { get; }
        string Name { get; set; }
        Type PropertyType { get; set; }
        Func<object, object> Getter { get; set; }
        Action<object, object> Setter { get; set; }
        PropertyDescriptor GetDescriptor();
        void AddDependency(IDependencyDefinition dependency);
        void Seal();
    }
}