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
        const string mutex_id = "{B1E7934A-F688-417f-8FCB-65C3985E9E27}";

        public static async Task Run()
        {
            await Task.Run(() => { CatchMutex(); });

            Mutex mutex;
            Mutex.TryOpenExisting(mutex_id, out mutex);
            try
            {
                try
                {
                    if (!mutex.WaitOne())
                    {
                        Console.WriteLine("Another instance of this program is running");
                        Environment.Exit(0);
                    }
                }
                catch (AbandonedMutexException)
                {

                }
            }
            finally
            {
                mutex.ReleaseMutex();
            }


        }

        private static void CatchMutex()
        {
            var mutex = new Mutex(false, mutex_id);
            

        }
    }
}
