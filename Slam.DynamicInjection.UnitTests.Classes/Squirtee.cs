using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hobbisoft.Slam.DynamicInjection;

namespace Hobbisoft.Slam.DynamicInjection.UnitTests.Classes
{
    public class Squirtee
    {
     
        public static void MyFunction()
        {
            Log.Output("Begginning Function Body.");
            Log.Output("More Function Body 1.");


            Log.Output("More Function Body 2.");
            Log.Output("Remainder of Function Body.");
        }
    }
}
