using System;
using System.Diagnostics;

namespace MicroModels.Dependencies.Observers
{
    /// <summary>
    /// An event handler wrapper used to create weak-reference event handlers, so that event subscribers 
    /// can be garbage collected without the event publisher interfering. 
    /// </summary>
    /// <typeparam name="TEventArgs">The type of event arguments used in the event handler.</typeparam>
    /// <remarks>
    /// To understand why this class is needed, see this page: 
    ///     http://www.paulstovell.net/blog/index.php/wpf-binding-bug-leads-to-possible-memory-issues/
    /// For examples on how this is used, it is best to look at the unit test: 
    ///     WeakEventProxyTests.cs
    /// </remarks>
    public sealed class WeakEventProxy<TEventArgs> 
        where TEventArgs : EventArgs
    {
        private readonly WeakReference _callbackReference;
        private readonly object _lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakEventProxy&lt;TEventArgs&gt;"/> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public WeakEventProxy(EventHandler<TEventArgs> callback)
        {
            _callbackReference = new WeakReference(callback, true);
        }

        /// <summary>
        /// Used as the event handler which should be subscribed to source collections.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [DebuggerNonUserCode]
        public void Handler(object sender, TEventArgs e)
        {
            var callback = _callbackReference.Target as EventHandler<TEventArgs>;
            if (callback != null)
            {
                callback(sender, e);
            }
        }
    }
}