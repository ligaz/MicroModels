using Sample.Infrastructure;
using System;

namespace Sample.Invoicing.BusinessObjects
{
    public class Order : Observable
    {
        private int _id;
        private DateTime _orderDate;
        private string _customerName;
        private string _customerAddress;

        public int Id
        {
            get { return _id; }
            set { _id = value; NotifyChanged("Id"); }
        }

        public DateTime OrderDate
        {
            get { return _orderDate; }
            set { _orderDate = value; NotifyChanged("OrderDate"); }
        }

        public string CustomerName
        {
            get { return _customerName; }
            set { _customerName = value; NotifyChanged("CustomerName"); }
        }

        public string CustomerAddress
        {
            get { return _customerAddress; }
            set { _customerAddress = value; NotifyChanged("CustomerAddress"); }
        }
    }
}


