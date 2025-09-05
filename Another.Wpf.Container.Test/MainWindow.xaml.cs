using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows;

namespace WpfSimpleViewManager.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainView
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetRequiredService<MainWindowViewModel>(); ;
        }
    }
}