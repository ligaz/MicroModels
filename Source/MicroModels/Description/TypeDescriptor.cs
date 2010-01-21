using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MicroModels.Description
{
    internal class TypeDescriptor
    {
        public static IEnumerable<PropertyDescriptor> GetProperties(object component)
        {
            return GetProperties(component.GetType());
        }

        public static IEnumerable<PropertyDescriptor> GetProperties(Type componentType)
        {
            return ReflectGetProperties(componentType);
        }

        private static PropertyDescriptor[] ReflectGetProperties(Type componentType)
        {
            BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
            PropertyInfo[] properties = componentType.GetProperties(bindingAttr);
            PropertyDescriptor[] sourceArray = new PropertyDescriptor[properties.Length];
            int length = 0;
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo propInfo = properties[i];

                if (propInfo.GetIndexParameters().Length <= 0)
                {
                    MethodInfo getMethod = propInfo.GetGetMethod();
                    MethodInfo setMethod = propInfo.GetSetMethod();
                    string propertyName = propInfo.Name;

                    if (getMethod != null)
                    {
                        Type propertyType = propInfo.PropertyType;
                        sourceArray[length++] = new ReflectPropertyDescriptor(
                            componentType, propertyName, propertyType, getMethod, setMethod,
                            propInfo.GetCustomAttributes(true).OfType<Attribute>().ToArray());
                    }
                }
            }
            if (length != sourceArray.Length)
            {
                var destinationArray = new PropertyDescriptor[length];
                Array.Copy(sourceArray, 0, destinationArray, 0, length);
                sourceArray = destinationArray;
            }

            return sourceArray;
        }
    }
}