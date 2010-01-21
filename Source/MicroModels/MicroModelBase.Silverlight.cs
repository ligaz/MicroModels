using MicroModels.Description;
using MicroModels.Utilities;
using System;

namespace MicroModels
{
    public partial class MicroModelBase
    {
        private MicroModelObject microModelObject;

        public MicroModelObject Object
        {
            get
            {
                if (this.microModelObject == null)
                {
                    this.microModelObject = CreateMicroModelObject(this);
                }

                return this.microModelObject;
            }
        }

        private static MicroModelObject CreateMicroModelObject(ISelfDescribing selfDescribing)
        {
            selfDescribing.SealTypeDescription();

            var properties = selfDescribing.GetProperties();
            var microModelObjectType = MicroModelObjectBuilder.GetMicroModelObjectType(properties);

            var microModelObject = (MicroModelObject)Activator.CreateInstance(microModelObjectType, selfDescribing);
            
            return microModelObject;
        }
    }
}