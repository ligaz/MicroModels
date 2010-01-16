using System.Windows;
using Sample.Invoicing.Services;
using Sample.Invoicing.Views;

namespace Sample
{
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            Loaded += WindowLoaded;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Home());
        }
    }
}
