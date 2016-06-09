using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DumpTest.Tests
{
    class ThreadEventWait
    {
        static EventWaitHandle _eventWaitHandle;

        public static void Run()
        {
            _eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);


            Thread tWaiter = new Thread(SomeWork);

            Thread tRaises = new Thread(async () =>
            {
                Console.WriteLine($"thread {Thread.CurrentThread.ManagedThreadId} Simulates 1 hour work");
                await Task.Delay(TimeSpan.FromHours(1));
                _eventWaitHandle.Set();
            });

            tRaises.Start();
            tWaiter.Start();


        }

        private static void SomeWork()
        {
            Console.WriteLine($"thread {Thread.CurrentThread.ManagedThreadId} waits for sahred EventWaitHandle");

            _eventWaitHandle.WaitOne();

            Console.WriteLine($"thread {Thread.CurrentThread.ManagedThreadId} released got EventWaitHandle signal (RELEASED)");
        }
    }
}

