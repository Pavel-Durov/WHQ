using Assignments.Core.Handlers.UnmanagedStackFrame.Strategies.Base;
using System.Collections.Generic;
using Microsoft.Diagnostics.Runtime;
using WinHandlesQuerier.Core.Model.Unified;
using Microsoft.Diagnostics.Runtime.Interop;
using WinNativeApi.WinNT;
using Assignments.Core.Infra;

namespace Assignments.Core.Handlers.UnmanagedStackFrame.Strategies
{
    class Unmanaged_x64_StackWalkerStrategy : UnmanagedStackWalkerStrategy
    {
        public Unmanaged_x64_StackWalkerStrategy()
        {
            _globalConfigs = Config.GetInstance();
        }

        Config _globalConfigs;

        public override List<byte[]> GetNativeParams(UnifiedStackFrame frame, ClrRuntime runtime, int paramCount)
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


        protected override UnifiedBlockingObject ReadCriticalSectionData(UnifiedStackFrame frame, ClrRuntime runtime)
        {
            UnifiedBlockingObject result = null;

            if (frame.ThreadContext != null)
            {

            }

            return result;
        }

        protected override void DealWithMultiple(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {

            //TODO: Complte 64 bit logic

            if (frame.ThreadContext != null)
            {

            }
        }

        protected override void DealWithSingle(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {

            //TODO: Complte 64 bit logic
            if (frame.ThreadContext != null)
            {
                if (_globalConfigs.OsVersion == WinVersions.Win_10)
                {
                    if(frame.ThreadContext.Is64Bit)
                    {
                        //1st Parameter is located in : RCX, 2nd Parameter is located in : RDX
                        var firstParam = frame.ThreadContext.Context_amd64.Rcx;
                        var scondParam = frame.ThreadContext.Context_amd64.Rdx;
                      }
                }
            }
        }
    }
}
