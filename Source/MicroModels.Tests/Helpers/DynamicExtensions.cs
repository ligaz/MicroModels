using System;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MicroModels.Tests.Helpers
{
    public static class DynamicExtensions
    {
        public static PropertyDescriptor GetPropertyDescriptor(this object source, string propertyName)
        {
            return GetPropertyDescriptor(source, propertyName, true);
        }

        public static PropertyDescriptor GetPropertyDescriptor(this object source, string propertyName, bool throwOnFailure)
        {
            var allProperties = TypeDescriptor.GetProperties(source).OfType<PropertyDescriptor>().ToList();
            var property = allProperties.FirstOrDefault(x => x.Name == propertyName);
            if (property == null)
            {
                if (throwOnFailure)
                {
                    var message = new StringBuilder();
                    message.AppendFormat("The property {0} does not exist.", propertyName).AppendLine();
                    message.AppendLine("Defined properties are:");
                    message.AppendLine(string.Join(Environment.NewLine,
                                                   allProperties.Select(x => " - " + x.Name).ToArray()));
                    throw new Exception(message.ToString());
                }
            }
            return property;
        }

        public static TProperty ReadProperty<TProperty>(this object source, string propertyName)
        {
            var property = source.GetPropertyDescriptor(propertyName);
            return (TProperty) property.GetValue(source);
        }

        public static void WriteProperty(this object source, string propertyName, object value)
        {
            var property = source.GetPropertyDescriptor(propertyName);
            property.SetValue(source, value);
        }
    }
}