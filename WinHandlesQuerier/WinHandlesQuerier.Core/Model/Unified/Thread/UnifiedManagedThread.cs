using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHandlesQuerier.Core.msos;
using Microsoft.Diagnostics.Runtime;

namespace WinHandlesQuerier.Core.Model.Unified.Thread
{
    public class UnifiedManagedThread : UnifiedThread
    {
        public UnifiedManagedThread(ThreadInfo info, List<UnifiedStackFrame> managedStack, List<UnifiedStackFrame> unManagedStack, List<UnifiedBlockingObject> blockingObjects) : base(info)
        {
            StackTrace = new List<UnifiedStackFrame>();

            if (managedStack != null)
            {
                StackTrace.AddRange(managedStack);

            }

            if (unManagedStack != null)
            {
                StackTrace.AddRange(unManagedStack);
            }
            BlockingObjects = blockingObjects;
        }

        public UnifiedManagedThread(ClrThread waiter)
            : base(new ThreadInfo()
            {
                OSThreadId = waiter.OSThreadId,
                ManagedThread = waiter
            })
        {
            //TODO: complete logic -> used with Blocking object Wiater    


        }
    }
}
