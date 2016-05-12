using System;
using System.Runtime.InteropServices;

namespace WinBase
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CRITICAL_SECTION
    {
        public IntPtr DebugInfo;
        public Int32 LockCount;
        public Int32 RecursionCount;
        public IntPtr OwningThread;        // from the thread's ClientId->UniqueThread
        public IntPtr LockSemaphore;
        public UIntPtr SpinCount;        // force size on 64-bit systems when packed
    };
}
