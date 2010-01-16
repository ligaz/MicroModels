using System.ComponentModel;
using System.Linq;

namespace MicroModels.Description
{
    internal class ModelTypeDescriptor : CustomTypeDescriptor
    {
        private readonly ISelfDescribing _model;

        public ModelTypeDescriptor(ISelfDescribing model)
        {
            _model = model;
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            var properties = base.GetProperties().Cast<PropertyDescriptor>();
            properties = properties.Union(_model.GetProperties());
            return new PropertyDescriptorCollection(properties.ToArray());
        }
    }
}