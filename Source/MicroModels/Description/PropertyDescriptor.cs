using System;

namespace MicroModels.Description
{
    public abstract class PropertyDescriptor
    {
        private readonly Attribute[] attributes;
        private readonly string name;

        protected PropertyDescriptor(string name, Attribute[] attributes)
        {
            this.name = name;
            this.attributes = attributes;
        }

        public Attribute[] Attributes
        {
            get { return this.attributes; }
        }

        public abstract Type ComponentType { get; }

        public abstract bool IsReadOnly { get; }

        public abstract Type PropertyType { get; }

        public virtual string Name
        {
            get { return this.name; }
        }

        public abstract bool CanResetValue(object component);

        public abstract object GetValue(object component);

        public abstract void ResetValue(object component);

        public abstract void SetValue(object component, object value);

        public abstract bool ShouldSerializeValue(object component);
    }
}