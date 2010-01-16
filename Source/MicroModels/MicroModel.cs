using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MicroModels.Description;
using MicroModels.Extensions;

namespace MicroModels
{
    [TypeValidationExtension]
    public class MicroModel : MicroModelBase
    {
        protected IPropertyDefinition Property(string propertyName, Type propertyType, Expression<Func<object>> getPropertyCallback, Func<object, object> setPropertyCallback)
        {
            return MicroModelExtensions.Property(this, propertyName, propertyType, getPropertyCallback, setPropertyCallback);
        }

        protected IPropertyDefinition Property<TProperty>(string propertyName, Expression<Func<TProperty>> getPropertyCallback, Func<TProperty, TProperty> setPropertyCallback)
        {
            return MicroModelExtensions.Property(this, propertyName, getPropertyCallback, setPropertyCallback);
        }

        protected IPropertyDefinition Property(string propertyName, Type propertyType, Expression<Func<object>> getPropertyCallback)
        {
            return MicroModelExtensions.Property(this, propertyName, propertyType, getPropertyCallback);
        }

        protected IPropertyDefinition Property<TProperty>(string propertyName, Expression<Func<TProperty>> getPropertyCallback)
        {
            return MicroModelExtensions.Property(this, propertyName, getPropertyCallback);
        }

        protected IPropertyDefinition Property<TProperty>(Expression<Func<TProperty>> propertyGetter)
        {
            return MicroModelExtensions.Property(this, propertyGetter);
        }

        protected IPropertyDefinition Command(string commandPropertyName, Action executedCallback)
        {
            return MicroModelExtensions.Command(this, commandPropertyName, executedCallback);
        }

        protected IPropertyDefinition Command<TCommandParameter>(string commandPropertyName, Action<TCommandParameter> executedCallback)
        {
            return MicroModelExtensions.Command(this, commandPropertyName, executedCallback);
        }

        protected IPropertyDefinition Command(string commandPropertyName, Action executedCallback, Func<bool> canExecuteCallback)
        {
            return MicroModelExtensions.Command(this, commandPropertyName, executedCallback, canExecuteCallback);
        }

        protected IPropertyDefinition Command<TCommandParameter>(string commandPropertyName, Action<TCommandParameter> executedCallback, Func<TCommandParameter, bool> canExecuteCallback)
        {
            return MicroModelExtensions.Command(this, commandPropertyName, executedCallback, canExecuteCallback);
        }

        protected IEnumerable<IPropertyDefinition> AllProperties(object source)
        {
            return MicroModelExtensions.AllProperties(this, source);
        }

        protected ICollectionDefinition<TElement> Collection<TElement>(string propertyName, Expression<Func<IEnumerable<TElement>>> items)
        {
            return MicroModelExtensions.Collection(this, propertyName, items);
        } 
    }
}