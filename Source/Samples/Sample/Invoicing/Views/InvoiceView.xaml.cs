using System.Windows.Controls;

namespace Sample.Invoicing.Views
{
    public partial class InvoiceView : Page
    {
        public InvoiceView(InvoiceViewModel model)
        {
            InitializeComponent();
            DataContext = model;
        }
    }
}
