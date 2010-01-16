using System;
using System.ComponentModel;

namespace MicroModels.Tests
{
    public class Customer : INotifyPropertyChanged
    {
        private string _firstName;
        private string _lastName;
        private DateTime _dateOfBirth;

        public event PropertyChangedEventHandler PropertyChanged;

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; OnPropertyChanged(new PropertyChangedEventArgs("LastName")); }
        }

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; OnPropertyChanged(new PropertyChangedEventArgs("LastName")); }
        }

        public DateTime DateOfBirth
        {
            get { return _dateOfBirth; }
            set { _dateOfBirth = value; OnPropertyChanged(new PropertyChangedEventArgs("LastName")); }
        }

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, e);
        }
    }
}