using WinHandlesQuerier.Core.Model;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHandlesQuerier.Core.Handlers;
using WinHandlesQuerier.Core.Model.Unified.Thread;

namespace Consumer.ProcessStrategies
{
    internal class DumpFileProcessStrategy : ProcessStrategy
    {
        public DumpFileProcessStrategy(string filePath) : base(filePath)
        {

        }
        const int PID_NOT_FOUND = -1;
        const int ATTACH_TO_PPROCESS_TIMEOUT = 999999;


        public override ProcessAnalysisResult Run()
        {
            using (DataTarget target = DataTarget.LoadCrashDump(_filePath))
            {
                if (Environment.Is64BitProcess && target.Architecture != Architecture.Amd64)
                {
                    throw new InvalidOperationException($"Unexpected architecture. Process runs as x64");
                }

                ClrRuntime runtime = target.ClrVersions[0].CreateRuntime();

                ProcessAnalyzer handler = new ProcessAnalyzer(target, runtime, _filePath);

                return handler.Handle();
            }
        }
    }
}
