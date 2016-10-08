using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hobbisoft.Slam.DynamicInjection.UnitTests.Classes
{

    internal interface IMyFactoryFactory
    {

    }

    /// <summary>
    /// SFSession Entry Factory 
    /// </summary>
    internal class MyFactoryFactory : IMyFactoryFactory
    {
        internal static bool MySealedFunction(bool RunAsAdmin, string MyString)
        {
           Log.Output("MySealedFunction called with string '" + MyString + "'");
           return true;
        }
    }

    class BaseWithProtectedVirtuals
    {
        protected virtual void MyFunction() { Log.Output("You called me in the base class."); }

    }
    class SealedProtectedVirtuals : BaseWithProtectedVirtuals
    {
        sealed protected override void MyFunction() { Log.Output("You called me in the derived class"); }

    }

}
