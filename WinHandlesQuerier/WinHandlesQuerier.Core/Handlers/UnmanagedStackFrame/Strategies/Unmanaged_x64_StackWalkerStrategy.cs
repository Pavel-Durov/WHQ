using Assignments.Core.Handlers.UnmanagedStackFrame.Strategies.Base;
using System.Collections.Generic;
using Microsoft.Diagnostics.Runtime;
using WinHandlesQuerier.Core.Model.Unified;
using Microsoft.Diagnostics.Runtime.Interop;
using WinNativeApi.WinNT;
using Assignments.Core.Infra;

namespace Assignments.Core.Handlers.UnmanagedStackFrame.Strategies
{
    /// <summary>
    /// https://msdn.microsoft.com/en-us/library/9z1stfyw.aspx
    ///RCX - Volatile - First integer argument
    ///RDX - Volatile -Second integer argument
    ///R8 - Volatile - Third integer argument
    ///R9 - Volatile - Fourth integer argument
    /// </summary>
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
                if (_globalConfigs.OsVersion == WinVersions.Win_10
                   || _globalConfigs.OsVersion == WinVersions.Win_8
                   || _globalConfigs.OsVersion == WinVersions.Win_8_1)
                {

                    var firstParam = frame.ThreadContext.Context_amd64.Rbx;
                    var secondParam = frame.ThreadContext.Context_amd64.R13;

                    //var thirdParam = frame.ThreadContext.Context_amd64.rsp;
                    var fourthParam = frame.ThreadContext.Context_amd64.R12;
                }
            }
        }

        protected override void DealWithSingle(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {

            //TODO: Complte 64 bit logic
            if (frame.ThreadContext != null)
            {
                if (_globalConfigs.OsVersion == WinVersions.Win_10
                    || _globalConfigs.OsVersion == WinVersions.Win_8
                    || _globalConfigs.OsVersion == WinVersions.Win_8_1)
                {
                    if (frame.ThreadContext.Is64Bit)
                    {
                        var handle = frame.ThreadContext.Context_amd64.Rdi;
                        var waitMs = frame.ThreadContext.Context_amd64.Rsi;
                    }
                }
            }
        }
    }
}
