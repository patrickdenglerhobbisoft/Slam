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
    /// Interaction logic for SQLLine.xaml
    /// </summary>
    public partial class SQLLine : UserControl
    {
        public string SprocName
        {

            set
            {
                lblSQLMain.Content = "exec " + value;
            }
        }

        public SqlParameterCollection SqlParameters
        {
            set
            {

                lblParameters.Tag = value;
                formatSQL(value);

            }
        }

        public SQLLine()
        {
            InitializeComponent();

        }

        private void formatSQL(SqlParameterCollection SqlParameters)
        {
      //      StackPanelSqlLines.Children.Clear();
            foreach (SqlParameter s in SqlParameters)
            {
                string sqlValue ="";
                if (s.Value !=null)
                     sqlValue = s.Value.ToString();
                
                string commandTypeString = Enum.GetName(typeof(System.Data.SqlDbType), s.SqlDbType);
                //(System.Data.SqlDbType)Enum.ToObject(typeof(System.Data.SqlDbType) , p.CommandType),
                switch ((System.Data.SqlDbType)Enum.ToObject(typeof(System.Data.SqlDbType) ,s.SqlDbType))
                {
                    case System.Data.SqlDbType.Char:
                    case System.Data.SqlDbType.VarChar:
                    case System.Data.SqlDbType.Date:
                    case System.Data.SqlDbType.DateTime:
                    case System.Data.SqlDbType.UniqueIdentifier:
                        sqlValue = "'" + sqlValue + "'";
                        break;


                }

                var line = commandTypeString + " " + s + " = " + sqlValue;

                StackPanelSqlLines.Children.Add(new Label()
                {
                    Content = line,
                });
            }

        }


        private void lblParameters_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //lblParameters.Content = txtParameters.Text;
            //txtParameters.Text = "";
        }
    }
}
