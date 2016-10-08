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
using System.Data.SqlClient;

namespace Hobbisoft.Slam.Tools.Analyzers
{
    /// <summary>
    /// Interaction logic for SQLViz.xaml
    /// </summary>
    /// 
    [VisualizerAttribute(VisualizerCategory.Database)]
    public partial class ErrorLog : UserControl, IVisualizer
    {
    
        public ErrorLog()
        {
            InitializeComponent();

#if DEBUGUI
            AddSQLStatement("exec sprocName", new SqlParameter[]
                {
                   
                   
                         new SqlParameter()
                         {
                             ParameterName = "@ID",
                             SqlDbType = System.Data.SqlDbType.UniqueIdentifier,
                             SqlValue = 100,
                         },
                         new SqlParameter()
                         {
                             ParameterName = "@Name",
                                SqlDbType = System.Data.SqlDbType.VarChar,
                             SqlValue = "Patrick",
                         },
                          new SqlParameter()
                         {
                             ParameterName = "@MID",
                                SqlDbType = System.Data.SqlDbType.Int,
                             SqlValue = 123123,
                         },
               
                });
#endif
        }


       
        internal void AddStatement(LoggerRemote loggerRemote)
        {

            StackPanelStatements.Text += loggerRemote.Message + "\r\n\r\n";
         
        }
    }
}
