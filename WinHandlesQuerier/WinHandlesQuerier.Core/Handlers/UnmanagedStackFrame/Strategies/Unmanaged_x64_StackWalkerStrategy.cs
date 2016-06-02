using Assignments.Core.Handlers.UnmanagedStackFrame.Strategies.Base;
using System.Collections.Generic;
using Microsoft.Diagnostics.Runtime;
using WinHandlesQuerier.Core.Model.Unified;
using Microsoft.Diagnostics.Runtime.Interop;

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
         
        }

        protected override void DealWithSingle(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {
            //TODO: Complte 64 bit logic

        }

        protected override UnifiedBlockingObject ReadCriticalSectionData(UnifiedStackFrame frame, ClrRuntime runtime)
        {
            UnifiedBlockingObject result = null;

            
            return result;
        }

        #region Helpers

        #endregion
    }
}
