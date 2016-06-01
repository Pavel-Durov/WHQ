using Assignments.Core.Handlers.UnmanagedStackFrame.Strategies.Base;
using System;
using System.Collections.Generic;
using Microsoft.Diagnostics.Runtime;
using WinHandlesQuerier.Core.Model.Unified;
using Microsoft.Diagnostics.Runtime.Interop;
using WinNativeApi.WinNT;
using System.Runtime.InteropServices;
using Kernel32;

namespace Assignments.Core.Handlers.UnmanagedStackFrame.Strategies
{
    class Unmanaged_x64_StackWalkerStrategy : UnmanagedStackWalkerStrategy
    {
        public Unmanaged_x64_StackWalkerStrategy(IDebugAdvanced debugClient)
        {
            //TODO: Complte 64 bit logic
            _debugClient = debugClient;
        }

        IDebugAdvanced _debugClient;

        public override List<byte[]> GetNativeParams(UnifiedStackFrame frame, ClrRuntime runtime, int paramCount)
        {
            List<byte[]> result = null;

            //TODO: Complte 64 bit logic
            CONTEXT threadContext = new CONTEXT();

            if (GetThreadContext(frame.OsThreadId, ref threadContext))
            {
                //TODO: something with threadContext
            }
            return result;
        }

        public override List<byte[]> ReadFromMemmory(uint startAddress, uint count, ClrRuntime runtime)
        {
            List<byte[]> result = null;

            //TODO: Complte 64 bit logic

            return result;
        }

        protected override void DealWithMultiple(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {

            //TODO: Complte 64 bit logic
            CONTEXT threadContext = new CONTEXT();

            if (GetThreadContext(frame.OsThreadId, ref threadContext))
            {
                //TODO: something with threadContext
            }
        }

        protected override void DealWithSingle(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {
            //TODO: Complte 64 bit logic
            CONTEXT threadContext = new CONTEXT();

            if (GetThreadContext(frame.OsThreadId, ref threadContext))
            {
                //TODO: something with threadContext
            }
        }

        protected override UnifiedBlockingObject ReadCriticalSectionData(UnifiedStackFrame frame, ClrRuntime runtime)
        {
            UnifiedBlockingObject result = null;

            //TODO: Complte 64 bit logic
            CONTEXT threadContext = new CONTEXT();

            if (GetThreadContext(frame.OsThreadId, ref threadContext))
            {
                //TODO: something with threadContext
            }

            return result;
        }

        #region Helpers

        private bool GetThreadContext(uint osThreadId, ref CONTEXT refResult)
        {
            IntPtr handle = IntPtr.Zero;
            try
            {
                handle = Kernel32.Functions.OpenThread(ThreadAccess.GET_CONTEXT, false, osThreadId);
                return Kernel32.Functions.GetThreadContext(handle, ref refResult);
            }
            finally
            {
                Kernel32.Functions.CloseHandle(handle);
            }
        }

        #endregion
    }
}
