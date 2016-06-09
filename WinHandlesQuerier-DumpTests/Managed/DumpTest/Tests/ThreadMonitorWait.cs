using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DumpTest.Tests
{
    class ThreadMonitorWait
    {
        static object _lockSync = new object();

        public static void Run()
        {

            Thread tWaiter = new Thread(async ()=> {

                Console.WriteLine($"thread {Thread.CurrentThread.ManagedThreadId} aquires lock");

                Monitor.Enter(_lockSync);
                
                Console.WriteLine($"thread {Thread.CurrentThread.ManagedThreadId} SIMULATES WORK for 1 hour");
                await Task.Delay(TimeSpan.FromHours(1));

                Monitor.Exit(_lockSync);
            });

            Thread tRaises = new Thread(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(4));

                Console.WriteLine($"thread {Thread.CurrentThread.ManagedThreadId} tryies to aquire the lock");
                Monitor.Enter(_lockSync);
                
              
                await Task.Delay(TimeSpan.FromHours(1));

                Monitor.Exit(_lockSync);
            });

            tRaises.Start();
            tWaiter.Start();

        }
    }
}
