using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hobbisoft.Slam.DynamicInjection.UnitTests.Classes
{



    public class ClassWithOverloadedFunctions
    {
        public string GetString(int Number)
        {
            return Number.ToString();
        }
        public string GetString(string Number)
        {
            return Number;
        }


    }


    public class ClassWithOverloadedFunctions_Slam
    {
        [SlamIgnore]
        string IngoreThisOne()
        {
            return "foo";
        }

        public string GetString(int Number)
        {
            return (Number + 5).ToString();
        }
        public string GetString(string Number)
        {
            return (Convert.ToInt16(Number) + 5).ToString();
        }
    }

    //internal static class MyFactoryFactory_Slam
    //{
    //    internal static MyFactory MySealedFunction(bool RunAsAdmin, ETPrincipal Principal)
    //    {
    //        Log.Output("Mocked Class Invoked");
    //        return null;

    //    }
    //}
    public class MySourceClass
    {
        public string MyProperty { get; set; }


        public static string PublicStaticWhatsTheTime()
        {
            return DateTime.Now.ToShortTimeString();
        }


        public string WhatsTheTimeCaller()
        {
            return WhatsTheTime();
        }

        private string WhatsTheTime()
        {
            return DateTime.Now.ToShortTimeString();
        }
    }


    public class MyMockClass
    {
        public string MyProperty_Slam { get; set; }


        public static string PublicStaticWhatsTheTime_Slam()
        {
            return "It's Time to Get Ill (Public Static)";
        }


        private string WhatsTheTime_Slam()
        {
            return "It's Time to Get Ill (private Instance)";
        }
    }

   

    public class PrimaryClassWithThreeFunctions
    {
        void Function1()
        {
          
        }

        void Function2()

        {
            
        }

        void Function3()
        {
            
        }
    }

   
    public class PrimaryClassWithThreeFunctions_Slam
    {
        void Function1()
        {

        }

        void Function2()
        {

        }

       
    }
}
