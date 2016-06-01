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
        {
            //TODO: Complte 64 bit logic
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

        public unsafe bool GetThreadContext(uint threadID, ref CONTEXT section)
        {
            bool result = false;
            uint contextSize = GetThreadContextSize();
            byte[] contextBytes = new byte[contextSize];

            IntPtr unmanagedPointer = Marshal.AllocHGlobal(contextBytes.Length);

            var h_result = _debugClient.GetThreadContext(unmanagedPointer, contextSize);
            if (h_result == Kernel32.Const.ERROR_SUCCESS)
            {
                var gch = GCHandle.Alloc(contextBytes, GCHandleType.Pinned);
                try
                {
                    section = Marshal.PtrToStructure<CONTEXT>(gch.AddrOfPinnedObject());
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

        private uint GetThreadContextSize()
        {
            uint result = 0;
            var plat = _dataReader.GetArchitecture();

            if (plat == Architecture.Amd64)
                result = 0x4d0;
            else if (plat == Architecture.X86)
                result = 0x2d0;
            else if (plat == Architecture.Arm)
                result = 0x1a0;
            else
                throw new InvalidOperationException("Unexpected architecture.");

            return result;
        }

        #endregion
    }
}
