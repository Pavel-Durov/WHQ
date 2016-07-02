using System.Collections.Generic;
using WinHandlesQuerier.Core.Model.Unified.Thread;

namespace Assignments.Core.Model
{
    public class ProcessAnalysisResult
    {
        public List<UnifiedThread> Threads { get; set; }

        public long ElapsedMilliseconds { get; set; }
    }
}
