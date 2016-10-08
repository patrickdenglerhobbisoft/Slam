using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Hobbisoft.Slam.DynamicInjection.UnitTests.Classes
{
    public enum LogColor { Default }
    public static class Log
    {
        public static void Output(string message)
        {
            Output(message,LogColor.Default);
        }

        public static void Output(string message, LogColor logColor)
        {
            string prefix = "";
            switch (logColor)
            {
                case LogColor.Default:
                    prefix = "+++>";
                    break;
            }

            Debug.WriteLine(prefix + " " + message);


        }
    }
}
