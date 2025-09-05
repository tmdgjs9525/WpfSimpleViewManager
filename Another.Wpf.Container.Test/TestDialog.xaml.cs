using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfSimpleViewManager.Test
{
    /// <summary>
    /// TestDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TestDialog : UserControl
    {
        public TestDialog()
        {
            InitializeComponent();
            var a = DataContext;
            Loaded += TestDialog_Loaded;
            DataContextChanged += TestDialog_DataContextChanged;
        }

        private void TestDialog_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var a = DataContext;
        }

        private void TestDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var a = DataContext;
        }
    }
}
