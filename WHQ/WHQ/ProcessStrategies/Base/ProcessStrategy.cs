using WHQ.Core.Model;
using System.Threading.Tasks;
using System;
using WHQ.CmdParams;
using WHQ.Providers.ClrMd.Model;

namespace WHQ.ProcessStrategies
{
    public abstract class ProcessStrategy
    {
        public ProcessStrategy(int pid)
        {
            _pid = pid;
        }

        public ProcessStrategy(string filePath)
        {
            _filePath = filePath;
        }

        protected int _pid;
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

        internal bool CheckTargetBitness(DataTarget target)
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
