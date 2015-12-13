using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.Handlers.WCT;

namespace Assignments.Core.Model.Blocking
{
    public class ThreadObjectInfo
    {

        public ThreadObjectInfo(_WAITCHAIN_NODE_INFO_THREAD_OBJECT threadObject)
        {
            this.ProcessId = threadObject.ProcessId;
            this.ThreadId = threadObject.ThreadId;
            this.WaitTime = threadObject.WaitTime;
            this.ContextSwitches = threadObject.ContextSwitches;
        }

        /// <summary>
        /// The process identifier
        /// </summary>
        public UInt32 ProcessId { get; private set; }
        /// <summary>
        /// The thread identifier. For COM and ALPC, this member can be 0.
        /// </summary>
        public UInt32 ThreadId { get; private set; }
        /// <summary>
        /// The wait time.
        /// </summary>
        public UInt32 WaitTime { get; private set; }
        /// <summary>
        /// The number of context switches.
        /// </summary>
        public UInt32 ContextSwitches { get; private set; }

    }
}
