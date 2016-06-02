using Assignments.Core.Model;
using Microsoft.Diagnostics.Runtime;
using WinNativeApi.WinNT;

namespace WinHandlesQuerier.Core.msos
{

    public class ThreadInfo
    {
        public uint Index { get; set; }
        public uint EngineThreadId { get; set; }
        public uint OSThreadId { get; set; }
        public ClrThread ManagedThread { get; set; }
        public string Detail { get; set; }

        public bool IsManagedThread { get { return ManagedThread != null; } }

        internal UnifiedThreadContext ContextStruct { get; set; }
    }

}
