using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Runtime;
using Assignments.Core.Model.Unified.Thread;

namespace Assignments.Core.Model.Unified
{
    public abstract class UnifiedThread
    {
        public bool IsManagedThread { get; protected set; }

        public uint Index { get; set; }

        public uint EngineThreadId { get; set; }

        public uint OSThreadId { get; set; }
 
        public string Detail { get; set; }
    }
}
