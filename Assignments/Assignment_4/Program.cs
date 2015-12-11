//#define DUMP_AS_SOURCE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core;
using System.IO;
using Assignments.Core.Handlers;
using Microsoft.Diagnostics.Runtime;

//TODO: post build -> start ../../../Assignment_3.DumpTest\bin\x86\Debug\Assignment_3.DumpTest.exe

namespace Assignment_4
{
    class Program
    {
        const string SOME_86_DUMP = @"C:\temp\Dumps\Assignment_3.dmp";

        const int ATTACH_TO_PPROCESS_TIMEOUT = 999999;

        static void Main(string[] args)
        {

#if DUMP_AS_SOURCE
            if (File.Exists(SOME_86_DUMP))
            {
                //Loading a crash dump
                using (DataTarget target = DataTarget.LoadCrashDump(SOME_86_DUMP))
                {
                    DoAnaytics(target);
                }

            }
#else
            
            Console.WriteLine("Please enter a PID: ");

            var pid = int.Parse(Console.ReadLine());
            using (DataTarget target = DataTarget.AttachToProcess(pid, ATTACH_TO_PPROCESS_TIMEOUT))
            {
                DoAnaytics(target);
            }

#endif
            Console.ReadKey();
        }


        private static void DoAnaytics(DataTarget target)
        {
            ThreadStackHandler handler = new ThreadStackHandler();
            var runtime = target.ClrVersions[0].CreateRuntime();

            foreach (ClrThread thread in runtime.Threads)
            {
                handler.Handle(target.DebuggerInterface, thread, runtime);
                break;
            }
        }
    }
}
