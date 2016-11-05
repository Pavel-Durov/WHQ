using System.Collections.Generic;
using WHQ.Core.Model.Unified.Thread;

namespace WHQ.Core.Model
{
    public class ProcessAnalysisResult
    {
        public List<UnifiedThread> Threads { get; set; }

        public long ElapsedMilliseconds { get; set; }

        public ProcessAnalysisResultError Error { get; set; }
    }

    public class ProcessAnalysisResultError
    {
        public string Description { get; set; }
    }
}
