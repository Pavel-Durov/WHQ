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
    class LiveProcess
    {

        const int PID_NOT_FOUND = -1;
        const int ATTACH_TO_PPROCESS_TIMEOUT = 999999;

        public static void Run(int pid)
        {

            //int pid = GetPidFromDumpProcessTextFile();

            if (pid != PID_NOT_FOUND)
            {
                Console.WriteLine("PID found in Assignment_3.DumpTest file :) ");
            }
            else
            {
                Console.WriteLine("--- Assignment_4 C# project ----");
                Console.WriteLine("Please enter a PID: ");

                pid = int.Parse(Console.ReadLine());
            }

            using (DataTarget target = DataTarget.AttachToProcess(pid, ATTACH_TO_PPROCESS_TIMEOUT))
            {
                DoAnaytics(target, pid);
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

        private static void DoAnaytics(DataTarget target, int pid)
        {

            var runtime = target.ClrVersions[0].CreateRuntime();

            //Live process handler
            ThreadStackHandler handler = new ThreadStackHandler(target.DebuggerInterface, runtime, pid);

            var result = handler.Handle();
            PrintHandler.Print(result, true);

            Console.ReadKey();
        }
    }
}
