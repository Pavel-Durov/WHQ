using WinHandlesQuerier.Core.Model;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Threading.Tasks;
using WinHandlesQuerier.Core.Handlers;
using System.IO;

namespace WinHandlesQuerier.ProcessStrategies
{
    public class DumpFileProcessStrategy : ProcessStrategy
    {
        public DumpFileProcessStrategy(string filePath) : base(filePath)
        {

        }

        public string FilePath => _filePath;

        public override async Task<ProcessAnalysisResult> Run()
        {
            ProcessAnalysisResult result = null;

            if (!File.Exists(_filePath))
            {
                result = SetError("File Not Found");
            }
            else
            {
                try
                {
                    using (DataTarget target = DataTarget.LoadCrashDump(_filePath))
                    {
                        if (Environment.Is64BitProcess && target.Architecture != Architecture.Amd64)
                        {
                            SetError("Unexpected architecture. Process runs as x64");
                        }
                        else
                        {
                            ClrRuntime runtime = target.ClrVersions[0].CreateRuntime();

                            using (ProcessAnalyzer handler = new ProcessAnalyzer(target, runtime, _filePath))
                            {
                                result = await handler.Handle();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = SetError(ex.Message);
                }
            }

            return result;
        }
    }
}
