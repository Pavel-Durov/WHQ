using Assignments.Core.Model.Unified;
using Assignments.Core.Model.Unified.Thread;
using Assignments.Core.msos;
using Microsoft.Diagnostics.Runtime;
using System.Collections.Generic;

namespace Assignments.Core.Handlers.StackAnalysis.Strategies
{
    public abstract class StackAnalysisStrategy
    {
        public abstract UnifiedUnManagedThread HandleUnManagedThread(ThreadInfo info);

        public abstract List<UnifiedBlockingObject> GetBlockingObjects(ClrThread thread);
    }
}
