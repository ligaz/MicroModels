using Sample.Infrastructure;

namespace Sample.Invoicing.BusinessObjects
{
    public class LineItem : Observable
    {
        private string _productName;
        private decimal _unitPrice;
        private int _quantity;

        public string ProductName
        {
            get { return _productName; }
            set { _productName = value; NotifyChanged("ProductName"); }
        }

        public decimal UnitPrice
        {
            get { return _unitPrice; }
            set { _unitPrice = value; NotifyChanged("UnitPrice"); }
        }

        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; NotifyChanged("Quantity"); }
        }
    }
}