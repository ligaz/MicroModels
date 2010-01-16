using System;
using System.Collections.Generic;
using MicroModels.Description;

namespace MicroModels
{
    public static class PropertyDefinitionExtensions
    {
        public static IPropertyDefinition WithPrefix(this IPropertyDefinition propertyDefinition, string prefix)
        {
            propertyDefinition.Name = prefix + propertyDefinition.Name;
            return propertyDefinition;
        }

        public static IEnumerable<IPropertyDefinition> WithPrefix(this IEnumerable<IPropertyDefinition> propertyDefinitions, string prefix)
        {
            foreach (var property in propertyDefinitions)
            {
                WithPrefix(property, prefix);
            }
            return propertyDefinitions;
        }

        public static IEnumerable<IPropertyDefinition> Excluding(this IEnumerable<IPropertyDefinition> propertyDefinitions, Func<IPropertyDefinition, bool> excludeItemCallback)
        {
            var items = new List<IPropertyDefinition>();
            foreach (var property in propertyDefinitions)
            {
                if (excludeItemCallback(property))
                {
                    property.Model.RemoveProperty(property);
                }
                else
                {
                    items.Add(property);
                }
            }
            return items;
        }

        public static IPropertyDefinition Named(this IPropertyDefinition propertyDefinition, string name)
        {
            propertyDefinition.Name = name;
            return propertyDefinition;
        }

        public static IPropertyDefinition ReadOnly(this IPropertyDefinition propertyDefinition)
        {
            propertyDefinition.Setter = null;
            return propertyDefinition;
        }
    }
}