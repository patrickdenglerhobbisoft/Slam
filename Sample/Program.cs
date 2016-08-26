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
                // replace static
                System.Diagnostics.Debug.WriteLine("Current results of StaticWhatsTheTime : " + MySourceClass.StaticWhatsTheTime());
                injector.ReplaceStaticMethod(typeof(MySourceClass), typeof(MyMockClass), "StaticWhatsTheTime", "StaticWhatsTheTime_Mock");
                System.Diagnostics.Debug.WriteLine("Current results of StaticWhatsTheTime : " + MySourceClass.StaticWhatsTheTime());
             
                // use new model for non-static - NOT YET WORKING
                
                MySourceClass mySourceClass = new MySourceClass();
                System.Diagnostics.Debug.WriteLine("Current results of WhatsTheTime : " + mySourceClass.WhatsTheTime());
                injector.ReplaceMethod(typeof(MySourceClass), typeof(MyMockClass), "WhatsTheTime", "WhatsTheTime_Mock", null);
                System.Diagnostics.Debug.WriteLine("Current results of WhatsTheTime : " + mySourceClass.WhatsTheTime());

            }
          
        }
    }

    public class MySourceClass
    {
        public static string StaticWhatsTheTime()
        {
            return DateTime.Now.ToShortTimeString();
        }

        public string WhatsTheTime()
        {
            return DateTime.Now.ToShortTimeString();
        }
    }

  
    public class MyMockClass
    {
        public static string StaticWhatsTheTime_Mock()
        {
            return "It's Time to Get Ill.";
        }
        public string WhatsTheTime_Mock()
        {
            return "It's Time to Get Ill.";
        }
    }
}
