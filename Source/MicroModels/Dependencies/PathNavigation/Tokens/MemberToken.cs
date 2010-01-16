using System;

namespace MicroModels.Dependencies.PathNavigation.Tokens
{
    /// <summary>
    /// A base class for objects that monitor a property.
    /// </summary>
    internal abstract class MemberToken : IToken
    {
        private readonly Action<object, string> _changeDetectedCallback;
        private readonly IPathNavigator _pathNavigator;
        private readonly object _propertyMonitorLock = new object();
        private readonly string _propertyName;
        private readonly string _remainingPath;
        private object _currentTarget;
        private IToken _nextMonitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberToken"/> class.
        /// </summary>
        /// <param name="currentTarget">The current target.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="remainingPath">The remaining path.</param>
        /// <param name="changeDetectedCallback">The change detected callback.</param>
        /// <param name="traverser">The traverser.</param>
        public MemberToken(object currentTarget, string propertyName, string remainingPath, Action<object, string> changeDetectedCallback, IPathNavigator traverser)
        {
            _changeDetectedCallback = changeDetectedCallback;
            _remainingPath = remainingPath;
            _propertyName = propertyName;
            _pathNavigator = traverser;
        }

        /// <summary>
        /// Gets the remaining fragments.
        /// </summary>
        public string RemainingPath
        {
            get { return _remainingPath; }
        }

        /// <summary>
        /// Gets the remaining fragments.
        /// </summary>
        public string PropertyName
        {
            get { return _propertyName; }
        }

        /// <summary>
        /// Gets the current target.
        /// </summary>
        protected object CurrentTarget
        {
            get { return _currentTarget; }
        }

        /// <summary>
        /// Gets the property monitor lock.
        /// </summary>
        protected object PropertyMonitorLock
        {
            get { return _propertyMonitorLock; }
        }

        /// <summary>
        /// Gets the traverser.
        /// </summary>
        protected IPathNavigator PathNavigator
        {
            get { return _pathNavigator; }
        }

        #region IToken Members
        /// <summary>
        /// Gets the next monitor.
        /// </summary>
        public IToken NextToken
        {
            get { return _nextMonitor; }
            private set
            {
                if (_nextMonitor != null)
                {
                    _nextMonitor.Dispose();
                }
                _nextMonitor = value;
            }
        }

        /// <summary>
        /// Acquires the target.
        /// </summary>
        /// <param name="target">The target.</param>
        public void AcquireTarget(object target)
        {
            lock (PropertyMonitorLock)
            {
                if (CurrentTarget != null)
                {
                    DiscardCurrentTargetOverride();
                }
                _currentTarget = target;
                if (CurrentTarget != null)
                {
                    MonitorCurrentTargetOverride();
                    NextToken = PathNavigator.TraverseNext(ReadCurrentPropertyValueOverride(), _remainingPath, NextMonitor_ChangeDetected);
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            DisposeOverride();
            NextToken = null;
        }
        #endregion

        /// <summary>
        /// When overridden in a derived class, gives the class an opportunity to discard the current target.
        /// </summary>
        protected abstract void DiscardCurrentTargetOverride();

        /// <summary>
        /// When overridden in a derived class, gives the class an opportunity to monitor the current target.
        /// </summary>
        protected abstract void MonitorCurrentTargetOverride();

        /// <summary>
        /// When overriden in a derived class, gets the value of the current target object.
        /// </summary>
        /// <returns></returns>
        protected abstract object ReadCurrentPropertyValueOverride();

        private void NextMonitor_ChangeDetected(object changedObject, string propertyName)
        {
            ChangeDetected(_propertyName + "." + propertyName);
        }

        /// <summary>
        /// Handles the current target property value changed.
        /// </summary>
        protected void HandleCurrentTargetPropertyValueChanged()
        {
            lock (PropertyMonitorLock)
            {
                var newValue = ReadCurrentPropertyValueOverride();
                NextToken = PathNavigator.TraverseNext(newValue, _remainingPath, NextMonitor_ChangeDetected);
            }
            ChangeDetected(_propertyName);
        }

        /// <summary>
        /// Notifies the parent IPropertyMonitor that a property on the target object has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void ChangeDetected(string propertyName)
        {
            if (_changeDetectedCallback != null)
            {
                _changeDetectedCallback(_currentTarget, propertyName);
            }
        }

        /// <summary>
        /// When overridden in a derived class, lets the derived class dispose any event handlers.
        /// </summary>
        protected abstract void DisposeOverride();
    }
}