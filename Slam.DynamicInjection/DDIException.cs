using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hobbisoft.Slam.DynamicInjection
{
    [Serializable]
    public class DDIException : Exception
    {
        public DDIException()
        { }

        public DDIException(string message)
            : base(message)
        { }

        public DDIException(string message, Exception innerException)
            : base(message, innerException)
        { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected DDIException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) { }
    }
}
