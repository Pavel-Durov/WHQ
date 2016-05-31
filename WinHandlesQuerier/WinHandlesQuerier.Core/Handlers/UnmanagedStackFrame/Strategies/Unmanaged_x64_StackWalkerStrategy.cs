using Assignments.Core.Handlers.UnmanagedStackFrame.Strategies.Base;
using System;
using System.Collections.Generic;
using Microsoft.Diagnostics.Runtime;
using WinHandlesQuerier.Core.Model.Unified;

namespace Assignments.Core.Handlers.UnmanagedStackFrame.Strategies
{
    class Unmanaged_x64_StackWalkerStrategy : UnmanagedStackWalkerStrategy
    {
        public override bool CheckForCriticalSectionCalls(UnifiedStackFrame frame, ClrRuntime runtime, out UnifiedBlockingObject blockingObject)
        {
            throw new NotImplementedException();
        }

        public override List<byte[]> GetNativeParams(UnifiedStackFrame stackFrame, ClrRuntime runtime, int paramCount)
        {
            throw new NotImplementedException();
        }

        public override List<byte[]> ReadFromMemmory(uint startAddress, uint count, ClrRuntime runtime)
        {
            throw new NotImplementedException();
        }

        protected override void DealWithMultiple(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {
            throw new NotImplementedException();
        }

        protected override void DealWithSingle(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {
            throw new NotImplementedException();
        }

        protected override UnifiedBlockingObject ReadCriticalSectionData(UnifiedStackFrame frame, ClrRuntime runtime)
        {
            throw new NotImplementedException();
        }
    }
}
