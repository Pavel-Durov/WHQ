using Assignments.Core.Model.Unified;
using Assignments.Core.msos;
using Microsoft.Diagnostics.Runtime;
using System.Collections.Generic;
using System.Linq;
namespace Assignments.Core.Handlers.StackAnalysis.Strategies
{
    public abstract class ProcessAnalysisStrategy
    {
        public virtual List<UnifiedBlockingObject> GetManagedBlockingObjects(ClrThread thread, List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime)
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

            var criticalSectionObjects = GetCriticalSections(unmanagedStack, runtime);
            if (criticalSectionObjects.Any())
            {
                if (result == null)
                    result = new List<UnifiedBlockingObject>();

                result.AddRange(criticalSectionObjects);
                var list = criticalSectionObjects.ToList() ;
            }

            return result;
        }

        public virtual IEnumerable<UnifiedBlockingObject> GetCriticalSections(List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime)
        {
            foreach (var item in unmanagedStack)
            {
                UnifiedBlockingObject blockObject;

                if (UnmanagedStackFrameWalker.CheckForCriticalSectionCalls(item, runtime, out blockObject))
                {
                    yield return blockObject;
                }
            }
        }

        public abstract List<UnifiedBlockingObject> GetUnmanagedBlockingObjects(ThreadInfo thread, List<UnifiedStackFrame> unmanagedStack);
    }
}
