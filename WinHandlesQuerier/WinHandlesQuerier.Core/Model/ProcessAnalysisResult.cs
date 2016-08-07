using System.Collections.Generic;
using WinHandlesQuerier.Core.Model.Unified.Thread;

namespace WinHandlesQuerier.Core.Model
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
