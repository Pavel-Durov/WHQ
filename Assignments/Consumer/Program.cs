
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
        const int PID_NOT_FOUND = 0;
        const int ATTACH_TO_PPROCESS_TIMEOUT = 999999;

        static void Main(string[] args)
        {

            if(args != null && args.Length == 2 && !String.IsNullOrEmpty(args[0]))
            {
                if (args[0] == "-dump" && !String.IsNullOrEmpty(args[1]))
                {
                    using (DataTarget target = DataTarget.LoadCrashDump(args[1]))
                    {
                        Global.DumpFile.DoAnaytics(target, args[1]);
                    }
                }
                else if(args[0] == "-pid")
                {
                    int parsed = 0;
                    if(int.TryParse(args[0], out parsed))
                    {
                        Global.LiveProcess.Run(parsed);
                    }
                }
            }


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
