using Assignments.Core.Model.Unified;
using Assignments.Core.msos;
using Microsoft.Diagnostics.Runtime;
using System.Collections.Generic;

namespace Assignments.Core.Handlers.StackAnalysis.Strategies
{
    public abstract class ProcessAnalysisStrategy
    {
        public virtual List<UnifiedBlockingObject> GetManagedBlockingObjects(ClrThread thread)
        {
            List<UnifiedBlockingObject> result = null;
            if (thread.BlockingObjects?.Count > 0)
            {
                result = new List<UnifiedBlockingObject>();

                foreach (var item in thread.BlockingObjects)
                {
                    result.Add(new UnifiedBlockingObject(item));
                }
            }
            return result;
        }

        public abstract List<UnifiedBlockingObject> GetUnmanagedBlockingObjects(ThreadInfo thread, List<UnifiedStackFrame> unmanagedStack);
    }
}
