using Kernel32;
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
            MultiWaitCalls();
            SingleWaitCalls();
        }

        private static void SingleWaitCalls()
        {
            ManualResetEvent even = new ManualResetEvent(false);
            var mulRes2 = Functions.WaitForSingleObject(even.SafeWaitHandle.DangerousGetHandle(), int.MaxValue);
        }

        public static IntPtr section;

        private static async void CriticalSectionCalls()
        {

            Functions.InitializeCriticalSection(out section);
            Console.WriteLine($"InitializeCriticalSection id: {Task.CurrentId} ");
            Functions.EnterCriticalSection(ref section);
            await Task.Delay(int.MaxValue);
        }

        private static void MultiWaitCalls()
        {
            IntPtr[] arr = new IntPtr[3];
            for (int i = 0; i < 3; i++)
            {
                var loopAutoEvent = new AutoResetEvent(false);
                arr[i] = loopAutoEvent.Handle;
            }

            Console.WriteLine(" - Kernel32Calls WaitForMultipleObjects");
            var mulRes0 = Functions.WaitForMultipleObjects(3, arr, true, 0);
            var mulRes1 = Functions.WaitForMultipleObjects(3, arr, false, 0);
            var mulRes2 = Functions.WaitForMultipleObjects(3, arr, true, int.MaxValue);
           

        }

        public const Int32 INFINITE = -1;
        public const Int32 WAIT_ABANDONED = 0x80;
        public const Int32 WAIT_OBJECT_0 = 0x00;
        public const Int32 WAIT_TIMEOUT = 0x102;
        public const Int32 WAIT_FAILED = -1;
    }

   
}
