using System;
using System.Collections.Generic;
using System.ComponentModel;
using MicroModels.Dependencies;
using MicroModels.Dependencies.PathNavigation;
using MicroModels.Dependencies.PathNavigation.TokenFactories;

namespace MicroModels.Description
{
    internal class DelegatePropertyDescriptor : PropertyDescriptor, IPropertyDefinition
    {
        private readonly Type _ownerType;
        private Type _propertyType;
        private string _name;
        private readonly IMicroModel _model;
        private readonly List<IDependencyDefinition> _dependencyDefinitions = new List<IDependencyDefinition>();
        private readonly List<IDependency> _dependencies = new List<IDependency>();

        public DelegatePropertyDescriptor(string name, IMicroModel model, Type propertyType) : base(name, null)
        {
            _name = name;
            _model = model;
            _ownerType = model.GetType();
            _propertyType = propertyType;
        }

        public Func<object, object> Getter { get; set; }
        public Action<object, object> Setter { get; set; }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            if (Getter == null)
            {
                throw new NotSupportedException();
            }
            return Getter(component);
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
            if (Setter == null)
            {
                throw new NotSupportedException();
            }
            Setter(component, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return _ownerType; }
        }

        public override bool IsReadOnly
        {
            get { return Setter == null; }
        }

        public IMicroModel Model
        {
            get { return _model; }
        }

        public override Type PropertyType
        {
            get { return _propertyType; }
        }

        Type IPropertyDefinition.PropertyType
        {
            get { return _propertyType; }
            set { _propertyType = value; }
        }

        public override string Name
        {
            get { return _name; }
        }

        #region IPropertyDefinition Members

        string IPropertyDefinition.Name
        {
            get { return _name; }
            set { _name = value; }
        }

        PropertyDescriptor IPropertyDefinition.GetDescriptor()
        {
            return this;
        }

        public void AddDependency(IDependencyDefinition dependency)
        {
            _dependencyDefinitions.Add(dependency);
        }

        public void Seal()
        {
            var pathNavigator = new PathNavigator(new ClrMemberTokenFactory());
            foreach (var dependencyDefinition in _dependencyDefinitions)
            {
                var dependency = dependencyDefinition.Attach(pathNavigator);
                dependency.SetReevaluateCallback(x => Model.RaisePropertyChanged(Name));
                _dependencies.Add(dependency);
            }
        }

        #endregion
    }
}