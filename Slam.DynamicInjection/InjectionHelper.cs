using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using System.Reflection.Emit;



public static class InjectionHelper
{
    public enum Status : int
    {
        Ready = 1,
        Uninitialized = 0,
        Error_HookCompileMethodFailed = -1,
        Error_LoadedMethodDescIteratorInitializationFailed = -2,
        Error_MethodDescInitializationFailed = -3,
        Error_DbgHelpNotFound = -4,
        Error_JITNotFound = -5,
        Error_DownloadPDBFailed = -6,
        Error_CLRNotFound = -7,
    }


    #region P/Invoke
    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr LoadLibraryW(string lpFileName);

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    private static extern bool FreeLibrary(IntPtr hModule);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
    #endregion
    

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate bool UpdateILCodesDelegate(IntPtr pMethodTable, IntPtr pMethodHandle, int md, IntPtr pBuffer, int dwSize);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate Status GetStatusDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate Status WaitForIntializationCompletionDelegate();

    private static IntPtr _moduleHandle;
    private static UpdateILCodesDelegate _updateILCodesMethod;
    private static GetStatusDelegate _getStatusDelegate;
    private static WaitForIntializationCompletionDelegate _waitForIntializationCompletionDelegate;


 

    public static void Initialize(bool showLoggingUI = false)
    {
        //if (showLoggingUI)
        //{
        //    _loggingForm = new LoggingForm();
        //    _loggingForm.Show();
        //}
        string currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        currentDir = Regex.Replace(currentDir, @"^(file\:\\)", string.Empty);

        // Environment.Is64BitProcess
        string path = Path.Combine(currentDir, (IntPtr.Size == 8) ? "Injection64.dll" : "Injection32.dll");

#if DEBUG
        
        Log("The injection.dll is only designed for .Net release mode process. it is not supposed to be used for debug mode.");
#endif

        _moduleHandle = LoadLibraryW(path);
        if (_moduleHandle == IntPtr.Zero)
        {
            path = path.Replace("\\bin", "\\bin\\x86");
            // try x86
            _moduleHandle = LoadLibraryW(path);
             if (_moduleHandle == IntPtr.Zero)
                 throw new FileNotFoundException(string.Format("Failed to load [{0}]", path));
        }
           

        IntPtr ptr = GetProcAddress(_moduleHandle, "UpdateILCodes");
        if (ptr == IntPtr.Zero)
            throw new MethodAccessException("Failed to locate UpdateILCodes function!");
        _updateILCodesMethod = (UpdateILCodesDelegate)Marshal.GetDelegateForFunctionPointer(ptr, typeof(UpdateILCodesDelegate));


        ptr = GetProcAddress(_moduleHandle, "GetStatus");
        if (ptr == IntPtr.Zero)
            throw new MethodAccessException("Failed to locate GetStatus function!");
        _getStatusDelegate = (GetStatusDelegate)Marshal.GetDelegateForFunctionPointer(ptr, typeof(GetStatusDelegate));



        ptr = GetProcAddress(_moduleHandle, "WaitForIntializationCompletion");
        if (ptr == IntPtr.Zero)
            throw new MethodAccessException("Failed to locate WaitForIntializationCompletion function!");
        _waitForIntializationCompletionDelegate = (WaitForIntializationCompletionDelegate)Marshal.GetDelegateForFunctionPointer(ptr, typeof(WaitForIntializationCompletionDelegate));
    }

    public static void Uninitialize()
    {
        if (_moduleHandle != IntPtr.Zero)
        {
            FreeLibrary(_moduleHandle);
            _moduleHandle = IntPtr.Zero;
        }
    }

    public static void UpdateILCodes(MethodBase method, byte[] ilCodes)
    {
        if (_updateILCodesMethod == null)
            throw new Exception("Please Initialize() first.");

        IntPtr pMethodTable = IntPtr.Zero;
        if (method.DeclaringType != null)
            pMethodTable = method.DeclaringType.TypeHandle.Value;

        IntPtr pMethodHandle = IntPtr.Zero;
        if (method is DynamicMethod)
        {
            pMethodHandle = GetDynamicMethodHandle(method);
        }
        else
        {
            pMethodHandle = method.MethodHandle.Value;
        }

        IntPtr pBuffer = Marshal.AllocHGlobal(ilCodes.Length);
        if (pBuffer == IntPtr.Zero)
            throw new OutOfMemoryException();

        Marshal.Copy(ilCodes, 0, pBuffer, ilCodes.Length);

        int token = 0;
        try
        {
            token = method.MetadataToken;
        }
        catch
        {        	
        }


        if (!_updateILCodesMethod(pMethodTable, pMethodHandle, token, pBuffer, ilCodes.Length))
            throw new Exception("UpdateILCodes() failed, please check the initialization is failed or uncompleted.");
    }

    public static Status WaitForIntializationCompletion()
    {
        return _waitForIntializationCompletionDelegate();
    }

    public static Status GetStatus()
    {
        return _getStatusDelegate();
    }

    private static IntPtr GetDynamicMethodHandle(MethodBase method)
    {
        // .Net 4.0
        {
            FieldInfo fieldInfo = typeof(DynamicMethod).GetField("m_methodHandle", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null)
            {
                object runtimeMethodInfoStub = fieldInfo.GetValue(method);
                if (runtimeMethodInfoStub != null)
                {
                    fieldInfo = runtimeMethodInfoStub.GetType().GetField("m_value", BindingFlags.Instance | BindingFlags.Public);
                    if (fieldInfo != null)
                    {
                        object internalRuntimeMethodHandle = fieldInfo.GetValue(runtimeMethodInfoStub);
                        if (internalRuntimeMethodHandle != null)
                        {
                            fieldInfo = internalRuntimeMethodHandle.GetType().GetField("m_handle", BindingFlags.NonPublic | BindingFlags.Instance);
                            if (fieldInfo != null)
                            {
                                return (IntPtr)fieldInfo.GetValue(internalRuntimeMethodHandle);
                            }
                        }
                    }
                }
            }
        }
        // .Net 2.0
        {
            FieldInfo fieldInfo = typeof(DynamicMethod).GetField("m_method", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null)
            {
                return ((RuntimeMethodHandle)fieldInfo.GetValue(method)).Value;
            }            
        }

        
        return IntPtr.Zero;
    }


    public static void Log(string message)
    {
        System.Diagnostics.Debug.WriteLine(message);
        using (StreamWriter sw = File.AppendText(@"C:\Users\Patrick\Documents\GitHub\bin\results.txt"))
        {
            sw.WriteLine(message);
        }
  
    }



    internal static void Dispose()
    {
        //if (_loggingForm != null)
        //{
        //    _loggingForm.Hide();
        //    _loggingForm= null;
        //}
    }

    public static MethodInfo GetSealedMethod(string AssemblyeNameWithPath, string FullClassName, string MethodName, object[] MethodParameters)
    {
        object result = new object();
        return   GetSealedMethod(AssemblyeNameWithPath,FullClassName,MethodName,MethodParameters,false,ref result);

    }
    /// <summary>
    /// 
    /// 
    /// </summary>
    /// <param name="AssemblyeNameWithPath">e.g.  @"D:\inetpub\Common\Slam.Core.dll"</param>
    /// <param name="FullClassName">e.g.  "Slam.Integration.Salesforce.MyFactoryFactory"</param>
    /// <param name="MethodName">e.g.  "MySealedFunction"</param>
    /// <param name="MethodParameters">e.g.  new object[] { true, new ETPrincipal(1) };</param>
    /// <returns></returns>
    public static MethodInfo GetSealedMethod(string AssemblyeNameWithPath, string FullClassName, string MethodName, object[] MethodParameters,bool InvokeMethod , ref object Result  )
    {

        Assembly coreAssembly = Assembly.LoadFile(AssemblyeNameWithPath);
        Type coreType = coreAssembly.GetType(FullClassName);

        var methods = coreType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static);

        MethodInfo methodInfo = null;
        foreach (var m in methods.Where(m => m.Name == MethodName && m.GetParameters().Length == MethodParameters.Length))
        {
            if (MethodParameters.Length == 0)
            {
                // nothing to compare just set the methodInfo and break
                methodInfo = m;
            }
            else
            {
                var p = m.GetParameters();
                bool parametersMatch = true;
                for (var i = 0; i < p.Length; i++) // no for reach order not garunteed
                {
                    if (p[i].ParameterType.ToString() != MethodParameters[i].GetType().ToString())
                    {
                        parametersMatch = false;
                        break;
                    }
                    
                }
                if (parametersMatch )
                {
                    methodInfo = m;
                    break;
                }
            }
        }

        if (methodInfo != null && InvokeMethod)
        {
            Result = methodInfo.Invoke(null, MethodParameters);
        }
        return methodInfo;
    }

}

