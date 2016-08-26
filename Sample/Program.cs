using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Inject();
        }

        static void Inject()
        {
            // Add reference to DDIEngine to project
            var injector = new DDIEngine.Injector();
            if (injector.CurrentStatus == InjectionHelper.Status.Ready)
            {
                System.Diagnostics.Debug.WriteLine("Current results of WhatsTheTime : " + MySourceClass.WhatsTheTime());
                injector.ReplaceStaticMethod(typeof(MySourceClass), typeof(MyMockClass), "WhatsTheTime", "WhatsTheTime_Mock");
                System.Diagnostics.Debug.WriteLine("Current results of WhatsTheTime : " + MySourceClass.WhatsTheTime());
                var wrapper = new MyWrapperClass();
                System.Diagnostics.Debug.WriteLine("Current results of WhatsTheTime : " + wrapper.GetTheTime());
                
            }
          
        }
    }


    public class MySourceClass
    {
        public static string WhatsTheTime()
        {
            return DateTime.Now.ToShortTimeString();
        }
    }

    public class MyWrapperClass
    {
        public string GetTheTime()
        {
            return MySourceClass.WhatsTheTime();
        }
    }
    public class MyMockClass
    {
        public static string WhatsTheTime_Mock()
        {
            return "It's Time to Get Ill.";
        }
    }
}
