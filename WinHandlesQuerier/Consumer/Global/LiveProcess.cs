using WinHandlesQuerier.Core.Handlers;
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

        const uint PID_NOT_FOUND = 0;
        const int ATTACH_TO_PPROCESS_TIMEOUT = 999999;

        public static void Run(uint pid)
        {
            if (pid != PID_NOT_FOUND)
            {
                Console.WriteLine("VALID PID");
            }
            else
            {
                Console.WriteLine("--- Assignment_4 C# project ----");
                Console.WriteLine("Please enter a PID: ");

                pid = uint.Parse(Console.ReadLine());
            }

            using (DataTarget target = DataTarget.AttachToProcess((int)pid, ATTACH_TO_PPROCESS_TIMEOUT))
            {
                DoAnaytics(target, pid);
            }

            Console.ReadKey();
        }

        private static void DoAnaytics(DataTarget target, uint pid)
        {

            var runtime = target.ClrVersions[0].CreateRuntime();

            //Live process handler
            ProcessAnalyzer handler = new ProcessAnalyzer(target.DebuggerInterface, runtime, pid);

            var result = handler.Handle();
            PrintHandler.Print(result, true);

            Console.ReadKey();
        }
    }
}
