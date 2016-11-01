using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slam.UnitTests.Classes
{

    internal interface ISfSessionEntryFactory
    {

    }

    /// <summary>
    /// SFSession Entry Factory 
    /// </summary>
    internal class SfSessionEntryFactory : ISfSessionEntryFactory
    {
        private static bool GetSessionEntry(bool RunAsAdmin, string MyString)
        {
           Log.Output("GetSessionEntry called with string '" + MyString + "'");
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
