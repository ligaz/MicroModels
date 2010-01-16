using System;
using System.ComponentModel;
using MicroModels.Dependencies.Observers;
using MicroModels.Helpers;

namespace MicroModels.Dependencies.PathNavigation.Tokens
{
    /// <summary>
    /// A property monitor for CLR based properties.
    /// </summary>
    internal sealed class ClrMemberToken : MemberToken
    {
        private readonly EventHandler<PropertyChangedEventArgs> _actualHandler;
        private readonly WeakEventProxy<PropertyChangedEventArgs> _weakHandler;
        private readonly PropertyChangedEventHandler _weakHandlerWrapper;
        private IPropertyReader<object> _propertyReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClrMemberToken"/> class.
        /// </summary>
        /// <param name="objectToObserve">The object to observe.</param>
        /// <param name="propertyName">The property path.</param>
        /// <param name="remainingPath">The remaining path.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="pathNavigator">The path navigator.</param>
        public ClrMemberToken(object objectToObserve, string propertyName, string remainingPath, Action<object, string> callback, IPathNavigator pathNavigator)
            : base(objectToObserve, propertyName, remainingPath, callback, pathNavigator)
        {
            _actualHandler = CurrentTarget_PropertyChanged;
            _weakHandler = new WeakEventProxy<PropertyChangedEventArgs>(_actualHandler);
            _weakHandlerWrapper = _weakHandler.Handler;
            
            AcquireTarget(objectToObserve);
        }

        /// <summary>
        /// When overridden in a derived class, gives the class an opportunity to discard the current target.
        /// </summary>
        protected override void DiscardCurrentTargetOverride()
        {
            var currentTarget = CurrentTarget as INotifyPropertyChanged;
            if (currentTarget != null)
            {
                currentTarget.PropertyChanged -= _weakHandlerWrapper;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gives the class an opportunity to monitor the current target.
        /// </summary>
        protected override void MonitorCurrentTargetOverride()
        {
            var currentTarget = CurrentTarget as INotifyPropertyChanged;
            if (currentTarget != null)
            {
                currentTarget.PropertyChanged += _weakHandlerWrapper;
            }
            _propertyReader = PropertyReaderFactory.Create<object>(CurrentTarget.GetType(), PropertyName);
        }

        /// <summary>
        /// When overriden in a derived class, gets the value of the current target object.
        /// </summary>
        /// <returns></returns>
        protected override object ReadCurrentPropertyValueOverride()
        {
            if (_propertyReader != null)
            {
                return _propertyReader.GetValue(CurrentTarget);
            }
            return null;
        }

        private void CurrentTarget_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyName)
            {
                HandleCurrentTargetPropertyValueChanged();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected override void DisposeOverride()
        {
            DiscardCurrentTargetOverride();
        }
    }
}