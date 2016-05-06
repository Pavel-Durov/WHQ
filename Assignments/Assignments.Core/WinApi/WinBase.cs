using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Assignments.Core.WinApi
{
    public class WinBase
    {

        public unsafe struct CRITICAL_SECTION
        {
            public RTL_CRITICAL_SECTION_DEBUG* DebugInfo;

            //
            //  The following three fields control entering and exiting the critical
            //  section for the resource
            //

            public long LockCount;
            public long RecursionCount;
            public IntPtr OwningThread;        // from the thread's ClientId->UniqueThread
            public IntPtr LockSemaphore;
            public UIntPtr SpinCount;        // force size on 64-bit systems when packed
        };

        public unsafe struct RTL_CRITICAL_SECTION_DEBUG
        {
            public ushort Type;
            public ushort CreatorBackTraceIndex;
            public CRITICAL_SECTION CriticalSection;
            public LIST_ENTRY ProcessLocksList;
            public int EntryCount;
            public int ContentionCount;
            public int Flags;
            public ushort CreatorBackTraceIndexHigh;
            public ushort SpareWORD;
        };

        public unsafe struct LIST_ENTRY
        {
            public LIST_ENTRY* Flink;
            public LIST_ENTRY* Blink;
        };
    }
}
