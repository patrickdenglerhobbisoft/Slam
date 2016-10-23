using Hobbisoft.Slam.DynamicInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Slam.UnitTests.Classes.External
{
    public class ExternalClass_Slam
    {
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public string ImExternal()
        {
         
            InjectionHelper.Log("In #2");
            return "I'm in DLL2 - SLam";
        }



    }
}
