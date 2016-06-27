using Assignments.Core.Handlers.UnmanagedStackFrame.Strategies.Base;
using Microsoft.Diagnostics.Runtime;
using WinHandlesQuerier.Core.Model.Unified;
using Assignments.Core.Infra;
using System;
using Assignments.Core.Handlers.UnmanagedStackFrameWalker.Strategies.x64;

namespace Assignments.Core.Handlers.UnmanagedStackFrame.Strategies
{
    /// <summary>
    /// This class is responsible for fetching function parameters.
    /// 
    /// Since it's x64 StackWalkerStrategy, 
    /// it relies on x64 Calling Convention - passing parameters to function using CPU registers. 
    /// 
    /// x64 Software Conventions - Calling Convention : https://msdn.microsoft.com/en-us/library/zthk2dkh.aspx
    /// x64 Software Conventions - Register Usage: https://msdn.microsoft.com/en-us/library/9z1stfyw.aspx
    /// </summary>
    internal class Unmanaged_x64_StackWalkerStrategy : UnmanagedStackWalkerStrategy
    {
        public Unmanaged_x64_StackWalkerStrategy()
        {
            _globalConfigs = Config.GetInstance();
        }

        StackFrameParmsFetchStrategy _unmanagedStackFrameParmFetcher;

        public StackFrameParmsFetchStrategy Strategy
        {
            get
            {
                if (_unmanagedStackFrameParmFetcher == null)
                {
                    SetStrategy();
                }
                return _unmanagedStackFrameParmFetcher;
            }
        }

        Config _globalConfigs;

        private void SetStrategy()
        {
            switch (_globalConfigs.OsVersion)
            {

                case WinVersions.Win_8_1: _unmanagedStackFrameParmFetcher = new StackFrameParmsFetchStrategy_Win_8_1(); break;
                case WinVersions.Win_8:
                    break;
                case WinVersions.Win_10: _unmanagedStackFrameParmFetcher = new StackFrameParmsFetchStrategy_Win_10(); break;
                case WinVersions.Win_7:
                    break;
                case WinVersions.Win_Vista:
                    break;
                case WinVersions.Win_XP:
                    break;
                case WinVersions.Win_XP_Pro_x64:
                    break;
                case WinVersions.Win_Me:
                    break;
                case WinVersions.Win_2000:
                    break;
                case WinVersions.Win_98:
                    break;
                default:
                    break;
            }
        }



        /// <summary>
        /// Original Function call example: 
        ///     void WINAPI EnterCriticalSection(LPCRITICAL_SECTION lpCriticalSection);
        /// </summary>
        protected override UnifiedBlockingObject GetCriticalSectionBlockingObject(UnifiedStackFrame frame, ClrRuntime runtime)
        {
            UnifiedBlockingObject result = null;

            var paramz = Strategy.GetenterCriticalSectionParam(frame);
            result = new UnifiedBlockingObject(paramz.First, UnifiedBlockingType.CriticalSectionObject);

            return result;
        }

        /// <summary>
        /// Original Function call example: 
        ///     void WINAPI EnterCriticalSection(LPCRITICAL_SECTION lpCriticalSection);
        /// </summary>
        protected override void DealWithCriticalSection(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {
            var paramz = Strategy.GetenterCriticalSectionParam(frame);
            EnrichUnifiedStackFrame(frame, paramz.First, pid);
        }

        /// <summary>
        /// Original Function call example: 
        ///     DWORD WaitForMultipleObjects(DWORD  nCount,HANDLE* lpHandles,BOOL bWaitAll,DWORD dwMilliseconds);
        /// </summary>
        protected override void DealWithMultiple(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {
            var paramz = Strategy.GetWaitForMultipleObjectsParams(frame, runtime);
            EnrichUnifiedStackFrame(frame, runtime, pid, paramz.First, paramz.Second);
        }

        /// <summary>
        ///  Original Function call example: 
        ///    DWORD WINAPI WaitForSingleObject(HANDLE hHandle,DWORD  dwMilliseconds);
        /// </summary>
        protected override void DealWithSingle(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {
            var paramz = Strategy.GetWaitForSingleObjectParams(frame);
            EnrichUnifiedStackFrame(frame, paramz.First, pid);
        }
    }
}
