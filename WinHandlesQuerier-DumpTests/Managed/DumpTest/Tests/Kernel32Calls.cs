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
        private static void CriticalSectionCalls()
        {
            //Simulating DeadLock
            new Thread(() =>
            {
                Console.WriteLine();
                Functions.InitializeCriticalSection(out section);
                Console.WriteLine($"EnterCriticalSection id: {Thread.CurrentThread.ManagedThreadId} ");
                Functions.EnterCriticalSection(ref section);

                var inner = new Thread(() =>
                {
                    Console.WriteLine("--DEAD LOCK--");
                    Console.WriteLine($"EnterCriticalSection id: {Thread.CurrentThread.ManagedThreadId} ");
                    Functions.EnterCriticalSection(ref section);

                    Thread.Sleep(2000);
                    Console.WriteLine($"LeaveCriticalSection id: {Thread.CurrentThread.ManagedThreadId} ");
                    Functions.LeaveCriticalSection(ref section);
                });
                inner.Start();
                inner.Join();

                Console.WriteLine($"LeaveCriticalSection id: {Thread.CurrentThread.ManagedThreadId} ");
                Functions.LeaveCriticalSection(ref section);
            }).Start();
        }

        private static async void MultiWaitCalls()
        {
            await Task.Run(() =>
            {
                IntPtr[] arr = new IntPtr[19];
                for (int i = 0; i < arr.Length; i++)
                {
                    var loopAutoEvent = new AutoResetEvent(false);
                    arr[i] = loopAutoEvent.Handle;
                }

                var mulRes2 = Functions.WaitForMultipleObjects(19, arr, true, int.MaxValue);

                Console.WriteLine($"WaitForMultipleObjects - Count: 19, bWaitAll : true, wait: {int.MaxValue} / {int.MaxValue.ToString("X")}");

            });
        }
    }


}
