using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hobbisoft.Slam.DynamicInjection
{

   
    [AttributeUsage(AttributeTargets.All )]
    public class SlamIgnore : Attribute
    {
        public SlamIgnore()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Event | AttributeTargets.Property)]
    public class SlamSquirtee : Attribute
    {
        public SlamSquirtee()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Event | AttributeTargets.Property)]
    public class SlamSquirter : Attribute
    {
        public int InjectionIndex;
        public int InjectionStartPoint;
        public int InjectionEndPoint;
        public SlamSquirter(int injectionIndex=-1, int injectionStartPoint = -1, int injectionEndPoint =  -1 )
        {
            this.InjectionIndex = injectionIndex;
            this.InjectionStartPoint = injectionStartPoint;
            this.InjectionEndPoint = injectionEndPoint;
        }
    }
}
