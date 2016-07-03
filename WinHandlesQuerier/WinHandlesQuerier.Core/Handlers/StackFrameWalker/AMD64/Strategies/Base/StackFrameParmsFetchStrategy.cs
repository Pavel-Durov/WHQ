using System;
using Microsoft.Diagnostics.Runtime;
using WinHandlesQuerier.Core.Model.Unified;

namespace WinHandlesQuerier.Core.Handlers.UnmanagedStackFrameWalker.AMD64
{
    internal abstract class StackFrameParmsFetchStrategy
    {
        public StackFrameParmsFetchStrategy(ClrRuntime runtime)
        {
            _runtime = runtime;
        }

        protected ClrRuntime _runtime;

        internal abstract Params GetWaitForMultipleObjectsParams(UnifiedStackFrame frame);
        internal abstract Params GetWaitForSingleObjectParams(UnifiedStackFrame frame);
        internal abstract Params GetenterCriticalSectionParam(UnifiedStackFrame frame);

        internal long ReadLong(ulong rspPtr)
        {
            long result = 0;
            byte[] buffer = new byte[IntPtr.Size];
            int read = 0;

            if (_runtime.ReadMemory(rspPtr, buffer, buffer.Length, out read))
            {
                result = BitConverter.ToInt64(buffer, 0);
            }
            return result;
        }
    }

    internal struct Params
    {
        internal ulong First;
        internal ulong Second;
        internal ulong Third;
        internal ulong Fourth;
    }
}
