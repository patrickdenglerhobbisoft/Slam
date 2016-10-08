using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hobbisoft.Slam.DynamicInjection.UnitTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Hobbisoft.Slam.DynamicInjection.Test.BasicTests basicTests = new Test.BasicTests();
            basicTests.RunAll();
        }
    }
}
