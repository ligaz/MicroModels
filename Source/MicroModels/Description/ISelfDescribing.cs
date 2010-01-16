using System.Collections.Generic;
using System.ComponentModel;

namespace MicroModels.Description
{
    public interface ISelfDescribing
    {
        void SealTypeDescription();
        IEnumerable<PropertyDescriptor> GetProperties();
    }
}