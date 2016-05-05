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
            CriticalSectionCalls();
            //WaitCalls();
        }

        static CRITICAL_SECTION section;
        private static void CriticalSectionCalls()
        {
            //Console.WriteLine(" - Kernel32Calls CRITICAL_SECTION ");

            InitializeCriticalSection(out section);

            EnterCriticalSection(ref section);

            LeaveCriticalSection(ref section);
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
        public static extern void InitializeCriticalSection(out CRITICAL_SECTION
    lpCriticalSection);

        [DllImport("kernel32.dll")]
        public static extern void EnterCriticalSection(ref CRITICAL_SECTION
   lpCriticalSection);

        // LEAVE CRITICAL SECTION
        [DllImport("kernel32.dll")]
        public static extern void LeaveCriticalSection(ref CRITICAL_SECTION
           lpCriticalSection);

        public struct CRITICAL_SECTION { /*int dummyMember; */}


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
}
