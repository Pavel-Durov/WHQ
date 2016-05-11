using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DumpTest.Tests
{
    class Kernel32Calls
    {
        public static void Run()
        {
            MutexCalls();
            CriticalSectionCalls();
            WaitCalls();
        }

        private static void MutexCalls()
        {
            SECURITY_ATTRIBUTES secAttribs = new SECURITY_ATTRIBUTES();
            secAttribs.nLength = Marshal.SizeOf(secAttribs);
            secAttribs.bInheritHandle = 1;

            var mutexAttributesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(secAttribs));
            var mutexName = "This-My-Mutex-Man";

            Console.WriteLine($"Creating Mutex with name : '{mutexName}'");

            var hMutex = CreateMutex(mutexAttributesPtr, true, mutexName);
            var lastError = Marshal.GetLastWin32Error();
            Console.WriteLine($"Win32 Error : {lastError}");

            // It's faster to first try OpenMutex
            IntPtr openMutex = OpenMutex((uint)(SyncObjectAccess.MUTEX_MODIFY_STATE), false, mutexName);

            var lastError2 = Marshal.GetLastWin32Error();
            Console.WriteLine($"Win32 Error : {lastError2}");

        }

        public static IntPtr section;
        private static async void CriticalSectionCalls()
        {
            //Console.WriteLine(" - Kernel32Calls IntPtr ");

            InitializeCriticalSection(out section);
            Console.WriteLine($"InitializeCriticalSection id: {Task.CurrentId} ");

            EnterCriticalSection(ref section);

            bool canenter = TryEnterCriticalSection(ref section);
            var onerror = Marshal.GetLastWin32Error();

            Console.WriteLine($"id: {Task.CurrentId}, can: {canenter }, error: {onerror }");


            await Task.Run(() =>
            {

                bool enter = TryEnterCriticalSection(ref section);
                LeaveCriticalSection(ref section);
                var error = Marshal.GetLastWin32Error();
                //Console.WriteLine($"id: {Task.CurrentId}, can: {enter}, error: {error}");

            });

            await Task.Run(() =>
            {
                bool enter = TryEnterCriticalSection(ref section);
                EnterCriticalSection(ref section);
                EnterCriticalSection(ref section);
                EnterCriticalSection(ref section);
                EnterCriticalSection(ref section);

                var error = Marshal.GetLastWin32Error();
                //Console.WriteLine($"id: {Task.CurrentId}, can: {enter}, error: {error}");
            });
          
        }

        private static void WaitCalls()
        {
            IntPtr[] arr = new IntPtr[3];
            for (int i = 0; i < 3; i++)
            {
                var loopAutoEvent = new AutoResetEvent(false);
                arr[i] = loopAutoEvent.Handle;
            }

            Console.WriteLine(" - Kernel32Calls WaitForMultipleObjects");
            var mulRes0 = WaitForMultipleObjects(3, arr, true, 0);
            var mulRes1 = WaitForMultipleObjects(3, arr, false, 0);
            var mulRes2 = WaitForMultipleObjects(3, arr, true, int.MaxValue);
        }
        [DllImport("kernel32.dll")]
        static extern IntPtr CreateMutex(IntPtr lpMutexAttributes, bool bInitialOwner,
   string lpName);


        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void InitializeCriticalSection(out IntPtr
    lpCriticalSection);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void EnterCriticalSection(ref IntPtr
   lpCriticalSection);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void LeaveCriticalSection(ref IntPtr
           lpCriticalSection);

        [DllImport("kernel32.dll")]
        static extern bool TryEnterCriticalSection(ref IntPtr
   lpCriticalSection);
        /// <summary>
        /// This value can be returned by CreateMutex() and is found in
        /// C++ in the error.h header file.
        /// </summary>
        public const int ERROR_ALREADY_EXISTS = 183;
        public const uint SYNCHRONIZE = 0x00100000;

        [DllImport("kernel32.dll")]
        public static extern bool ReleaseMutex(IntPtr hMutex);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenMutex(uint dwDesiredAccess, bool bInheritHandle, string lpName);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool SetSecurityDescriptorDacl(ref SECURITY_DESCRIPTOR securityDescriptor, bool daclPresent, IntPtr dacl, bool daclDefaulted);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool InitializeSecurityDescriptor(out SECURITY_DESCRIPTOR securityDescriptor, uint dwRevision);



        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool Beep(uint dwFreq, uint dwDuration);


        [DllImport("kernel32.dll")]
        static extern uint WaitForMultipleObjects(uint nCount, IntPtr[] lpHandles, bool bWaitAll, uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, [In, MarshalAs(UnmanagedType.Bool)] bool bManualReset, [In, MarshalAs(UnmanagedType.Bool)] bool bIntialState, [In, MarshalAs(UnmanagedType.BStr)] string lpName);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Int32 WaitForSingleObject(IntPtr Handle, uint Wait);

        public const Int32 INFINITE = -1;
        public const Int32 WAIT_ABANDONED = 0x80;
        public const Int32 WAIT_OBJECT_0 = 0x00;
        public const Int32 WAIT_TIMEOUT = 0x102;
        public const Int32 WAIT_FAILED = -1;
    }

    /// <summary>
    /// Security enumeration from:
    /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dllproc/base/synchronization_object_security_and_access_rights.asp
    /// </summary>
    [Flags]
    public enum SyncObjectAccess : uint
    {
        DELETE = 0x00010000,
        READ_CONTROL = 0x00020000,
        WRITE_DAC = 0x00040000,
        WRITE_OWNER = 0x00080000,
        SYNCHRONIZE = 0x00100000,
        EVENT_ALL_ACCESS = 0x001F0003,
        EVENT_MODIFY_STATE = 0x00000002,
        MUTEX_ALL_ACCESS = 0x001F0001,
        MUTEX_MODIFY_STATE = 0x00000001,
        SEMAPHORE_ALL_ACCESS = 0x001F0003,
        SEMAPHORE_MODIFY_STATE = 0x00000002,
        TIMER_ALL_ACCESS = 0x001F0003,
        TIMER_MODIFY_STATE = 0x00000002,
        TIMER_QUERY_STATE = 0x00000001
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct SECURITY_DESCRIPTOR
    {
        public byte revision;
        public byte size;
        public short control;
        public IntPtr owner;
        public IntPtr group;
        public IntPtr sacl;
        public IntPtr dacl;
    }
}
