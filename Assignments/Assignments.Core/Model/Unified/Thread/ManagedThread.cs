using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Runtime;

namespace Assignments.Core.Model.Unified.Thread
{
    public class ManagedThread : UnifiedThread
    {
        public ManagedThread(ClrThread thread, string detail)
        {
            Thread = thread;

            base.OSThreadId = Thread.OSThreadId;
            base.IsManagedThread = true;
            //base.EngineThreadId = Thread.
            //base.Index
            base.Detail = detail;
        }

        public ClrThread Thread { get; private set; }

    }
}
