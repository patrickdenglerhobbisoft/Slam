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

using EnvDTE;


namespace Slam.Tools
{
    /// <summary>
    /// Interaction logic for VisualizerHost.xaml
    /// </summary>
    public partial class VisualizerHost : UserControl
    {
        //private CompositionContainer container;
        private readonly MessageServer _messageServer;
        //[Import(typeof(IVisualizer))]
        public IVisualizer Visualizer;

        public VisualizerHost()
        {
            InitializeComponent();
            InitalizeMefComponents();
            _messageServer = new MessageServer(this);
            System.Threading.Thread t = new System.Threading.Thread(_messageServer.Monitor);
            t.Start();


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
            //catalog.Catalogs.Add(new AssemblyCatalog(typeof(Slam.Tools).Assembly));

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
            if (Visualizer != null)
            {
                
                System.Reflection.Assembly info = Visualizer.GetType().Assembly;
                info.GetCustomAttributes(Visualizer.GetType(),false);
                // var attribute =  (VisualizerAttribute) Attribute.GetCustomAttribute(info., typeof (VisualizerAttribute));
                //switch (attribute.Category)
                //{
                //    case VisualizerCategory.Database:
                //        break;
                //    case  VisualizerCategory.General:
                //        break;
                //    case VisualizerCategory.Profling:
                //        break;
                //    case VisualizerCategory.Slotworker:
                //        break;
                //}
            }
            StackPanelHost.Children.Add(Visualizer as UIElement);
            
        }

        public void RemoveVisualizer(IVisualizer Visualizer)
        {
            // TO DO FIX UP CATEGORIES
            StackPanelHost.Children.Remove(Visualizer as UIElement);
        }
    }
}