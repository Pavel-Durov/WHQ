using Kernel32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinBase;

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

        private static async void SingleWaitCalls()
        {
            await Task.Run(() =>
           {
               ManualResetEvent even = new ManualResetEvent(false);
               var handle = even.SafeWaitHandle.DangerousGetHandle();
               Console.WriteLine("handle : 0x{0}, Int : {1}", handle.ToString("X"), handle);
               var mulRes2 = Functions.WaitForSingleObject(handle, int.MaxValue);
           });
        }

        public static CRITICAL_SECTION section = new CRITICAL_SECTION();
        private static async void CriticalSectionCalls()
        {
            await Task.Run(async() =>
            {
                Console.WriteLine();
                Functions.InitializeCriticalSection(out section);
                Console.WriteLine($"InitializeCriticalSection id: {Thread.CurrentThread.ManagedThreadId} ");
                Functions.EnterCriticalSection(ref section);
                await Task.Delay(int.MaxValue);
            });
        }

        private static async void MultiWaitCalls()
        {
            await Task.Run(() =>
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
            });

        }

        public const Int32 INFINITE = -1;
        public const Int32 WAIT_ABANDONED = 0x80;
        public const Int32 WAIT_OBJECT_0 = 0x00;
        public const Int32 WAIT_TIMEOUT = 0x102;
        public const Int32 WAIT_FAILED = -1;
    }


}
