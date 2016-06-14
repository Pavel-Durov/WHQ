using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace EnterCriticalSectionTest_Managed
{
    class Program
    {
        public static CRITICAL_SECTION section = new CRITICAL_SECTION();

        static void Main(string[] args)
        {
            Console.WriteLine("Test started");
            Console.WriteLine($"Is64BitProcess : {Environment.Is64BitProcess}{Environment.NewLine}");

            var proc = Process.GetCurrentProcess();
            Console.WriteLine("PID : " + proc.Id);


            //Simulating DeadLock
            new Thread(() =>
            {
                Console.WriteLine();
                InitializeCriticalSection(out section);
                Console.WriteLine($"EnterCriticalSection id: {Thread.CurrentThread.ManagedThreadId} ");
                EnterCriticalSection(ref section);

                var inner = new Thread(() =>
                {
                    Console.WriteLine("--DEAD LOCK--");
                    Console.WriteLine($"EnterCriticalSection id: {Thread.CurrentThread.ManagedThreadId} ");
                    EnterCriticalSection(ref section);

                    Thread.Sleep(2000);
                    Console.WriteLine($"LeaveCriticalSection id: {Thread.CurrentThread.ManagedThreadId} ");
                    LeaveCriticalSection(ref section);
                });
                inner.Start();
                inner.Join();

                Console.WriteLine($"LeaveCriticalSection id: {Thread.CurrentThread.ManagedThreadId} ");
                LeaveCriticalSection(ref section);
            }).Start();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void EnterCriticalSection(ref CRITICAL_SECTION lpCriticalSection);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void InitializeCriticalSection(out CRITICAL_SECTION lpCriticalSection);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void LeaveCriticalSection(ref CRITICAL_SECTION lpCriticalSection);

        [StructLayout(LayoutKind.Sequential)]
        public struct CRITICAL_SECTION
        {
            public Int64 LockCount;
            public Int64 RecursionCount;
            public IntPtr OwningThread;        // from the thread's ClientId->UniqueThread
            public IntPtr LockSemaphore;
            public UIntPtr SpinCount;        // force size on 64-bit systems when packed
        };
    }
}
