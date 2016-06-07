using WinHandlesQuerier.Core.Handlers;
using Microsoft.Diagnostics.Runtime;
using System;
using Assignments.Core.Infra;

namespace Consumer.Global
{
    class LiveProcess
    {
        public static void Run(uint pid = Constants.INVALID_PID)
        {
            if (pid == Constants.INVALID_PID)
            {
                Console.WriteLine("--- Assignment_4 C# project ----");
                Console.WriteLine("Please enter a PID: ");

                pid = uint.Parse(Console.ReadLine());
            }

            using (DataTarget target = DataTarget.AttachToProcess((int)pid, Constants.MAX_ATTACH_TO_PPROCESS_TIMEOUT))
            {
                if (Environment.Is64BitProcess && target.Architecture != Architecture.Amd64)
                {
                    throw new InvalidOperationException($"Unexpected architecture. Process runs as x64");
                }

                Console.WriteLine("Attached To Process Successfully");
                DoAnaytics(target, pid);

            }

            Console.ReadKey();
        }

        private static void DoAnaytics(DataTarget target, uint pid)
        {

            var clrVer = target.ClrVersions[0];

            //Live process handler
            var runtime = clrVer.CreateRuntime();
            ProcessAnalyzer handler = new ProcessAnalyzer(target, runtime, pid);

            var result = handler.Handle();
            PrintHandler.Print(result, true);

            Console.ReadKey();
        }
    }
}
