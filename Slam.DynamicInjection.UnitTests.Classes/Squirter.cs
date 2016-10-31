using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slam.DynamicInjection;

namespace Slam.UnitTests.Classes
{
    public class Squirter
    {

        [SlamSquirter(8)]
        public static void MyFunction()
        {

            Log.Output("More Function Body 1.");
            
            Log.Output("TOTALLY DIFFERENT LINE");
        }
    }
}
