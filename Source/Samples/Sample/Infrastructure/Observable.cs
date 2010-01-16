using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Sample.Infrastructure
{
    /// <summary>
    /// A base class for objects that implement the INotifyPropertyChanged interface.
    /// </summary>
    public abstract class Observable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for every public property on this object.
        /// </summary>
        protected void NotifyChanged()
        {
            var propertyNames = TypeDescriptor.GetProperties(this).OfType<PropertyDescriptor>().Select(x => x.Name);
            NotifyChanged(propertyNames);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for each event in the collection.
        /// </summary>
        /// <param name="propertyNames">The property names.</param>
        protected void NotifyChanged(IEnumerable<string> propertyNames)
        {
            foreach (var property in propertyNames.Distinct())
            {
                OnPropertyChanged(new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the given property names.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        /// <param name="additionalPropertyNames">Any other properties that also changed.</param>
        protected void NotifyChanged(string propertyName, params string[] additionalPropertyNames)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            NotifyChanged(additionalPropertyNames);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, e);
        }
    }
}