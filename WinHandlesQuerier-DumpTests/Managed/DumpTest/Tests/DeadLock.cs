using System;
using System.Threading;

namespace DumpTest.Tests
{
    public class DeadLock
    {
        static object _sync = new object();
        static object _sync2 = new object();

        static ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

        public static void Run()
        {
            Console.WriteLine("Running DeadLock");

            Thread another = new Thread(() =>
            {
                lock (_sync2)
                {
                    _manualResetEvent.Set();
                    lock (_sync) { }
                }
            });

            lock (_sync)
            {
                another.Start();
                _manualResetEvent.WaitOne();
                lock (_sync2) { }
            }
        }
    }
}
