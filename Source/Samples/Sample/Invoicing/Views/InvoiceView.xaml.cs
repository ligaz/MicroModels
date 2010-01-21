using Sample.Invoicing.Services;

namespace Sample.Invoicing.Views
{
    public partial class InvoiceView
    {
        public InvoiceView()
        {
            InitializeComponent();
            DataContext = CreateModel();
        }

        private static InvoiceViewModel CreateModel()
        {
            var service = new OrderService();
            return new InvoiceViewModel(service.GetOrder(1), service.GetLineItems(1), service);
        }
    }
}
