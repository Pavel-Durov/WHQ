using WinHandlesQuerier.Core.Infra;
using WinHandlesQuerier.Core.Model;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Threading.Tasks;
using WinHandlesQuerier.Core.Handlers;
using WinHandlesQuerier.Handlers;

namespace WinHandlesQuerier.ProcessStrategies
{
    internal class LiveProcessStrategy : ProcessStrategy
    {
        public LiveProcessStrategy(uint pid) : base(pid)
        {

        }

        public override async Task<ProcessAnalysisResult> Run()
        {
            ProcessAnalysisResult result = null;
            if (_pid == Constants.INVALID_PID)
            {
                Console.WriteLine("--- Assignment_4 C# project ----");
                Console.WriteLine("Please enter a PID: ");

                _pid = uint.Parse(Console.ReadLine());
            }
            try
            {
                using (DataTarget target = DataTarget.AttachToProcess((int)_pid, Constants.MAX_ATTACH_TO_PPROCESS_TIMEOUT))
                {
                    if (Environment.Is64BitProcess && target.Architecture != Architecture.Amd64)
                    {
                        throw new InvalidOperationException($"Unexpected architecture. Process runs as x64");
                    }

                    Console.WriteLine("Attached To Process Successfully");
                    result = await DoAnaytics(target, _pid);
                }
            }
            catch (ClrDiagnosticsException e)
            {
                PrintHandler.PrintToConsole(e.Message);
            }
          
            return result;
        }

        private static async Task<ProcessAnalysisResult> DoAnaytics(DataTarget target, uint pid)
        {
            var clrVer = target.ClrVersions[0];

            //Live process handler
            var runtime = clrVer.CreateRuntime();
            ProcessAnalyzer handler = new ProcessAnalyzer(target, runtime, pid);

            return await handler.Handle();
        }
    }
}
