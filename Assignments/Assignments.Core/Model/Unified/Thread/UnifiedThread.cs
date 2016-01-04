using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Runtime;
using Assignments.Core.Model.Unified.Thread;
using Assignments.Core.msos;

namespace Assignments.Core.Model.Unified
{
    public class UnifiedThread
    {
        public UnifiedThread()
        {

        }

        public UnifiedThread(ThreadInfo info, uint index, uint engineThreadId, uint OSThreadId, string detail)
        {
            this.IsManagedThread = info.IsManagedThread;
            this.Index = index;
            this.EngineThreadId = engineThreadId;
            this.OSThreadId = OSThreadId;
            this.Detail = Detail;
        }

        public bool IsManagedThread { get; protected set; }

        public uint Index { get; set; }

        public uint EngineThreadId { get; set; }

        public uint OSThreadId { get; set; }
 
        public string Detail { get; set; }
    }
}
