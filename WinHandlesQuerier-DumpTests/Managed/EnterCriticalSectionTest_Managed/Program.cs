using Kernel32;
using System;
using System.Diagnostics;
using System.Threading;
using WinBase;

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

      
    }
}
