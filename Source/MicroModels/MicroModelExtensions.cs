using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Input;
using MicroModels.Commands;
using MicroModels.Dependencies;
using MicroModels.Dependencies.ExpressionAnalysis.Extractors;
using MicroModels.Description;
using MicroModels.Utilities;
using MicroModels.Expressions;
using MicroModels.Dependencies.ExpressionAnalysis;

#if !SILVERLIGHT
using System.ComponentModel;
#endif

namespace MicroModels
{
    public static class MicroModelExtensions
    {
        private static ExpressionAnalyzer _analyser;

        static MicroModelExtensions()
        {
            _analyser = new ExpressionAnalyzer(new ExternalDependencyExtractor(), new StaticDependencyExtractor());
        }

        private static IPropertyDefinition BuildProperty(IMicroModel model, string propertyName, Type propertyType)
        {
            var property = new DelegatePropertyDescriptor(propertyName, model, propertyType);
            model.AddProperty(property);
            return property;
        }

        private static void AddDependencies(IPropertyDefinition property, LambdaExpression expression)
        {
            var dependencies = _analyser.DiscoverDependencies(expression, null);
            foreach (var dependency in dependencies)
            {
                property.AddDependency(dependency);
            }
        }

        public static IPropertyDefinition Property(this IMicroModel model, string propertyName, Type propertyType, Expression<Func<object>> getPropertyCallback, Func<object, object> setPropertyCallback)
        {
            getPropertyCallback = Evaluator.EvaluateClosures(getPropertyCallback);
            var compiledGetter = getPropertyCallback.Compile();
            var property = BuildProperty(model, propertyName, propertyType);
            property.Getter = (x) => compiledGetter();
            property.Setter = (x, y) => setPropertyCallback(y);
            AddDependencies(property, getPropertyCallback);
            return property;
        }

        public static IPropertyDefinition Property<TProperty>(this IMicroModel model, string propertyName, Expression<Func<TProperty>> getPropertyCallback, Func<TProperty, TProperty> setPropertyCallback)
        {
            getPropertyCallback = Evaluator.EvaluateClosures(getPropertyCallback);
            var compiledGetter = getPropertyCallback.Compile();
            var property = BuildProperty(model, propertyName, typeof(TProperty));
            property.Getter = (x) => compiledGetter();
            property.Setter = (x, y) => setPropertyCallback((TProperty)y);
            AddDependencies(property, getPropertyCallback);
            return property;
        }

        public static IPropertyDefinition Property(this IMicroModel model, string propertyName, Type propertyType, Expression<Func<object>> getPropertyCallback)
        {
            getPropertyCallback = Evaluator.EvaluateClosures(getPropertyCallback);
            var compiledGetter = getPropertyCallback.Compile();
            var property = BuildProperty(model, propertyName, propertyType);
            property.Getter = (x) => compiledGetter();
            AddDependencies(property, getPropertyCallback);
            return property;
        }

        public static IPropertyDefinition Property<TProperty>(this IMicroModel model, string propertyName, Expression<Func<TProperty>> getPropertyCallback)
        {
            getPropertyCallback = Evaluator.EvaluateClosures(getPropertyCallback);
            var compiledGetter = getPropertyCallback.Compile();
            var property = BuildProperty(model, propertyName, typeof(TProperty));
            property.Getter = (x) => compiledGetter();
            AddDependencies(property, getPropertyCallback);
            return property;
        }

        public static IPropertyDefinition Property<TProperty>(this IMicroModel model, Expression<Func<TProperty>> propertyGetter)
        {
            propertyGetter = Evaluator.EvaluateClosures(propertyGetter);
            var member = propertyGetter.GetOutermostMember();
            var propertyInfo = (PropertyInfo)member.Member;
            var property = BuildProperty(model, propertyInfo.Name, typeof(TProperty));
            var readEvaluator = new SafeExpressionEvaluator(propertyGetter.Body, new Dictionary<string, object>());
            property.Getter = (x) => readEvaluator.Evaluate();
            AddDependencies(property, propertyGetter);
            if (propertyInfo.CanWrite)
            {
                var writeEvaluator = new SafeExpressionEvaluator(member.Expression, new Dictionary<string, object>());
                property.Setter = (x, y) =>
                                      {
                                          var target = writeEvaluator.Evaluate();
                                          propertyInfo.SetValue(target, y, null);
                                      };
            }
            return property;
        }

        public static IPropertyDefinition Command(this IMicroModel model, string commandPropertyName, Action executedCallback)
        {
            var command = new DelegateCommand(x => executedCallback());
            var property = BuildProperty(model, commandPropertyName, typeof (ICommand));
            property.Getter = x => command;
            return property;
        }

        public static IPropertyDefinition Command<TCommandParameter>(this IMicroModel model, string commandPropertyName, Action<TCommandParameter> executedCallback)
        {
            var command = new DelegateCommand(x => executedCallback((TCommandParameter)x));
            var property = BuildProperty(model, commandPropertyName, typeof(ICommand));
            property.Getter = x => command;
            return property;
        }

        public static IPropertyDefinition Command(this IMicroModel model, string commandPropertyName, Action executedCallback, Func<bool> canExecuteCallback)
        {
            var command = new DelegateCommand(x => executedCallback(), x => canExecuteCallback());
            var property = BuildProperty(model, commandPropertyName, typeof(ICommand));
            property.Getter = x => command;
            return property;
        }

        public static IPropertyDefinition Command<TCommandParameter>(this IMicroModel model, string commandPropertyName, Action<TCommandParameter> executedCallback, Func<TCommandParameter, bool> canExecuteCallback)
        {
            var command = new DelegateCommand(x => executedCallback((TCommandParameter)x), x => canExecuteCallback((TCommandParameter)x));
            var property = BuildProperty(model, commandPropertyName, typeof(ICommand));
            property.Getter = x => command;
            return property;
        }

        public static IEnumerable<IPropertyDefinition> AllProperties(this IMicroModel model, object source)
        {
            var properties = TypeDescriptor.GetProperties(source).OfType<PropertyDescriptor>();
            var propertiesToAdd = new List<IPropertyDefinition>();
            foreach (var sourceProperty in properties)
            {
                var property = sourceProperty;
                var propertyToAdd = new DelegatePropertyDescriptor(property.Name, model, property.PropertyType);
                propertyToAdd.Getter = x => property.GetValue(source);
                if (!property.IsReadOnly)
                {
                    propertyToAdd.Setter = (x, v) => property.SetValue(source, v);
                }

                model.AddProperty(propertyToAdd);
                propertiesToAdd.Add(propertyToAdd);
            }
            return propertiesToAdd;
        }

        public static ICollectionDefinition<TElement> Collection<TElement>(this IMicroModel model, string propertyName, Expression<Func<IEnumerable<TElement>>> items)
        {
            items = Evaluator.EvaluateClosures(items);
            var compiledGetter = items.Compile();
            var property = BuildProperty(model, propertyName, typeof(ObservableCollection<object>));
            var collectionDefinition = new CollectionDefinition<TElement>(property, compiledGetter);
            property.Getter = x => collectionDefinition.Collection;
            AddDependencies(property, items);
            return collectionDefinition;
        }
    }

    public interface ICollectionDefinition<TElement> : IPropertyDefinition
    {
        ICollectionDefinition<TElement> Each(Action<TElement, IMicroModel> definitionEditor);
    } 

    public class ModelFactory
    {
        private readonly List<Action<object, IMicroModel>> _editors = new List<Action<object, IMicroModel>>();

        public void Add(Action<object, IMicroModel> definitionEditor)
        {
            _editors.Add(definitionEditor);
        }

        public IMicroModel Build(object source)
        {
            var model = new MicroModel();
            model.AllProperties(source);
            foreach (var editor in _editors)
            {
                editor(source, model);
            }
            return model;
        }
    }

    public class CollectionDefinition<TElement> : ICollectionDefinition<TElement>
    {
        private readonly IPropertyDefinition _property;
        private readonly Func<IEnumerable<TElement>> _itemsGetter;
        private readonly ModelFactory _modelFactory = new ModelFactory();

        public CollectionDefinition(IPropertyDefinition property, Func<IEnumerable<TElement>> itemsGetter)
        {
            _property = property;
            _itemsGetter = itemsGetter;
        }

        public ICollectionDefinition<TElement> Each(Action<TElement, IMicroModel> definitionEditor)
        {
            _modelFactory.Add((x,y) => definitionEditor((TElement)x,y));
            return this;
        }

        public ObservableCollection<object> Collection
        {
            get
            {
                var source = _itemsGetter();
                var collection = new ObservableCollection<object>();
                foreach (var element in source)
                {
                    collection.Add(_modelFactory.Build(element));
                }
                return collection;
            }
        }

        public IMicroModel Model
        {
            get { return _property.Model; }
        }

        public string Name
        {
            get { return _property.Name; }
            set { _property.Name = value; }
        }

        public Type PropertyType
        {
            get { return _property.PropertyType; }
            set { _property.PropertyType = value; }
        }

        public Func<object, object> Getter
        {
            get { return _property.Getter; }
            set { _property.Getter = value; }
        }

        public Action<object, object> Setter
        {
            get { return _property.Setter; }
            set { _property.Setter = value; }
        }

        public PropertyDescriptor GetDescriptor()
        {
            return _property.GetDescriptor();
        }

        public void AddDependency(IDependencyDefinition dependency)
        {
            _property.AddDependency(dependency);
        }

        public void Seal()
        {
            _property.Seal();
        }
    }
}