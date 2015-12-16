using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_3
{
    class Program
    {

        const string SOME_86_DUMP = @"_C:\temp\dumps\Assignment_3.dmp";
        const int ATTACH_TO_PPROCESS_TIMEOUT = int.MaxValue;
        static void Main(string[] args)
        {
            ThreadStackHandler handler = new ThreadStackHandler();

            DataTarget target = null;
            if (File.Exists(SOME_86_DUMP))
            {
                //Loading a crash dump
                target = DataTarget.LoadCrashDump(SOME_86_DUMP);

            }
            else
            {
                Console.WriteLine("--- Assignment_3 ----");
                Console.WriteLine("Please enter a PID: ");


                var pid = int.Parse(Console.ReadLine());

                target = DataTarget.AttachToProcess(pid, ATTACH_TO_PPROCESS_TIMEOUT);
            }



            var runtime = target.ClrVersions[0].CreateRuntime();

            foreach (ClrThread thread in runtime.Threads)
            {
                handler.Handle(target.DebuggerInterface, thread, runtime);
                break;
            }

            Console.ReadKey();
        }
    }

}
