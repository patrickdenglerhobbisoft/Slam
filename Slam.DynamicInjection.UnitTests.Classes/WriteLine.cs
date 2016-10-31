using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;


using System.IO;
//using System.Text;
using System.Globalization;
using System.Security;
using Microsoft.Win32;

using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Versioning;
using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;
using Slam.DynamicInjection;


namespace Slam.UnitTests.Classes
{
    public static class Console_Slam
    {
        /// <devdoc>
        ///    <para>Writes a message followed by a line terminator to the trace listeners in the
        ///    <see cref='System.Diagnostics.Debug.Listeners'/> collection. The default line terminator 
        ///       is a carriage return followed by a line feed (\r\n).</para>
        /// </devdoc>
        [HostProtection(UI = true)]
        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        public static void WriteLine(String value)
        {
            DefaultInjectors.LogMessage(value);
        }
    }
}
