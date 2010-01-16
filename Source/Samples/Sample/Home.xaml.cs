using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Sample.CustomerEditor;
using Sample.Invoicing.Services;
using Sample.Invoicing.Views;

namespace Sample
{
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
        }

        private void GoToInvoiceEditor(object sender, RoutedEventArgs e)
        {
            var service = new OrderService();
            var model = new InvoiceViewModel(service.GetOrder(1), service.GetLineItems(1), service);
            var view = new InvoiceView(model);
            NavigationService.Navigate(view);
        }

        private void GoToCustomerEditorDemo(object sender, RoutedEventArgs e)
        {
            var customer = new Customer();
            customer.FirstName = "John";
            customer.LastName = "Smith";
            var model = new EditCustomerModel(customer, new CustomerRepository());
            var view = new EditCustomerView(model);
            NavigationService.Navigate(view);
        }
    }
}
