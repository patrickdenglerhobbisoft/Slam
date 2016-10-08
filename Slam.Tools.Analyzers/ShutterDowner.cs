using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace Hobbisoft.Slam.Tools.Analyzers
{
    public class ShutterDowner
    {
        public static void ShutDown(App app)
        {
           
        }

        internal static void ShutDown(System.Windows.Application application)
        {
            Thread t = new Thread(() =>
            {
                application.Shutdown();
            });
            t.Start();
        }
    }
}
