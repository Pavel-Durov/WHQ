using Assignments.Core.Handlers.UnmanagedStackFrame.Strategies.Base;
using System;
using System.Collections.Generic;
using Microsoft.Diagnostics.Runtime;
using WinHandlesQuerier.Core.Model.Unified;
using Microsoft.Diagnostics.Runtime.Interop;
using WinNativeApi.WinNT;
using System.Threading;
using System.Runtime.InteropServices;

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

        public override List<byte[]> GetNativeParams(UnifiedStackFrame stackFrame, ClrRuntime runtime, int paramCount)
        {
            List<byte[]> result = null;

            //TODO: Complte 64 bit logic

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
            Test(frame);
        }

        private void Test(UnifiedStackFrame frame)
        {

            int contextSize;

            contextSize = 0x2d0;
            // var plat = _debugClient.GetArchitecture();
            //if (plat == Architecture.Amd64)
            //    contextSize = 0x4d0;
            //else if (plat == Architecture.X86)
            //    contextSize = 0x2d0;
            //else if (plat == Architecture.Arm)
            //    contextSize = 0x1a0;
            //else
            //    throw new InvalidOperationException("Unexpected architecture.");

            CONTEXT cont = new CONTEXT();
            bool res = Kernel32.Functions.GetThreadContext(frame.OsThreadId, ref cont);
            if (!res)
            {
                var error = Marshal.GetLastWin32Error();

            }

            byte[] context = new byte[contextSize];

            var result = _debugClient.GetThreadContext(frame.OsThreadId, (uint)contextSize);
        }

        protected override void DealWithSingle(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {
            Test(frame);
            //TODO: Complte 64 bit logic
        }

        protected override UnifiedBlockingObject ReadCriticalSectionData(UnifiedStackFrame frame, ClrRuntime runtime)
        {
            UnifiedBlockingObject result = null;

            //TODO: Complte 64 bit logic
            Test(frame);
            return result;
        }
    }
}
