using WinHandlesQuerier.Core.Model;
using System.Threading.Tasks;

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

        public abstract Task<ProcessAnalysisResult> Run();
    }
}
