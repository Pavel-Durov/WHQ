using System;
using System.Collections.Generic;
using System.Linq;
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
                await Task.Run(() => { CatchMutex(); });
                try
                {
                    if (!mutex.WaitOne(TimeSpan.FromSeconds(60), false))
                    {
                        Console.WriteLine("Someone got the mutex before me");
                    }
                    else
                    {
                        Console.WriteLine($"thread {Thread.CurrentThread.ManagedThreadId} aquires mutex");
                    }
                }
                catch (Exception e)
                {
                   //..
                }
                finally
                {
                    Console.WriteLine($"thread {Thread.CurrentThread.ManagedThreadId} simulates work for 10 minutes");

                    //some work simulation
                    await Task.Delay(TimeSpan.FromMinutes(10));

                    Console.WriteLine($"thread {Thread.CurrentThread.ManagedThreadId} releases the mutex");
                    mutex.ReleaseMutex();
                }
            }
        }

        private static void CatchMutex()
        {
            Thread workerThread = new Thread(ThreadWork);
            workerThread.Start();
        }

        private static void ThreadWork()
        {
            Thread.Sleep(3000);

            Mutex mutex = null;
            if (Mutex.TryOpenExisting(mutex_id, out mutex))
            {
                try
                {
                    var timeout = (uint)TimeSpan.FromHours(1).TotalMilliseconds;
                    var handle = mutex.SafeWaitHandle.DangerousGetHandle();

                    
                    Console.WriteLine($"thread {Thread.CurrentThread.ManagedThreadId} waits for mutex with timout of {timeout}");

                    Kernel32Calls.WaitForSingleObject(handle, timeout);

                    Console.WriteLine($"thread {Thread.CurrentThread.ManagedThreadId} released from mutex");
                }
                catch (Exception e)
                {

                }
            }
        }
    }
}