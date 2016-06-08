using Assignments.Core.Handlers.UnmanagedStackFrame.Strategies.Base;
using System.Collections.Generic;
using Microsoft.Diagnostics.Runtime;
using WinHandlesQuerier.Core.Model.Unified;
using Microsoft.Diagnostics.Runtime.Interop;
using WinNativeApi.WinNT;
using Assignments.Core.Infra;
using System;

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

        protected override UnifiedBlockingObject ReadCriticalSectionData(UnifiedStackFrame frame, ClrRuntime runtime)
        {
            UnifiedBlockingObject result = null;

            if (frame.ThreadContext != null)
            {
                if (_globalConfigs.OsVersion == WinVersions.Win_10
                  || _globalConfigs.OsVersion == WinVersions.Win_8
                  || _globalConfigs.OsVersion == WinVersions.Win_8_1)
                {
                    var firstParam = frame.ThreadContext.Context_amd64.Rcx;
                }
            }

            return result;
        }

        protected override void DealWithMultiple(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {
            if (frame.ThreadContext != null)
            {
                if (_globalConfigs.OsVersion == WinVersions.Win_10
                   || _globalConfigs.OsVersion == WinVersions.Win_8
                   || _globalConfigs.OsVersion == WinVersions.Win_8_1)
                {
                    //1st : handlesCount (DWORD)
                    var firstParam = frame.ThreadContext.Context_amd64.Rbx;
                    //2nd: Handles pointer (HANDLE)
                    var secondParam = frame.ThreadContext.Context_amd64.R13;
                    //3rd: WaitAll (BOOLEAN)
                    var thirdParam = frame.ThreadContext.Context_amd64.R15;
                    //4th: Timeout (DWORD)
                    var fourthParam = frame.ThreadContext.Context_amd64.R12;
                }
            }
        }

        protected override void DealWithSingle(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {
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
