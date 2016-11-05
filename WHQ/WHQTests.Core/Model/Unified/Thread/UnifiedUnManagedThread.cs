using WHQ.Core.msos;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHQ.Core.Model.Unified.Thread
{
    public class UnifiedUnManagedThread : UnifiedThread
    {
        public UnifiedUnManagedThread(ThreadInfo info, List<UnifiedStackFrame> unmanagedStack, List<UnifiedBlockingObject> blockingObjects) : base(info)
        {
            BlockingObjects = blockingObjects;
            StackTrace = unmanagedStack;
        }
    }
}
