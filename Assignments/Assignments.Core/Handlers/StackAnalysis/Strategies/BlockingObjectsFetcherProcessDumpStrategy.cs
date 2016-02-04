using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.Model.Unified;
using Assignments.Core.Model.Unified.Thread;
using Assignments.Core.msos;
using Microsoft.Diagnostics.Runtime;
using Assignments.Core.Model.MiniDump;

namespace Assignments.Core.Handlers.StackAnalysis.Strategies
{
    public class BlockingObjectsFetcherProcessDumpStrategy : BlockingObjectsFetcherStrategy
    {

        public BlockingObjectsFetcherProcessDumpStrategy(int pid) : base(pid)
        {
            _miniDump = new MiniDumpHandler();
        }

        private void InitMiniDumpHandler()
        {
            _miniDump.Init((uint)_pid);
            _miniDumpHandles = _miniDump.GetHandleData();
        }

        List<MiniDumpHandle> _miniDumpHandles;
        MiniDumpHandler _miniDump;
        
        public override List<UnifiedBlockingObject> GetUnmanagedBlockingObjects(ThreadInfo thread, List<UnifiedStackFrame> unmanagedStack)
        {
            return GetMiniDumpBlockingObjects(thread, unmanagedStack);
        }


        public override List<UnifiedBlockingObject> GetManagedBlockingObjects(ClrThread thread)
        {
            return base.GetClrBlockingObjects(thread);
        }

        private List<UnifiedBlockingObject> GetMiniDumpBlockingObjects(ThreadInfo thread, List<UnifiedStackFrame> unmanagedStack)
        {
            List<UnifiedBlockingObject> result = null;

            var stackFrameHandles = from frame in unmanagedStack
                                    where frame.Handles?.Count > 0
                                    select frame;


            if (stackFrameHandles != null && stackFrameHandles.Any())
            {
                result = new List<UnifiedBlockingObject>();

                foreach (var item in _miniDumpHandles)
                {
                    result.Add(new UnifiedBlockingObject(item));
                }
            }

            return result;
        }

    }
}
