using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Reflection.Emit;
using System.Linq.Expressions;
using System.Runtime.Versioning;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ThreadState = System.Threading.ThreadState;

namespace DDIEngine
{
    public class Injector
    {
        private InjectionHelper.Status currentStatus = InjectionHelper.Status.Uninitialized;
        public InjectionHelper.Status CurrentStatus
        {
            get { return currentStatus; }
            private set { currentStatus = value; }
        }
        public Injector()
        {
         
            int errorCountAllowed = 3;
            try
            {
            
                InjectionHelper.Initialize();
                System.Threading.Thread.Sleep(1000);
                do
                {
                    currentStatus = InjectionHelper.GetStatus();
                    Log("Waiting");
                    if (currentStatus.ToString().ToUpper().Contains("ERROR")) ;
                    {
                        if (errorCountAllowed-- <= 0)
                            break;
                    }

                } while ( currentStatus != InjectionHelper.Status.Ready);
            

                if (currentStatus == InjectionHelper.Status.Ready)
                {
                    IniALL.Ini();
                 
                }
            }
            catch (Exception e)
            {
                Log("Exception occurred : " + e.Message);
            }
        }

     


        public bool ReplaceAndCallInstanceMethod(Type sourceType, Type targetType, string MethodName, string MethodReplacementName ,bool isPublic )
        {
            // get the target method first

            MethodInfo targetMethod = sourceType.GetMethod(MethodName, (isPublic ? BindingFlags.Public : BindingFlags.NonPublic) | BindingFlags.Instance);
            MethodInfo sourceMethod = targetType.GetMethod(MethodReplacementName, (isPublic ? BindingFlags.Public : BindingFlags.NonPublic) | BindingFlags.Instance);

            byte[] ilCodes = sourceMethod.GetMethodBody().GetILAsByteArray();
            InjectionHelper.UpdateILCodes(targetMethod, ilCodes);
            return true;
        }
        public bool ReplaceStaticMethod(Type sourceType, Type targetType, string MethodName, string MethodReplacementName)
        {
            MethodInfo targetMethod = sourceType.GetMethod(MethodName, BindingFlags.Public | BindingFlags.Static);
            MethodInfo srcMethod = targetType.GetMethod(MethodReplacementName, BindingFlags.Public | BindingFlags.Static);
          
            byte[] ilCodes = srcMethod.GetMethodBody().GetILAsByteArray();
            InjectionHelper.UpdateILCodes(targetMethod, ilCodes);

            return true;
        }

        void Log(string s)
        {

            try
            {
                using (StreamWriter sw = File.AppendText(@"d:\ilresults.txt"))
                {
                    sw.WriteLine(DateTime.Now.ToShortTimeString() + " : " + s + " -- status -- " + currentStatus.ToString());

                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
