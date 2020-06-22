using System;
using System.Windows;

namespace Humidity.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(Object dataContext)
        {
            InitializeComponent();

            DataContext = dataContext;
        }
    }
}
