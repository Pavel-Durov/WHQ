using Assignments.Core.Handlers.UnmanagedStackFrame.Strategies.Base;
using System;
using System.Collections.Generic;
using Microsoft.Diagnostics.Runtime;
using WinHandlesQuerier.Core.Model.Unified;
using Microsoft.Diagnostics.Runtime.Interop;
using WinNativeApi.WinNT;
using System.Runtime.InteropServices;
using Kernel32;
using WinHandlesQuerier.Core.Exceptions;

namespace Assignments.Core.Handlers.UnmanagedStackFrame.Strategies
{
    class Unmanaged_x64_StackWalkerStrategy : UnmanagedStackWalkerStrategy
    {
        public Unmanaged_x64_StackWalkerStrategy(IDebugAdvanced debugClient, IDataReader dataReader, ClrRuntime runtime) 
            : base (CONTEXT_SIZE_AMD64)
        {
            _debugClient = debugClient;
            _dataReader = dataReader;
            _runtime = runtime;

            
        }
        
        IDataReader _dataReader;
        IDebugAdvanced _debugClient;
        ClrRuntime _runtime;

        public override List<byte[]> GetNativeParams(UnifiedStackFrame frame, ClrRuntime runtime, int paramCount)
        {
            List<byte[]> result = null;

            //TODO: Complte 64 bit logic
            var threadContext = new CONTEXT_AMD64();

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
            var threadContext = new CONTEXT_AMD64();

            if (GetThreadContext(frame.OsThreadId, ref threadContext))
            {
                //TODO: something with threadContext
            }
        }

        protected override void DealWithSingle(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {
            //TODO: Complte 64 bit logic
            var threadContext = new CONTEXT_AMD64();

            if (GetThreadContext(frame.OsThreadId, ref threadContext))
            {
                //TODO: something with threadContext
            }
        }

        protected override UnifiedBlockingObject ReadCriticalSectionData(UnifiedStackFrame frame, ClrRuntime runtime)
        {
            UnifiedBlockingObject result = null;

            //TODO: Complte 64 bit logic
            CONTEXT_AMD64 threadContext = new CONTEXT_AMD64();

            if (GetThreadContext(frame.OsThreadId, ref threadContext))
            {
                //TODO: something with threadContext
            }

            return result;
        }

        #region Helpers

        public unsafe bool GetThreadContext(uint threadID, ref CONTEXT_AMD64 section)
        {
            bool result = false;
            
            byte[] contextBytes = new byte[ContextSize];

            var handle = Kernel32.Functions.OpenThread(ThreadAccess.GET_CONTEXT, false, threadID);
            IntPtr unmanagedPointer = Marshal.AllocHGlobal(contextBytes.Length);

            
            var h_result = _debugClient.GetThreadContext(unmanagedPointer, ContextSize);
            if (h_result == Kernel32.Const.ERROR_SUCCESS)
            {
                var gch = GCHandle.Alloc(contextBytes, GCHandleType.Pinned);
                try
                {
                    section = Marshal.PtrToStructure<CONTEXT_AMD64>(gch.AddrOfPinnedObject());
                    
                    result = true;
                }
                finally
                {
                    gch.Free();
                    Marshal.FreeHGlobal(unmanagedPointer);
                }
            }
            return result;
        }

        #endregion
    }
}
