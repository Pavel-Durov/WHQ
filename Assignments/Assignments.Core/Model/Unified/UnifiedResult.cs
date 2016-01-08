using Assignments.Core.Model.StackFrames;
using Assignments.Core.Model.Unified.Thread;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.msos;
using Microsoft.Diagnostics.Runtime;
using Assignments.Core.Handlers.WCT;
using Assignments.Core.Handlers;
using Assignments.Core.Model.WCT;

namespace Assignments.Core.Model.Unified
{
    public class UnifiedResult
    {
        //ctor for managed thread
        public UnifiedResult(ClrThread thread, ThreadInfo specific_info, List<UnifiedStackFrame> managedStack, List<UnifiedStackFrame> unmanagedStack, List<UnifiedBlockingObject> blockingObjects)
        {
            Thread = new UnifiedManagedThread(thread, specific_info);
            StackTrace = new List<UnifiedStackFrame>(managedStack);

            StackTrace.AddRange(unmanagedStack);
            BlockingObjects = blockingObjects;
        }

        //ctor for unmanaged thread
        public UnifiedResult(ThreadInfo specific_info, List<UnifiedStackFrame> unmanagedStack, List<UnifiedBlockingObject> blockingObjects)
        {
            BlockingObjects = blockingObjects;
            Thread = new UnifiedUnManagedThread(specific_info);
            StackTrace = unmanagedStack;
        }

        

        public UnifiedThread Thread { get; private set; }
        public List<UnifiedStackFrame> StackTrace { get; private set; }
        public List<UnifiedBlockingObject> BlockingObjects { get; private set; }

    }
}
