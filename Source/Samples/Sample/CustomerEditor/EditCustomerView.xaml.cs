namespace Sample.CustomerEditor
{
    public partial class EditCustomerView
    {
        public EditCustomerView()
        {
            InitializeComponent();
            DataContext = CreateModel();
        }

        private static EditCustomerModel CreateModel()
        {
            var customer = new Customer();
            customer.FirstName = "John";
            customer.LastName = "Smith";

            return new EditCustomerModel(customer, new CustomerRepository());
        }
    }
}
