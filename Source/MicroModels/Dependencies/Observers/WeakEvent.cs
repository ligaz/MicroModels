using System;

namespace MicroModels.Dependencies.Observers
{
    /// <summary>
    /// Contains methods to build weak event handlers.
    /// </summary>
    public sealed class Weak
    {
        /// <summary>
        /// Creates a weak event handler for a given event handler.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
        /// <param name="eventHandler">The event handler.</param>
        public static WeakEvent<TEventArgs> Event<TEventArgs>(EventHandler<TEventArgs> eventHandler) where TEventArgs : EventArgs
        {
            return new WeakEvent<TEventArgs>(eventHandler);
        }
    }

    /// <summary>
    /// Represents a wrapper around subscribing to a weak event and keeping the event handler callback alive.
    /// </summary>
    /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
    public sealed class WeakEvent<TEventArgs> where TEventArgs : EventArgs
    {
        private readonly EventHandler<TEventArgs> _originalHandler;
        private readonly WeakEventProxy<TEventArgs> _weakProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakEvent&lt;TEventArgs&gt;"/> class.
        /// </summary>
        /// <param name="originalHandler">The original handler.</param>
        public WeakEvent(EventHandler<TEventArgs> originalHandler)
        {
            _originalHandler = originalHandler;
            _weakProxy = new WeakEventProxy<TEventArgs>(_originalHandler);
        }

        /// <summary>
        /// Gets or sets the subscriber.
        /// </summary>
        /// <value>The subscriber.</value>
        public WeakEventProxy<TEventArgs> HandlerProxy
        {
            get { return _weakProxy; }
        }
    }
}