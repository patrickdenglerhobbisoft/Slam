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


        public MethodInfo FindMethod(MethodDetails methodDetails)
        {

            MethodInfo method = null;
            Type type = methodDetails.Type;
            string methodName = methodDetails.MethodName;
            Tuple<string, Type>[] parameterInfo = methodDetails.ParameterInfo;

            // examine the original method details
            var sourceMethodsToInspect = type.GetMethods().Where(m => m.Name == methodName);
            if (sourceMethodsToInspect.Count == 1)
            {
                // no overloads
                method = sourceMethodsToInspect.FirstOrDefault();
            }
            else
            {
                if (parameterInfo == null)
                {
                    throw new DDIException("The method you are trying to replace is overloaded, but you did not provide parameter information.  Include the parameterinfo in this method call.");
                }
                // let's compare signatures
                foreach (var m in sourceMethodsToInspect)
                {
                    bool isMatch = true; // intialize to true for 0 parameters
                    ParameterInfo[] parameters = m.GetParameters();
                    // do a quick count comparison
                    if (parameters.Length == parameterInfo.Length)
                    {
                        for (var i = 0; i < parameters.Length; i++)
                        {
                            if ((parameterInfo[i].Item1.ToString() != parameters[0].Name) ||
                                ((parameterInfo[i].Item2 as Type) != parameters[0].ParameterType))
                            {
                                isMatch = false;
                                break;
                            }
                        }
                        if (isMatch)
                        {
                            method = m;
                            break;
                        }
                    }
                }
                if (method == null)
                {
                    throw new DDIException("Could not find matching method and/or parameter binding for method " +
                                           methodName + " in type " + type.Name + ".");
                }
            }
            return method;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="replacementType"></param>
        /// <param name="MethodName"></param>
        /// <param name="MethodReplacementName"></param>
        /// <param name="methodDetails">If your method is overloaded, pass in methodDetails to define which overload to select</param>
        /// <returns></returns>
        public bool ReplaceMethod(Type sourceType, Type replacementType, string MethodName, string MethodReplacementName, MethodDetails methodDetails)
        {
            if (methodDetails == null)
            {
                MethodInfo sourceMethod = sourceType.GetMethod(MethodName);
                MethodInfo replacementMethod = replacementType.GetMethod(MethodReplacementName);

                byte[] ilCodes = sourceMethod.GetMethodBody().GetILAsByteArray();
                InjectionHelper.UpdateILCodes(replacementMethod, ilCodes);
            }
            else
            {
                throw new DDIException("Polymorphism support not yet implemented.");

            }
            return true;
        }

        public bool ReplaceAndCallInstanceMethod(Type sourceType, Type replacementType, string MethodName, string MethodReplacementName, bool isPublic)
        {
            // get the target method first

            MethodInfo sourceMethod = sourceType.GetMethod(MethodName, (isPublic ? BindingFlags.Public : BindingFlags.NonPublic) | BindingFlags.Instance);
            MethodInfo replacementMethod = replacementType.GetMethod(MethodReplacementName, (isPublic ? BindingFlags.Public : BindingFlags.NonPublic) | BindingFlags.Instance);

            byte[] ilCodes = sourceMethod.GetMethodBody().GetILAsByteArray();
            InjectionHelper.UpdateILCodes(replacementMethod, ilCodes);
            return true;
        }
        public bool ReplaceStaticMethod(Type sourceType, Type replacementType, string MethodName, string MethodReplacementName)
        {
            MethodInfo sourceMethod = sourceType.GetMethod(MethodName, BindingFlags.Public | BindingFlags.Static);
            MethodInfo replacementMethod = replacementType.GetMethod(MethodReplacementName, BindingFlags.Public | BindingFlags.Static);
          
            byte[] ilCodes = replacementMethod.GetMethodBody().GetILAsByteArray();
            InjectionHelper.UpdateILCodes(sourceMethod, ilCodes);

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

    public class MethodDetails
    {
        public Type Type { get; set; }
        public string MethodName { get; set; }
        public Tuple<string, Type>[] ParameterInfo { get; set; }
    }
}
