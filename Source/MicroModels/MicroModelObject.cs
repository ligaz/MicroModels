using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MicroModels.Description;

#if SILVERLIGHT
using PropertyDescriptor = MicroModels.Description.PropertyDescriptor;
#else
using PropertyDescriptor = System.ComponentModel.PropertyDescriptor;
#endif

namespace MicroModels
{
    public abstract class MicroModelObject : INotifyPropertyChanged
    {
        private readonly IEnumerable<PropertyDescriptor> propertyDescriptors;

        public event PropertyChangedEventHandler PropertyChanged;

        protected MicroModelObject(MicroModelBase microModel)
        {
            this.propertyDescriptors = ((ISelfDescribing) microModel).GetProperties();

            microModel.PropertyChanged += (s, e) => this.OnPropertyChanged(e);
        }

        protected virtual T GetValue<T>(string propertyName)
        {
            var propertyDescriptor = this.GetPropertyDescriptor(propertyName);
            return (T) propertyDescriptor.GetValue(this);
        }

        protected virtual void SetValue<T>(string propertyName, T value)
        {
            var propertyDescriptor = this.GetPropertyDescriptor(propertyName);
            propertyDescriptor.SetValue(this, value);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            var hanlder = this.PropertyChanged;
            if (hanlder != null)
            {
                hanlder(this, args);
            }
        }

        private PropertyDescriptor GetPropertyDescriptor(string propertyName)
        {
            var propertyDescriptor = this.propertyDescriptors.Where(p => p.Name == propertyName).FirstOrDefault();
            if (propertyDescriptor == null)
            {
                throw new ArgumentException(
                    string.Format("PropertyDescriptor with name {0} was not found.", propertyName), "propertyName");
            }

            return propertyDescriptor;
        }
    }
}