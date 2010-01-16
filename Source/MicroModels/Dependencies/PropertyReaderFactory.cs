using System;
using System.Collections.Generic;
using System.Reflection;

namespace MicroModels.Helpers
{
    /// <summary>
    /// Provides the ability to read the value of a property from a given object without using reflection.
    /// </summary>
    internal static class PropertyReaderFactory
    {
        private static readonly Dictionary<string, object> _readers = new Dictionary<string, object>();

        /// <summary>
        /// Creates a reader that reads a specific type of property from a given object type by creating a delegate to access it.
        /// </summary>
        /// <typeparam name="TCast">The type to cast the returning property to.</typeparam>
        /// <param name="objectType">The type of the object containing the property.</param>
        /// <param name="propertyName">The name of the property that will be read.</param>
        /// <returns>A strongly typed property reader that will read the property value without reflection.</returns>
        public static IPropertyReader<TCast> Create<TCast>(Type objectType, string propertyName)
        {
            var key = objectType.AssemblyQualifiedName + "-" + propertyName;
            IPropertyReader<TCast> result = null;
            if (_readers.ContainsKey(key))
            {
                result = _readers[key] as IPropertyReader<TCast>;
            }
            if (result == null)
            {
                var propertyInfo = objectType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (propertyInfo != null)
                {
                    if (typeof (TCast).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        var delegateReaderType = typeof (Func<,>).MakeGenericType(propertyInfo.DeclaringType, propertyInfo.PropertyType);
                        var readerType = typeof (DelegatePropertyReader<,,>).MakeGenericType(propertyInfo.DeclaringType, propertyInfo.PropertyType, typeof (TCast));
                        var propertyGetterMethodInfo = propertyInfo.GetGetMethod(true);
                        if (propertyGetterMethodInfo == null)
                        {
                            throw new ArgumentException(string.Format("The property '{0}' on type '{1}' does not contain a getter which could be accessed by the MicroModels binding infrastructure.", propertyName, propertyInfo.DeclaringType));
                        }
                        var propertyGetterDelegate = Delegate.CreateDelegate(delegateReaderType, propertyGetterMethodInfo);
                        result = (IPropertyReader<TCast>) Activator.CreateInstance(readerType, propertyGetterDelegate);
                        _readers[key] = result;
                    }
                }
                else
                {
                    var fieldInfo = objectType.GetField(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (fieldInfo != null)
                    {
                        if (typeof (TCast).IsAssignableFrom(fieldInfo.FieldType))
                        {
                            result = new FieldReader<TCast>(fieldInfo);
                            _readers[key] = result;
                        }
                    }
                }
            }
            return result;
        }

        #region Nested type: DelegatePropertyReader
        /// <summary>
        /// Private implementation of IDelegatePropertyReader.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <typeparam name="TCast">The type to cast the result to.</typeparam>
        private class DelegatePropertyReader<TInput, TReturn, TCast> : IPropertyReader<TCast>
            where TReturn : TCast
        {
            private readonly Func<TInput, TReturn> _caller;

            /// <summary>
            /// Initializes a new instance of the <see cref="DelegatePropertyReader&lt;TInput, TReturn, TCast&gt;"/> class.
            /// </summary>
            /// <param name="caller">The caller.</param>
            public DelegatePropertyReader(Func<TInput, TReturn> caller)
            {
                _caller = caller;
            }

            #region IPropertyReader<TCast> Members
            /// <summary>
            /// Gets the value.
            /// </summary>
            /// <param name="input">The input.</param>
            /// <returns></returns>
            public TCast GetValue(object input)
            {
                return _caller((TInput) input);
            }
            #endregion
        }
        #endregion

        #region Nested type: FieldReader
        /// <summary>
        /// Private implementation of IDelegatePropertyReader.
        /// </summary>
        /// <typeparam name="TCast">The type to cast the result to.</typeparam>
        private class FieldReader<TCast> : IPropertyReader<TCast>
        {
            private readonly FieldInfo _field;

            /// <summary>
            /// Initializes a new instance of the <see cref="DelegatePropertyReader&lt;TInput, TReturn, TCast&gt;"/> class.
            /// </summary>
            /// <param name="field">The field.</param>
            public FieldReader(FieldInfo field)
            {
                _field = field;
            }

            #region IPropertyReader<TCast> Members
            /// <summary>
            /// Gets the value.
            /// </summary>
            /// <param name="input">The input.</param>
            /// <returns></returns>
            public TCast GetValue(object input)
            {
                try
                {
                    return (TCast) _field.GetValue(input);
                }
                catch
                {
                    return default(TCast);
                }
            }
            #endregion
        }
        #endregion
    }
}