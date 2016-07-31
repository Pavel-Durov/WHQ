using WinHandlesQuerier.Core.Model;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Threading.Tasks;
using WinHandlesQuerier.Core.Handlers;

namespace WinHandlesQuerier.ProcessStrategies
{
    public class DumpFileProcessStrategy : ProcessStrategy
    {
        public DumpFileProcessStrategy(string filePath) : base(filePath)
        {

        }
        const int PID_NOT_FOUND = -1;

        public string FilePath => _filePath;

        public override async Task<ProcessAnalysisResult> Run()
        {
            using (DataTarget target = DataTarget.LoadCrashDump(_filePath))
            {
                if (Environment.Is64BitProcess && target.Architecture != Architecture.Amd64)
                {
                    throw new InvalidOperationException($"Unexpected architecture. Process runs as x64");
                }

                ClrRuntime runtime = target.ClrVersions[0].CreateRuntime();

                ProcessAnalyzer handler = new ProcessAnalyzer(target, runtime, _filePath);

                return await handler.Handle();
            }
        }
    }
}
