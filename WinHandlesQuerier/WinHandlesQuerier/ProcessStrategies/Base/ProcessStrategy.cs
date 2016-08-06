using WinHandlesQuerier.Core.Model;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Runtime;
using System;
using WinHandlesQuerier.CmdParams;

namespace WinHandlesQuerier.ProcessStrategies
{
    public abstract class ProcessStrategy
    {
        public ProcessStrategy(uint pid)
        {
            _pid = pid;
        }

        public ProcessStrategy(string filePath)
        {
            _filePath = filePath;
        }

        protected uint _pid;
        protected string _filePath;

        public CommonVerb Options { get; internal set; }

        public abstract Task<ProcessAnalysisResult> Run();

        protected ProcessAnalysisResult SetError(string message)
        {
            return new ProcessAnalysisResult()
            {
                Error = new ProcessAnalysisResultError() { Description = message }
            };
        }

        internal bool IsSuitableBitness(DataTarget target)
        {
            bool resut = true;

            if (Environment.Is64BitProcess && target.Architecture != Architecture.Amd64)
            {
                SetError("Unexpected architecture. Process runs as x64");
                resut = false;
            }
            return resut;
        }
    }
}
