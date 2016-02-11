
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

namespace Consumer
{
    class Program
    {
        const string SOME_86_DUMP = @"C:\temp\dumps\DumpTest.dmp";
        const int PID_NOT_FOUND = 0;
        const int ATTACH_TO_PPROCESS_TIMEOUT = 999999;

        static void Main(string[] args)
        {
            //Global.LiveProcessTest.Run();
            Global.DumpFileTest.Run();
            //Global.Test.Run(pid);
            //umpFile64Bit.Test.Run(pid);
            Console.ReadKey();
        }

        private static int GetPidFromDumpProcessTextFile()
        {
            int pid = PID_NOT_FOUND;

            var fileContent = File.ReadAllText(@"./../../../dump_pid.txt");
            if (!String.IsNullOrEmpty(fileContent))
            {
                var success = int.TryParse(fileContent, out pid);
            }

            return pid;
        }

      
    }
}
