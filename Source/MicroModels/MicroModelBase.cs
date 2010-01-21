using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MicroModels.Description;
using MicroModels.Extensions;

#if SILVERLIGHT
using PropertyDescriptor = MicroModels.Description.PropertyDescriptor;
#else
using PropertyDescriptor = System.ComponentModel.PropertyDescriptor;
#endif

namespace MicroModels
{
    public abstract partial class MicroModelBase : IMicroModel, ISelfDescribing, INotifyPropertyChanged
    {
        private readonly List<IPropertyDefinition> _properties = new List<IPropertyDefinition>();
        private readonly List<IModelExtension> _extensions = new List<IModelExtension>();
        private bool _isSealed;
        private bool _isSealedForExtensions;

        void ISelfDescribing.SealTypeDescription()
        {
            if (_isSealed) return;

            DiscoverExtensions();

            _isSealedForExtensions = true;
            foreach (var extension in _extensions.OrderBy(x => x.Priority))
            {
                extension.Apply(this);
            }

            foreach (var property in _properties)
            {
                property.Seal();
            }

            _isSealed = true;
        }

        protected virtual void DiscoverExtensions()
        {
            var allAttributes = GetType().GetCustomAttributes(true);
            var extensions = allAttributes.OfType<IModelExtension>();
            foreach (var extension in extensions)
            {
                _extensions.Add(extension);
            }
        }

        public void RemoveProperty(IPropertyDefinition property)
        {
            EnsureNotSealed();
            if (_properties.Contains(property))
            {
                _properties.Remove(property);
            }
        }

        IEnumerable<PropertyDescriptor> ISelfDescribing.GetProperties()
        {
            return _properties.Select(x => x.GetDescriptor()).ToList().AsReadOnly();
        }

        public IEnumerable<IModelExtension> GetExtensions()
        {
            return _extensions.AsReadOnly();
        }

        public void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual IEnumerable<IPropertyDefinition> GetProperties()
        {
            return _properties.AsReadOnly();
        }

        protected virtual void AddExtension(IModelExtension extension)
        {
            EnsureNotSealed();
            EnsureNotSealedForExtensions();
            if (!_extensions.Contains(extension))
            {
                _extensions.Add(extension);
            }
        }

        protected virtual void RemoveExtension(IModelExtension extension)
        {
            EnsureNotSealed();
            EnsureNotSealedForExtensions();
            if (_extensions.Contains(extension))
            {
                _extensions.Remove(extension);
            }
        }

        protected virtual void AddProperty(IPropertyDefinition property)
        {
            EnsureNotSealed();
            _properties.Add(property);
        }

        protected void EnsureNotSealed()
        {
            if (_isSealed)
            {
                throw new NotSupportedException("The operation could not be performed as the model has been sealed.");
            }
        }

        protected void EnsureNotSealedForExtensions()
        {
            if (_isSealedForExtensions)
            {
                throw new NotSupportedException("Extensions cannot be added or removed because the model has been sealed.");
            }
        }

        void IMicroModel.AddExtension(IModelExtension modelExtension)
        {
            AddExtension(modelExtension);
        }

        void IMicroModel.RemoveExtension(IModelExtension modelExtension)
        {
            RemoveExtension(modelExtension);
        }

        void IMicroModel.AddProperty(IPropertyDefinition property)
        {
            AddProperty(property);
        }

        IEnumerable<IPropertyDefinition> IMicroModel.GetProperties()
        {
            return GetProperties();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
