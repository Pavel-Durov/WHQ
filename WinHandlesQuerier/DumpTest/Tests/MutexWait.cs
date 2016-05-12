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
    class MutexWait
    {
        const string mutex_id = "this-is-the-most-unique-id-ever-8947F";

        public static async Task Run()
        {
            using (var mutex = new Mutex(false, mutex_id))
            {
                try
                {
                    if (!mutex.WaitOne(TimeSpan.FromSeconds(60), false))
                    {
                        Console.WriteLine("Someone got the mutex before me");
                    }
                    else
                    {
                        Console.WriteLine($"thread {Thread.CurrentThread.ManagedThreadId} aquires mutex handle:{mutex.Handle}");
                    }
                }
                catch (Exception e)
                {
                   //..
                }
                finally
                {
                    Thread t = new Thread(ThreadWork);
                    t.Start();

                    Console.WriteLine($"thread {Thread.CurrentThread.ManagedThreadId} simulates work for 1 hour");

                    //some work simulation
                    await Task.Delay(TimeSpan.FromHours(10));

                    //Console.WriteLine($"thread {Thread.CurrentThread.ManagedThreadId} releases the mutex");
                   // mutex.ReleaseMutex();
                }
            }
        }


        private static void ThreadWork()
        {
            Mutex mutex = null;
            
            if (Mutex.TryOpenExisting(mutex_id, out mutex))
            {
                try
                {
                    var timeout = Kernel32.Const.INFINITE;
                    var handle = mutex.SafeWaitHandle.DangerousGetHandle();

                    
                    Console.WriteLine($"thread {Thread.CurrentThread.ManagedThreadId} waits for mutex with timout of {timeout}, hanlde :{mutex.Handle}");

                    Functions.WaitForSingleObject(handle, timeout);

                    Console.WriteLine($"thread {Thread.CurrentThread.ManagedThreadId} Donw waiting for mutex");
                    
                }
                catch (Exception e)
                {

                }
            }
        }
    }
}