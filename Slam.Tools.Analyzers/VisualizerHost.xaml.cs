using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel.Composition.Hosting;

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Threading;

namespace Hobbisoft.Slam.Tools.Analyzers
{
    /// <summary>
    /// Interaction logic for VisualizerHost.xaml
    /// </summary>
    [Guid("9ED54F84-A89D-4fcd-A854-44251E922109")]
    public partial class VisualizerHost : UserControl
    {
        private CompositionContainer container;
        private readonly MessageServer _messageServer;
        private readonly MessageServerLog _messageServerLog;

        [Import(typeof(IVisualizer))]
        public IVisualizer Visualizer;
        private Dictionary<string, SQLViz> DBWatchers = new Dictionary<string, SQLViz>();
        public VisualizerHost()
        {
            InitializeComponent();
            InitalizeMefComponents();
            _messageServer = new MessageServer(this);
            _messageServerLog = new MessageServerLog(this);
            System.Threading.Thread t = new System.Threading.Thread(_messageServer.Monitor);
            System.Threading.Thread t1 = new System.Threading.Thread(_messageServerLog.Monitor);

            t.Start();
            t1.Start();

#if DEBUGUI
            SQLViz s = new SQLViz()
            {
                ConnectionString = "Server / Database",
            };
            AddVisualizer(s);
#endif
        }

        private void InitalizeMefComponents()
        {
            ////An aggregate catalog that combines multiple catalogs
            //var catalog = new AggregateCatalog();
            ////Adds all the parts found in the same assembly as the Hobbisoft.Slam_VS_ToolsPackage class
            //catalog.Catalogs.Add(new AssemblyCatalog(typeof(Hobbisoft.Slam.Tools.Analyzers).Assembly));

            ////Create the CompositionContainer with the parts in the catalog
            //container = new CompositionContainer(catalog);

            ////Fill the imports of this object
            //try
            //{
            //    this.container.ComposeParts(this);
            //}
            //catch (CompositionException compositionException)
            //{
            //    Utlities.Log(compositionException.Message);
            //}
        }

        public void AddVisualizer(IVisualizer Visualizer)
        {
          //  (Visualizer as Control).Width = this.Width - 30;
            (Visualizer as Control).Margin = new Thickness(8, 8, 8, 8);
            StackPanelHost.Children.Add(Visualizer as UIElement);


           
       
            
        }

        public void RemoveVisualizer(IVisualizer Visualizer)
        {
            // TO DO FIX UP CATEGORIES
            StackPanelHost.Children.Remove(Visualizer as UIElement);
        }

        private void VisualizerHostWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}