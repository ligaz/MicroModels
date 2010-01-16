using System.Windows.Controls;

namespace Sample.CustomerEditor
{
    public partial class EditCustomerView : Page
    {
        public EditCustomerView(EditCustomerModel model)
        {
            InitializeComponent();
            DataContext = model;
        }
    }
}
