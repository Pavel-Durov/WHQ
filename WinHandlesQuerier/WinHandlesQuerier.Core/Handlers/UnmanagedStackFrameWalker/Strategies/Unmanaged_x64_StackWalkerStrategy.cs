using Assignments.Core.Handlers.UnmanagedStackFrame.Strategies.Base;
using Microsoft.Diagnostics.Runtime;
using WinHandlesQuerier.Core.Model.Unified;
using Assignments.Core.Infra;

namespace Assignments.Core.Handlers.UnmanagedStackFrame.Strategies
{
    /// <summary>
    /// This class is responsible for fetching function parameters.
    /// 
    /// Since it's x64 StackWalkerStrategy, 
    /// it relies on x64 Calling Convention - passing parameters to function using CPU registers. 
    /// 
    ///https://msdn.microsoft.com/en-us/library/9z1stfyw.aspx
    ///RCX - Volatile - First integer argument
    ///RDX - Volatile -Second integer argument
    ///R8 - Volatile - Third integer argument
    ///R9 - Volatile - Fourth integer argument
    /// </summary>
    internal class Unmanaged_x64_StackWalkerStrategy : UnmanagedStackWalkerStrategy
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

                    result = new UnifiedBlockingObject(firstParam, UnifiedBlockingType.CriticalSectionObject);
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
                    var hWaitCount = frame.ThreadContext.Context_amd64.Rbx;
                    //2nd: Handles pointer (HANDLE)
                    var hPtr = frame.ThreadContext.Context_amd64.R13;
                    //3rd: WaitAll (BOOLEAN)
                    var thirdParam = frame.ThreadContext.Context_amd64.R15;
                    //4th: Timeout (DWORD)
                    var fourthParam = frame.ThreadContext.Context_amd64.R12;

                    EnrichUnifiedStackFrame(frame, runtime, pid, hWaitCount, hPtr);
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
                        var watTime = frame.ThreadContext.Context_amd64.R12;

                        EnrichUnifiedStackFrame(frame, handle, pid);
                    }
                }
            }
        }
    }
}
