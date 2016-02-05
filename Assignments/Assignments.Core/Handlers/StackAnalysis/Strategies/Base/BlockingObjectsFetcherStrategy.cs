using Assignments.Core.Model.Unified;
using Assignments.Core.Model.Unified.Thread;
using Assignments.Core.msos;
using Microsoft.Diagnostics.Runtime;
using System.Collections.Generic;

namespace Assignments.Core.Handlers.StackAnalysis.Strategies
{
    public abstract class BlockingObjectsFetcherStrategy
    {
        public BlockingObjectsFetcherStrategy(string pathToDump)
        {
            _pathToDump = pathToDump;
        }

        public BlockingObjectsFetcherStrategy(int pid)
        {
            _pid = pid;
        }

        protected int _pid;
        protected string _pathToDump;

        public abstract List<UnifiedBlockingObject> GetManagedBlockingObjects(ClrThread thread);

        public abstract List<UnifiedBlockingObject> GetUnmanagedBlockingObjects(ThreadInfo thread, List<UnifiedStackFrame> unmanagedStack);


        protected List<UnifiedBlockingObject> GetClrBlockingObjects(ClrThread thread)
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
    }
}
