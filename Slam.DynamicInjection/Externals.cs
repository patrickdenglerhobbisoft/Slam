using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Slam.Core
{
    public class Externals
    {
             [DllImport("kernel32.dll")]
        private static extern bool QueryFullProcessImageName(IntPtr hprocess, int dwFlags,
                       StringBuilder lpExeName, out int size);
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess,
                       bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hHandle);

        [DllImport("kernel32.dll")]
       static extern bool QueryInformationJobObject(IntPtr hJob, JOBOBJECTINFOCLASS
                JobObjectInformationClass, IntPtr lpJobObjectInfo,
                  uint cbJobObjectInfoLength, IntPtr lpReturnLength);

       

//        BOOL WINAPI QueryInformationJobObject(
//  _In_opt_  HANDLE             hJob,
//  _In_      JOBOBJECTINFOCLASS JobObjectInfoClass,
//  _Out_     LPVOID             lpJobObjectInfo,
//  _In_      DWORD              cbJobObjectInfoLength,
//  _Out_opt_ LPDWORD            lpReturnLength
//);

         [DllImport( "kernel32.dll" , EntryPoint = "CreateJobObjectW" , CharSet = CharSet.Unicode )]
        public static extern IntPtr CreateJobObject( SecurityAttributes JobAttributes , string lpName );


         [Flags]
         public enum ProcessAccessFlags : uint
         {
             All = 0x001F0FFF,
             Terminate = 0x00000001,
             CreateThread = 0x00000002,
             VirtualMemoryOperation = 0x00000008,
             VirtualMemoryRead = 0x00000010,
             DuplicateHandle = 0x00000040,
             CreateProcess = 0x000000080,
             SetQuota = 0x00000100,
             SetInformation = 0x00000200,
             QueryInformation = 0x00000400,
             QueryLimitedInformation = 0x00001000,
             Synchronize = 0x00100000
         }
        public class SecurityAttributes
        {

            public int nLength; //Useless field = 0
            public IntPtr pSecurityDescriptor; //хз))
            public bool bInheritHandle; //Возможность наследования

            public SecurityAttributes()
            {
                this.bInheritHandle = true;
                this.nLength = 0;
                this.pSecurityDescriptor = IntPtr.Zero;
            }
        }

        [DllImport( "kernel32.dll" )]
        static extern bool SetInformationJobObject( IntPtr hJob , JOBOBJECTINFOCLASS JobObjectInfoClass , IntPtr lpJobObjectInfo , uint cbJobObjectInfoLength );

        public enum JOBOBJECTINFOCLASS
        {
            JobObjectAssociateCompletionPortInformation = 7 ,
            JobObjectBasicLimitInformation = 2 ,
            JobObjectBasicUIRestrictions = 4 ,
            JobObjectEndOfJobTimeInformation = 6 ,
            JobObjectExtendedLimitInformation = 9 ,
            JobObjectSecurityLimitInformation = 5
        }


        [StructLayout( LayoutKind.Sequential )]
        struct JOBOBJECT_BASIC_LIMIT_INFORMATION
        {
            public Int64 PerProcessUserTimeLimit;
            public Int64 PerJobUserTimeLimit;
            public Int16 LimitFlags;
            public UIntPtr MinimumWorkingSetSize;
            public UIntPtr MaximumWorkingSetSize;
            public Int16 ActiveProcessLimit;
            public Int64 Affinity;
            public Int16 PriorityClass;
            public Int16 SchedulingClass;
        }

        public enum LimitFlags
        {
            JOB_OBJECT_LIMIT_ACTIVE_PROCESS = 0x00000008 ,
            JOB_OBJECT_LIMIT_AFFINITY = 0x00000010 ,
            JOB_OBJECT_LIMIT_BREAKAWAY_OK = 0x00000800 ,
            JOB_OBJECT_LIMIT_DIE_ON_UNHANDLED_EXCEPTION = 0x00000400 ,
            JOB_OBJECT_LIMIT_JOB_MEMORY = 0x00000200 ,
            JOB_OBJECT_LIMIT_JOB_TIME = 0x00000004 ,
            JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 0x00002000 ,
            JOB_OBJECT_LIMIT_PRESERVE_JOB_TIME = 0x00000040 ,
            JOB_OBJECT_LIMIT_PRIORITY_CLASS = 0x00000020 ,
            JOB_OBJECT_LIMIT_PROCESS_MEMORY = 0x00000100 ,
            JOB_OBJECT_LIMIT_PROCESS_TIME = 0x00000002 ,
            JOB_OBJECT_LIMIT_SCHEDULING_CLASS = 0x00000080 ,
            JOB_OBJECT_LIMIT_SILENT_BREAKAWAY_OK = 0x00001000 ,
            JOB_OBJECT_LIMIT_WORKINGSET = 0x00000001
        }


        [DllImport( "kernel32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        static extern bool AssignProcessToJobObject( IntPtr hJob , IntPtr hProcess );

        [StructLayout( LayoutKind.Sequential )]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }
    }
}
