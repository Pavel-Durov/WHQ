using WinHandlesQuerier.Core.Infra;
using WinHandlesQuerier.Core.Model;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Threading.Tasks;
using WinHandlesQuerier.Core.Handlers;

namespace WinHandlesQuerier.ProcessStrategies
{
    public class LiveProcessStrategy : ProcessStrategy
    {
        public LiveProcessStrategy(int pid) : base(pid)
        {

        }

        public int PID => _pid;

        public override async Task<ProcessAnalysisResult> Run()
        {
            ProcessAnalysisResult result = null;

            try
            {
                using (DataTarget target = DataTarget.AttachToProcess((int)_pid, Constants.MAX_ATTACH_TO_PPROCESS_TIMEOUT))
                {
                    if (CheckTargetBitness(target))
                    {
                        Console.WriteLine("Attached To Process Successfully");

                        var clrVer = target.ClrVersions[0];

                        var runtime = clrVer.CreateRuntime();

                        using (ProcessAnalyzer handler = new ProcessAnalyzer(target, runtime, (uint)_pid))
                        {
                            result = await handler.Handle();
                        }
                    }
                }
            }
            catch (ClrDiagnosticsException ex)
            {
                result = SetError(ex.Message);
            }

            return result;
        }
    }
}
