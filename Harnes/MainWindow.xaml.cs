using Hobbisoft.Slam.DynamicInjection.UnitTests.Classes;
using Slam.UnitTests.Classes.External;
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

namespace Harnes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ExternalClass_Slam ecs;
        ExternalClass ec;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            ecs = new ExternalClass_Slam();
            ec = new ExternalClass();
        }

        private void btnUnload_Click(object sender, RoutedEventArgs e)
        {

            ecs = null;
            ec = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
