using System.Collections.Generic;
using MicroModels.Extensions;

namespace MicroModels.Description
{
    public interface IMicroModel
    {
        void AddExtension(IModelExtension modelExtension);
        void RemoveExtension(IModelExtension modelExtension);
        void AddProperty(IPropertyDefinition property);
        void RemoveProperty(IPropertyDefinition property);
        IEnumerable<IPropertyDefinition> GetProperties();
        IEnumerable<IModelExtension> GetExtensions();
        void RaisePropertyChanged(string propertyName);
    }
}