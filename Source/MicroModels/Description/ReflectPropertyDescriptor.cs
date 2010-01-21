using System;
using System.Globalization;
using System.Reflection;

namespace MicroModels.Description
{
    internal class ReflectPropertyDescriptor : PropertyDescriptor
    {
        private readonly MethodInfo getMethod;
        private readonly MethodInfo setMethod;
        private readonly Type propertyType;
        private readonly Type componentType;

        public ReflectPropertyDescriptor(Type componentType, string name, Type propertyType, MethodInfo getMethod, MethodInfo setMethod, Attribute[] attrs)
            : base(name, attrs)
        {
            this.componentType = componentType;
            this.propertyType = propertyType;

            this.getMethod = getMethod;
            this.setMethod = setMethod;
        }

        public override Type PropertyType
        {
            get
            {
                return this.propertyType;
            }
        }

        public override Type ComponentType
        {
            get
            {
                return this.componentType;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return this.setMethod == null;
            }
        }

        public override object GetValue(object component)
        {
            if (component == null)
                return null;

            try
            {
                return this.getMethod.Invoke(component, null);
            }
            catch (Exception getValueError)
            {
                if (getValueError is TargetInvocationException)
                {
                    getValueError = getValueError.InnerException;
                }
                string message = getValueError.Message;
                if (message == null)
                {
                    message = getValueError.GetType().Name;
                }

                throw new TargetInvocationException(
                    string.Format(CultureInfo.InvariantCulture, "Could not get the value of property '{0}' value!\nError:\n{1}", this.Name, message),
                    getValueError);
            }
        }

        public override void SetValue(object component, object value)
        {
            if (this.setMethod != null)
            {
                this.setMethod.Invoke(component, new[] { value });
            }
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}