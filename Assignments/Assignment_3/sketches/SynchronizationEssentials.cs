using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment_3.Scaches
{
    //Article http://www.albahari.com/threading/part2.aspx

    class SynchronizationEssentials
    {
        volatile bool _complete;
        public SynchronizationEssentials()
        {
            doStuff();
        }

        private void doStuff()
        {
            Thread t = new Thread(new ThreadStart(SomeAction));
            t.Start();

            var threadState = t.ThreadState;
            t.Abort();
        }

        private void SomeAction()
        {
            //DO something
        }
    }

    class ThreadUnsafe
    {
        static int _val1 = 1, _val2 = 1;

        static void Go()
        {
            if (_val2 != 0)
            {
                Console.WriteLine(_val1 / _val2);
            }

            _val2 = 0;
        }
    }

    class ThreadSafe
    {
        static readonly object _locker = new object();
        static int _val1, _val2;

        static void Go()
        {
            lock (_locker)
            {
                if (_val2 != 0) Console.WriteLine(_val1 / _val2);
                _val2 = 0;
            }
        }
    }

    class ThreadSafeWithMonitor
    {
        static readonly object _locker = new object();
        static int _val1, _val2;

        static void Go()
        {
            bool lockTaken = false;
            Monitor.Enter(_locker, ref lockTaken);
            try
            {
                if (_val2 != 0) Console.WriteLine(_val1 / _val2);
                _val2 = 0;
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(_locker);
                }
            }
        }
    }
}
