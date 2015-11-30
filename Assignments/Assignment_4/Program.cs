using Assignment_4.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core;
using System.IO;
using Assignments.Core.Handlers;
using Microsoft.Diagnostics.Runtime;

namespace Assignment_4
{
    class Program
    {
        const string SOME_86_DUMP = @"C:\temp\Dumps\Assignment_3.dmp";

        static void Main(string[] args)
        {
            ThreadStackHandler handler = new ThreadStackHandler();
            //WctApi api = new WctApi();

            //api.TestRun();

            //Console.ReadKey();


            if (File.Exists(SOME_86_DUMP))
            {
                //Loading a crash dump
                using (DataTarget target = DataTarget.LoadCrashDump(SOME_86_DUMP))
                {
                    var runtime = target.ClrVersions[0].CreateRuntime();

                    foreach (ClrThread thread in runtime.Threads)
                    {

                        handler.Handle(target.DebuggerInterface, thread, runtime);
                        break;
                    }
                }

            }

            Console.ReadKey();
        }

    
    }
}
