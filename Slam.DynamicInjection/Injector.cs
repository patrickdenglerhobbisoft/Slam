using EasyHook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Hobbisoft.Slam.DynamicInjection
{

    public class MethodInfoRestoration
    {

        public MethodInfo OriginalMethod {get;set;}
        public MethodInfo ReplacedMethod { get; set; }


        public byte[] OriginalMethodDetails { get; set; }
        public byte[] ReplacedMethodDetails { get; set; }
    

    }

    public enum SlamType {  Replace, Squirt }
   


    public class Injector : IDisposable
    {

         private static BindingFlags bf = BindingFlags.FlattenHierarchy | BindingFlags.IgnoreReturn | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.SuppressChangeType;


         public static Dictionary<string, List<MethodInfoRestoration>> ClassInfoRestoration;
        public static void SlamClass(Type sourceType, Type replacementType, Type finalModelType = null)
        {
            InitializeIfNot();
            _singleInjector.SlamClassInternal(sourceType, replacementType);
        }

        public static List<Byte[]> GetILForMethod(Type type, string methodName)
        {
            InitializeIfNot();
            return _singleInjector.GetILForMethodInternal(type, methodName);
        }

        private static void InitializeIfNot()
        {
            if (_singleInjector == null)
            {
                _singleInjector = new Injector();
                ClassInfoRestoration = new Dictionary<string, List<MethodInfoRestoration>>();
            }
        }
        private List<Byte[]> GetILForMethodInternal(Type type, string methodName)
        {
            MethodInfo methodToDempose = null;
            List<Byte[]> results = new  List<byte[]>();
            foreach (MethodInfo methodToInspect in type.GetMethods(bf))
            {
                if (methodToInspect.Name == methodName)
                {
                    // let's decompose regardless of signature
                    results.Add(methodToInspect.GetMethodBody().GetILAsByteArray());
                }
            }

#if DEBUG
            foreach (var result in results)
                OutputIL(type.Name,methodName,result);
#endif

            return results;
            
        }

     /*
Description:

    Injects a library into the target process. This is a very stable operation.
    The problem so far is, that only the NET layer will support injection
    through WOW64 boundaries and into other terminal sessions. It is quite
    complex to realize with unmanaged code and that's why it is not supported!

    If you really need this feature I highly recommend to at least look at C++.NET
    because using the managed injection can speed up your development progress
    about orders of magnitudes. I know by experience that writing the required
    multi-process injection code in any unmanaged language is a rather daunting task!

Parameters:

    - InTargetPID

        The process in which the library should be injected.
    
    - InWakeUpTID

        If the target process was created suspended (RhCreateAndInject), then
        this parameter should be set to the main thread ID of the target.
        You may later resume the process from within the injected library
        by calling RhWakeUpProcess(). If the process is already running, you
        should specify zero.

    - InInjectionOptions

        All flags can be combined.

        EASYHOOK_INJECT_DEFAULT: 
            
            No special behavior. The given libraries are expected to be unmanaged DLLs.
            Further they should export an entry point named 
            "NativeInjectionEntryPoint" (in case of 64-bit) and
            "_NativeInjectionEntryPoint@4" (in case of 32-bit). The expected entry point 
            signature is REMOTE_ENTRY_POINT.

        EASYHOOK_INJECT_MANAGED: 
        
            The given user library is a NET assembly. Further they should export a class
            named "EasyHook.InjectionLoader" with a static method named "Main". The
            signature of this method is expected to be "int (String)". Please refer
            to the managed injection loader of EasyHook for more information about
            writing such managed entry points.

        EASYHOOK_INJECT_STEALTH:

            Uses the experimental stealth thread creation. If it fails
            you may try it with default settings. 

		EASYHOOK_INJECT_HEART_BEAT:
			
			Is only used internally to workaround the managed process creation bug.
			For curiosity, NET seems to hijack our remote thread if a managed process
			is created suspended. It doesn't do anything with the suspended main thread,


    - InLibraryPath_x86

        A relative or absolute path to the 32-bit version of the user library being injected.
        If you don't want to inject into 32-Bit processes, you may set this parameter to NULL.

    - InLibraryPath_x64

        A relative or absolute path to the 64-bit version of the user library being injected.
        If you don't want to inject into 64-Bit processes, you may set this parameter to NULL.

    - InPassThruBuffer

        An optional buffer containg data to be passed to the injection entry point. Such data
        is available in both, the managed and unmanaged user library entry points.
        Set to NULL if no used.

    - InPassThruSize

        Specifies the size in bytes of the pass thru data. If "InPassThruBuffer" is NULL, this
        parameter shall also be zero.

Returns:
      * */
        public static void Slam( object classToSlam)
        {
            InjectionHelper.Log("entered Slam");
            try
            {
                Type classTypeToSlam = classToSlam.GetType();
                if (ClassInfoRestoration.ContainsKey(classTypeToSlam.Name))
                {
                    foreach (var restorationItem in ClassInfoRestoration[classTypeToSlam.Name])
                    {
                        if (!restorationItem.OriginalMethod.IsStatic)
                        {
                            foreach (MethodInfo method in classTypeToSlam.GetMethods(bf))
                            {
                                if (method.Name == restorationItem.OriginalMethod.Name)
                                {
                                    InjectionHelper.UpdateILCodes(method, restorationItem.ReplacedMethodDetails);
                                }
                            }
                        
                        }
                    }
                    
                }
            }
            catch (Exception e)
            {
                InjectionHelper.Log(e.Message);
                if (e.StackTrace !=null)                  InjectionHelper.Log(e.StackTrace);
               if (e.InnerException !=null && e.InnerException.Message!=null) InjectionHelper.Log(e.InnerException.Message);
                    
            }

        }
        private void SlamClassInternal(Type sourceType, Type replacementType)
        {
          
            var methodReplacements = new List<MethodInfoRestoration>();
            BindingFlags bf = BindingFlags.FlattenHierarchy | BindingFlags.IgnoreReturn | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.SuppressChangeType;
            MethodInfo methodFromSource = null;
            MethodInfo methodFromSquirt=null; // 

            var p = Process.GetProcesses();
            foreach  ( var o in p)
            {
                try
                {
                    foreach (ProcessModule m in o.Modules)
                    {
                        Log(m.ModuleName + " in process " + o.ProcessName + " " + " Id: " + o.Id);
                    }
                }
                catch (Exception e)
                {
                    Log(e.Message);
                }
            }
            // walk the replacment classes (likely) subset of members and inject into source
            foreach (MethodInfo methodFromReplacement in replacementType.GetMethods(bf))
            {
                // premptive strike that needs to be replaced with better determiners
                if ((methodFromReplacement.Name == "ToString") || (methodFromReplacement.Name == "Dispose") || (methodFromReplacement.Name == "GetHashCode") || (methodFromReplacement.Name == "Equals") ||
                    (methodFromReplacement.Name == "ReferenceEquals") || (methodFromReplacement.Name == "GetType") || (methodFromReplacement.Name == "Finalize") || (methodFromReplacement.Name == "MemberwiseClone"))
                    continue;

        

                // check for SlamIngore attribute
                if (methodFromReplacement.GetCustomAttributes().Where(a => a.TypeId.ToString() == "Hobbisoft.Slam.DynamicInjection.SlamIgnore").Count > 0)
                    continue;

                methodFromSource = null;
                // make sure its one we actually can replace TODO: Add logging to those that cannot be
                if (
                        methodFromReplacement.GetMethodImplementationFlags() == MethodImplAttributes.IL
                    || methodFromReplacement.GetMethodImplementationFlags() == MethodImplAttributes.NoInlining
                    )
                {
                    bool isOverLoaded = false;
                    var replacementMethodParameterInfo = methodFromReplacement.GetParameters();
                    try
                    {
                        methodFromSource = sourceType.GetMethod(methodFromReplacement.Name.Replace("_Slam", ""), bf);
                    }

                    catch (System.Reflection.AmbiguousMatchException ambigous)
                    {
                        isOverLoaded = true;
                    }
                    if (isOverLoaded)
                    {
                        // this means that we have an overloaded method now we have to look more closely
                        var methods = sourceType.GetMethods(bf).Where(m => m.Name == methodFromReplacement.Name);

                        foreach (var method in methods)
                        {
                            var sourceMethodParameterInfo = method.GetParameters();
                            if (sourceMethodParameterInfo.Length == replacementMethodParameterInfo.Length)
                            {
                                bool found = true;
                                methodFromSource = null;
                                if (replacementMethodParameterInfo.Length == 0)
                                {
                                    methodFromSource = method;  // doing it below already
                                    break;
                                }

                                for (var i = 0; i < sourceMethodParameterInfo.Length; i++)
                                {
                                    if (sourceMethodParameterInfo[i].ParameterType != replacementMethodParameterInfo[i].ParameterType)
                                    {
                                        found = false;

                                    }
                                }

                                if (found)
                                {
                                    methodFromSource = method;
                                    break;
                                }
                            }



                        }
                    }


                    // let's store this stuff
                    MethodInfoRestoration methodInfoRestoration = new MethodInfoRestoration()
                    {
                         OriginalMethod = methodFromSource,
                         ReplacedMethod = methodFromReplacement,
                      
                         
                        OriginalMethodDetails = methodFromSource.GetMethodBody().GetILAsByteArray(),
                         ReplacedMethodDetails = methodFromReplacement.GetMethodBody().GetILAsByteArray(),
                       
                    };
                    methodReplacements.Add(methodInfoRestoration);

                    // could move this out of the look if we preserve sourceType.
                    if (!ClassInfoRestoration.ContainsKey(sourceType.Name))
                        ClassInfoRestoration.Add(sourceType.Name, new List<MethodInfoRestoration>());

                    ClassInfoRestoration[sourceType.Name].Add(methodInfoRestoration);

                   
                }
            }


            byte[] byteArrayOfNewFunction;
            
            // do IL work (for now doing it seperately
            foreach (var m in methodReplacements)
            {
                // check for slam vs. squirt
                if (
                    m.OriginalMethod.GetCustomAttributes()
                        .Where(a => a.TypeId.ToString() == "Hobbisoft.Slam.DynamicInjection.SlamSquirtee")
                        .Count > 0)
                {

                   
                    int indexOfSquirtHere = m.ReplacedMethod.GetCustomAttribute<SlamSquirter>().InjectionIndex;
                    if (indexOfSquirtHere < 0 || indexOfSquirtHere > m.OriginalMethodDetails.Length)
                    {
                        throw new Exception("Index out of bounds");
                    }
                
                    
                    byteArrayOfNewFunction = new byte[m.OriginalMethodDetails.Length + m.ReplacedMethodDetails.Length];

                    Array.Copy(m.OriginalMethodDetails, 0, byteArrayOfNewFunction, 0, indexOfSquirtHere );

                    Array.Copy(m.ReplacedMethodDetails, 0, byteArrayOfNewFunction, indexOfSquirtHere, m.ReplacedMethodDetails.Length);

                    Array.Copy(m.ReplacedMethodDetails, indexOfSquirtHere, byteArrayOfNewFunction, indexOfSquirtHere - 1 + m.ReplacedMethodDetails.Length, m.ReplacedMethodDetails.Length - indexOfSquirtHere);

                    //// then commit to the IL which is now in originalmethod details
                    InjectionHelper.UpdateILCodes(m.OriginalMethod, byteArrayOfNewFunction);

                    // output bytes 
                    // squirtee 
                    OutputIL(sourceType.Name, m.OriginalMethod.Name + " (Squirtee)", m.OriginalMethodDetails);
                    // squirter
                    OutputIL(sourceType.Name, m.OriginalMethod.Name + " (Squirter)", m.ReplacedMethodDetails);
                    // squirter
                    OutputIL(sourceType.Name, m.OriginalMethod.Name + " (New Squirtee)", byteArrayOfNewFunction);
                   

                }
                else
                {
                   if (m.OriginalMethod.IsStatic) // OK to slam statics
                        InjectionHelper.UpdateILCodes(m.OriginalMethod, m.ReplacedMethodDetails);
                   else
                   {
                       HookAndUpdate(m);
                   }
                }
            }
        }

        private void HookAndUpdate(MethodInfoRestoration m)
        {
             
            
           
            MethodInfo targetMethod = m.OriginalMethod;
            MethodInfo replaceMethod = m.ReplacedMethod;

            byte[] ilCodes = new byte[5];
            ilCodes[0] = (byte)OpCodes.Jmp.Value;
            ilCodes[1] = (byte)(replaceMethod.MetadataToken & 0xFF);
            ilCodes[2] = (byte)(replaceMethod.MetadataToken >> 8 & 0xFF);
            ilCodes[3] = (byte)(replaceMethod.MetadataToken >> 16 & 0xFF);
            ilCodes[4] = (byte)(replaceMethod.MetadataToken >> 24 & 0xFF);

            InjectionHelper.UpdateILCodes(targetMethod, ilCodes);

            
        }
        

        
        private int FindBytes(byte[] haystack, byte[] needle)
        {
          
            int insertIndex = -1;
            var len = needle.Length;
            var limit = haystack.Length - len;
            for (var i = 0; i <= limit; i++)
            {
                var k = 0;
                for (k = 0; k < len; k++)
                {
                    if (needle[k] != haystack[i + k]) break;
                }
                if (k == len)
                {
                    insertIndex = i;
                    break;
                }
            }

            
            return insertIndex;

        }
        #region DEV_GARBAGE


        #region not sure if we are handling constructors yet

        //switch (methodInfo.MemberType)
        //      {
        //          case MemberTypes.Constructor:
        //              if ((Convert.ToInt32(membersToPreserve) &
        //                   Convert.ToInt32(Classifier.MembersToPreserve.Constructor)) ==
        //                  (Convert.ToInt32(membersToPreserve)))
        //              {
        //                  // need to similar but specialized replacement as Method

        //              }
        //              break;

        #endregion





        #endregion

        #region Fields and properties


        public InjectionHelper.Status CurrentStatus { get; private set; }
        private static Injector _singleInjector;

        #endregion

        #region constructor(s)

        private Injector()
            : this(false)
        {
        }

        private Injector(bool showOutputInWindow)
        {
            // I don't like the UI as dependency to the dll, but then again this is a unit test dll so it should be OK
            // Should introduce UI separately (perhaps the UI itself is the parameter on the constructor, that's it!!)
            // like this
            // public Injector(UIObject debugWindow) -- next version :)


            InjectionHelper.Initialize(showOutputInWindow);
            CurrentStatus = InjectionHelper.Status.Uninitialized;
            var errorCountAllowed = 100; // TODO: need to fix threading to actaully wait on a callback
            InjectionHelper.Log("Initializing Injector.");

            try
            {
                Thread.Sleep(1500); // TODO: need to fix threading to actaully wait on a callback

                do
                {
                    CurrentStatus = InjectionHelper.GetStatus();
                    InjectionHelper.Log("Checking status.");
                    if (CurrentStatus.ToString().ToUpper().Contains("ERROR"))
                    {
                        Log(string.Format("Injector Initialization Error or Delay: {0} {1} Retries remaining ",
                            new object[] { CurrentStatus.ToString(), errorCountAllowed }));
                        if (errorCountAllowed-- <= 0)
                            break;
                    }
                } while (CurrentStatus != InjectionHelper.Status.Ready);

                if (CurrentStatus == InjectionHelper.Status.Ready)
                    IniALL.Ini();
            }
            catch (Exception e)
            {
                InjectionHelper.Log("Error during Injector Intialization :" + e.Message);
            }
        }

        #endregion

        #region utils and cleaup

        internal IList<ILInstruction> GetInstructionCode(MethodInfo methonInfo)
        {
            var m = new MethodBodyReader(methonInfo);
            return m.instructions;
        }
        private void OutputIL(string className, string methodName, byte[] results)
        {
            Debug.WriteLine(className + "." + methodName);
            Debug.WriteLine("===========================");
            // output in debug mode
            foreach (var b in results) { Debug.WriteLine(b); }


        }

        private void OutputMethodData(MethodInfo sourceMethod)
        {
            var ilCode = new StringBuilder();
            var iLCodeList = GetInstructionCode(sourceMethod);
            Log("MethodInfo for " + sourceMethod.Name);
            foreach (var instruction in iLCodeList)
                Log(instruction.GetCode());
        }


        internal void Log(string message)
        {
            InjectionHelper.Log(message);
        }

        public void Dispose()
        {
            //if (InjectionHelper.IsUIActive)
            //    InjectionHelper.Dispose();
        }

        #endregion

     

       
    }

    #region Classifier

    public class Classifier
    {
        public enum MembersToPreserve
        {
            Constructor = 1,
            Destructor = 2
        }

        public Type Type { get; set; }
        public string MethodName { get; set; }
        public BindingFlags BindingsFlags { get; set; }

        [Description("Not yet implemented")]
        public string FieldName { get; set; }
    }

    #endregion
}