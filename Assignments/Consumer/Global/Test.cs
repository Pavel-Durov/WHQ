using Assignments.Core.Handlers;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer.Global
{
    class Test
    {
        const string SOME_86_DUMP = @"C:\temp\Dumps\Assignment_3.dmp";
        const int PID_NOT_FOUND = -1;
        const int ATTACH_TO_PPROCESS_TIMEOUT = 999999;
        public static void Run(int pid)
        {
            using (DataTarget target = DataTarget.AttachToProcess(pid, ATTACH_TO_PPROCESS_TIMEOUT))
            {
                DoAnaytics(target);
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

        private static void DoAnaytics(DataTarget target)
        {
            ThreadStackHandler handler = new ThreadStackHandler();
            var runtime = target.ClrVersions[0].CreateRuntime();

            var result = handler.Handle(target.DebuggerInterface, runtime);

            PrintHandler.Print(result, true);



            Console.ReadKey();
        }
    }
}
