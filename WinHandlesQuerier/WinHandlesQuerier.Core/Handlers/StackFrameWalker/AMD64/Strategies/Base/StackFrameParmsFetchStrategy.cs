using System;
using Microsoft.Diagnostics.Runtime;
using WinHandlesQuerier.Core.Model.Unified;

namespace WinHandlesQuerier.Core.Handlers.UnmanagedStackFrameWalker.AMD64
{
    /// <summary>
    /// x64 Registers Names
    /// https://msdn.microsoft.com/en-us/library/ff561499.aspx
    /// </summary>
    internal abstract class StackFrameParmsFetchStrategy
    {
        public StackFrameParmsFetchStrategy(ClrRuntime runtime)
        {
            _runtime = runtime;
        }

        protected ClrRuntime _runtime;

        internal abstract Params GetWaitForMultipleObjectsParams(UnifiedStackFrame frame);
        
        internal virtual Params GetWaitForSingleObjectParams(UnifiedStackFrame frame)
        {
            Params result = new Params();

            result.First = frame.ThreadContext.Context_amd64.Rdi;
            result.Second = frame.ThreadContext.Context_amd64.Rsi;

            return result;
        }

        internal virtual Params GetEnterCriticalSectionParam(UnifiedStackFrame frame)
        {
            Params result = new Params();
            result.First = frame.ThreadContext.Context_amd64.Rdi;
            return result;
        }

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

        internal ulong ReadBoolean(ulong stackPointer)
        {
            byte[] buffer = new byte[IntPtr.Size];
            int read = 0;

            bool waitAllFlagParam = false;
            if (_runtime.ReadMemory(stackPointer, buffer, buffer.Length, out read))
            {
                waitAllFlagParam = BitConverter.ToBoolean(buffer, 0);
            }

            return waitAllFlagParam ? (ulong)1 : (ulong)0;
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
