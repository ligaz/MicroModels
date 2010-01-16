using System;
using System.ComponentModel;

namespace MicroModels.Description
{
    public class ModelTypeDescriptionProvider : TypeDescriptionProvider
    {
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            var model = (ISelfDescribing)instance;
            model.SealTypeDescription();
            return new ModelTypeDescriptor(model);
        }
    }
}