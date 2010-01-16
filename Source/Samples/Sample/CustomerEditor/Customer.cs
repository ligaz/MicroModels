using Sample.Infrastructure;

namespace Sample.CustomerEditor
{
    public class Customer : Observable
    {
        private string _firstName;
        private string _lastName;

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; NotifyChanged("FirstName"); }
        }

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; NotifyChanged("FirstName"); }
        }
    }
}
