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
        public UnifiedManagedThread(ThreadInfo info) : base (info)
        {

        }

        public UnifiedManagedThread(ClrThread thread, ThreadInfo specific_info) : this(specific_info)
        {
            MangedThread = thread;
        }

        public UnifiedManagedThread(ClrThread waiter) : base()
        {
            MangedThread = waiter;
            base.IsManagedThread = true;
            base.OSThreadId = waiter.OSThreadId;
        }

        public ClrThread MangedThread { get; private set; }
    }
}
