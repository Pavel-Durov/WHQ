using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.Model.Unified;
using Assignments.Core.Model.Unified.Thread;
using Assignments.Core.msos;
using Microsoft.Diagnostics.Runtime;

namespace Assignments.Core.Handlers.StackAnalysis.Strategies
{
    public class BlockingObjectsFetcherProcessDumpStrategy : BlockingObjectsFetcherStrategy
    {

        public BlockingObjectsFetcherProcessDumpStrategy(string dumpFilePath)
        {
            _miniDump = new MiniDump.MiniDumpHandler(dumpFilePath);
        }

        MiniDump.MiniDumpHandler _miniDump;

        public override List<UnifiedBlockingObject> GetUnmanagedBlockingObjects(ThreadInfo thread, List<UnifiedStackFrame> unmanagedStack)
        {
            var miniDumpHandles = _miniDump.GetHandles();

            List<UnifiedBlockingObject> result = null;

            //Snooped unmanaged stack data
            var stackFrameHandles = from frame in unmanagedStack
                                    where frame.Handles?.Count > 0
                                    select frame;

            if (stackFrameHandles != null && stackFrameHandles.Any())
            {
                result = new List<UnifiedBlockingObject>();

                foreach (var item in miniDumpHandles)
                {
                    result.Add(new UnifiedBlockingObject(item));
                }
            }

            var modules = _miniDump.GetModuleList();
            return result;
        }
    }
}
