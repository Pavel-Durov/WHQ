using System.Collections.Generic;
using WinHandlesQuerier.Core.Model.Unified.Thread;

namespace WinHandlesQuerier.Core.Model
{
    public class ProcessAnalysisResult
    {
        public List<UnifiedThread> Threads { get; set; }

        public long ElapsedMilliseconds { get; set; }
    }
}
