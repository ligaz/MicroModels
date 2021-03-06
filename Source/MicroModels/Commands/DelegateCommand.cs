﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MicroModels.Utilities;

namespace MicroModels.Commands
{
    internal class DelegateCommand : ICommand
    {
        private readonly Func<object, bool> _canExecuteMethod;
        private readonly Action<object> _executeMethod;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> executeMethod)
            : this(executeMethod, null)
        {
        }

        public DelegateCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            Guard.ArgumentNotNull(executeMethod, "executeMethod");
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        public bool CanExecute(object parameter)
        {
            return ((_canExecuteMethod == null) || _canExecuteMethod(parameter));
        }

        public void Execute(object parameter)
        {
            _executeMethod(parameter);
        }

        protected virtual void OnCanExecuteChanged()
        {
            var canExecuteChangedHandler = CanExecuteChanged;
            if (canExecuteChangedHandler == null) return;

            Dispatcher dispatcher = Dispatcher;
            if (!((dispatcher == null) || dispatcher.CheckAccess()))
            {
                dispatcher.BeginInvoke(new Action(this.OnCanExecuteChanged));
            }
            else
            {
                canExecuteChangedHandler(this, EventArgs.Empty);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute(parameter);
        }

        void ICommand.Execute(object parameter)
        {
            Execute(parameter);
        }

        private static Dispatcher Dispatcher
        {
            get
            {
#if SILVERLIGHT
                if (Deployment.Current != null)
                {
                    return Deployment.Current.Dispatcher;
                }
#else
                if (Application.Current != null)
                {
                    return Application.Current.Dispatcher;
                }
#endif
                return null;
            }
        }
    }
}
