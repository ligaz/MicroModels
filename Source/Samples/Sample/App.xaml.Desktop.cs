using System;
using System.Windows;

namespace Sample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.StartupUri = new Uri("Window1.xaml", UriKind.Relative);
        }
    }
}
