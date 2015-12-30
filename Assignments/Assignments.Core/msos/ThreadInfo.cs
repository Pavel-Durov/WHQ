using Microsoft.Diagnostics.Runtime;

namespace Assignments.Core.msos
{

    public class ThreadInfo
    {
        public uint Index { get; set; }
        public uint EngineThreadId { get; set; }
        public uint OSThreadId { get; set; }
        public ClrThread ManagedThread { get; set; }
        public string Detail { get; set; }

        public bool IsManagedThread { get { return ManagedThread != null; } }
    }

}
