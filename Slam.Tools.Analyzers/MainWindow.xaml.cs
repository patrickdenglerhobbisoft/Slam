using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Hobbisoft.Slam.Tools.Analyzers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
          //  this.Topmost = true;
           // VisualizerHostControl.AddVisualizer(new SQLViz());
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //VisualizerHostControl.Width = this.Width-20;
            //VisualizerHostControl.Height = this.Height - 20;
            //foreach (Control _control in VisualizerHostControl.StackPanelHost.Children)
            //{
            //    _control.Width = this.Width - 40;
               
            //}
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            MessageServerLog._quit = true;
            MessageServer._quit = true;

            Thread.Sleep(500);

            Application.Current.Shutdown();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
           //ShutterDowner.ShutDown(Application.Current);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           // Application.Current.Shutdown();
        }
    }
}                
