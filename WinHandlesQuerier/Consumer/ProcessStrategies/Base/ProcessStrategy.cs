using WinHandlesQuerier.Core.Model;
using System.Collections.Generic;
using WinHandlesQuerier.Core.Model.Unified.Thread;
using System.Threading.Tasks;

namespace Consumer.ProcessStrategies
{
    abstract class ProcessStrategy
    {
        private ProcessStrategy()
        {

        }

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

        public abstract Task<ProcessAnalysisResult> Run();
    }
}
