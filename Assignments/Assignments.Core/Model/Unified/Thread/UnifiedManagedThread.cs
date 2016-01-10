using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.msos;
using Microsoft.Diagnostics.Runtime;

namespace Assignments.Core.Model.Unified.Thread
{
    public class UnifiedManagedThread : UnifiedThread
    {
        public UnifiedManagedThread(ThreadInfo info, List<UnifiedStackFrame> managedStack, List<UnifiedStackFrame> unManagedStack, List<UnifiedBlockingObject> blockingObjects) : base(info)
        {
            StackTrace = new List<UnifiedStackFrame>(managedStack);
            StackTrace.AddRange(unManagedStack);
            BlockingObjects = blockingObjects;
        }

        public UnifiedManagedThread(ClrThread waiter)
        {
            //TODO: complete logi -> used with Blocking object Wiater    
        }
    }
}
