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
    public partial class SQLViz : UserControl, IVisualizer
    {
        public string ConnectionString { get { return txtServerInfo.Text; } set { txtServerInfo.Text = value; } }

        public SQLViz()
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


        private string formatSQL(List<SqlRemoteParameter> SqlParameters)
        {
            string final = "";
            foreach (SqlRemoteParameter s in SqlParameters)
            {

              
                //string sqlValue ="";
                //if (s.SqlValue !=null)
                //    sqlValue = s.SqlValue.ToString();
                
                //string commandTypeString = Enum.GetName(typeof(System.Data.SqlDbType), s.SqlDbType);
                ////(System.Data.SqlDbType)Enum.ToObject(typeof(System.Data.SqlDbType) , p.CommandType),
                //switch ((System.Data.SqlDbType)Enum.ToObject(typeof(System.Data.SqlDbType) ,s.SqlDbType))
                //{
                //    case System.Data.SqlDbType.Char:
                //    case System.Data.SqlDbType.VarChar:
                //    case System.Data.SqlDbType.Date:
                //    case System.Data.SqlDbType.DateTime:
                //    case System.Data.SqlDbType.UniqueIdentifier:
                //        sqlValue = "'" + sqlValue + "'";
                //        break;


                //}

                var line = s.ParameterName + " = " + s.ParameterValue;
                    final += line + ", ";
            }

            return final;
           }

        internal void AddSQLStatement(SqlRemote sqlRemote)
        {

            StackPanelStatements.Text += "exec " + sqlRemote.CommandText + " " + formatSQL(sqlRemote.SqlParameters) + "\r\n\r\n";
         //   StackPanelStatements.Items.Add("exec " + sqlRemote.CommandText + " " + formatSQL(sqlRemote.SqlParameters));
    
            //  string text = "exec " + cmd.CommandText;

        }
    }
}
