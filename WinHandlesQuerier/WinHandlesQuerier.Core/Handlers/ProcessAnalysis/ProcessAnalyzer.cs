using Microsoft.Diagnostics.Runtime;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Diagnostics.Runtime.Interop;
using WinHandlesQuerier.Core.msos;
using WinHandlesQuerier.Core.Model.Unified;
using System;
using WinHandlesQuerier.Core.Model.Unified.Thread;
using WinHandlesQuerier.Core.Handlers.StackAnalysis.Strategies;
using WinHandlesQuerier.Core.Infra;
using System.Diagnostics;
using WinHandlesQuerier.Core.Model;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace WinHandlesQuerier.Core.Handlers
{
    public class ProcessAnalyzer : IDisposable
    {
        /// <summary>
        /// Used for live process analysis
        /// </summary>
        public ProcessAnalyzer(DataTarget dataTarget, ClrRuntime runtime, uint pid) : this(dataTarget, runtime)
        {
            _pid = pid;
            _processQuerierStrategy = new LiveProcessQuerierStrategy(_debugClient, _dataReader, runtime);
            _globalConfig.Init(_processQuerierStrategy.CPUArchitechture);
        }

        /// <summary>
        /// Used for dump file analysis
        /// </summary>
        public ProcessAnalyzer(DataTarget dataTarget, ClrRuntime runtime, string pathToDumpFile) : this(dataTarget, runtime)
        {
            var dumpStrategy = new DumpFileQuerierStrategy(pathToDumpFile, _runtime, _debugClient, _dataReader);
            _processQuerierStrategy = dumpStrategy;
            _globalConfig.Init(_processQuerierStrategy.CPUArchitechture, dumpStrategy.SystemInfo);
        }

        private ProcessAnalyzer(DataTarget dataTarget, ClrRuntime runtime)
        {
            _debugClient = dataTarget.DebuggerInterface;
            _dataReader = dataTarget.DataReader;
            _runtime = runtime;
            _globalConfig = Config.GetInstance();
            _isDisposed = false;
        }

        #region Members

        private readonly ProcessQuerierStrategy _processQuerierStrategy;
        private readonly Config _globalConfig;
        private readonly Stopwatch _operationTime = new Stopwatch();

        private readonly IDebugClient _debugClient;
        private readonly IDataReader _dataReader;
        private readonly ClrRuntime _runtime;
        private readonly uint _pid;

        private bool _isDisposed;

        #endregion


        public async Task<ProcessAnalysisResult> Handle()
        {
            _operationTime.Start();

            uint _numThreads = 0;
            Util.VerifyHr(((IDebugSystemObjects)_debugClient).GetNumberThreads(out _numThreads));

            List<UnifiedThread> result = new List<UnifiedThread>();

            for (uint threadIdx = 0; threadIdx < _numThreads; ++threadIdx)
            {
                result.Add(await GetUnifiedThread(threadIdx));
            }

            _operationTime.Stop();

            return new ProcessAnalysisResult()
            {
                ElapsedMilliseconds = _operationTime.ElapsedMilliseconds,
                Threads = result
            };
        }

        private async Task<UnifiedThread> GetUnifiedThread(uint threadIdx)
        {
            UnifiedThread result = null;

            ThreadInfo specific_info = GetThreadInfo(threadIdx);
            _processQuerierStrategy.SetThreadContext(specific_info);

            try
            {
                if (specific_info.IsManagedThread)
                {
                    result = HandleManagedThread(specific_info);
                }
                else
                {
                    result = await HandleUnManagedThread(specific_info);
                }
            }
            catch (Exception e)
            {
                LogHandler.Log(e.ToString(), LOG_LEVELS.INFO);
            }

            return result;
        }

        #region Thread Method

        private async Task<UnifiedUnManagedThread> HandleUnManagedThread(ThreadInfo specific_info)
        {
            UnifiedUnManagedThread result = null;
            var unmanagedStack = GetNativeStackTrace(specific_info);

            var blockingObjects = await _processQuerierStrategy.GetUnmanagedBlockingObjects(specific_info, unmanagedStack, _runtime);

            result = new UnifiedUnManagedThread(specific_info, unmanagedStack, blockingObjects);

            return result;
        }

        private UnifiedManagedThread HandleManagedThread(ThreadInfo specific_info)
        {
            UnifiedManagedThread result = null;

            ClrThread clr_thread = _runtime.Threads.Where(x => x.OSThreadId == specific_info.OSThreadId).FirstOrDefault();
            if (clr_thread != null)
            {
                var managedStack = GetManagedStackTrace(clr_thread, specific_info);
                List<UnifiedStackFrame> unmanagedStack = null;

                unmanagedStack = GetNativeStackTrace(specific_info);

                var blockingObjs = _processQuerierStrategy.GetManagedBlockingObjects(clr_thread, unmanagedStack, _runtime);

                result = new UnifiedManagedThread(specific_info, managedStack, unmanagedStack, blockingObjs);

            }
            return result;
        }

        private ThreadInfo GetThreadInfo(uint threadIndex)
        {
            uint[] engineThreadIds = new uint[1];
            uint[] osThreadIds = new uint[1];
            Util.VerifyHr(((IDebugSystemObjects)_debugClient).GetThreadIdsByIndex(threadIndex, 1, engineThreadIds, osThreadIds));
            ClrThread managedThread = _runtime.Threads.FirstOrDefault(thread => thread.OSThreadId == osThreadIds[0]);
            return new ThreadInfo
            {
                Index = threadIndex,
                EngineThreadId = engineThreadIds[0],
                OSThreadId = osThreadIds[0],
                ManagedThread = managedThread
            };
        }

        #endregion


        #region StackTrace

        private List<UnifiedStackFrame> GetManagedStackTrace(ClrThread thread, ThreadInfo info)
        {
            return (from frame in thread.StackTrace
                    let sourceLocation = SymbolCache.GetFileAndLineNumberSafe(frame)
                    select _processQuerierStrategy.ConvertToUnified(frame, sourceLocation, info)
                    ).ToList();
        }

        private unsafe List<UnifiedStackFrame> GetNativeStackTrace(ThreadInfo info)
        {
            Util.VerifyHr(((IDebugSystemObjects)_debugClient).SetCurrentThreadId(info.EngineThreadId));

            DEBUG_STACK_FRAME[] stackFrames = new DEBUG_STACK_FRAME[400];
            uint framesFilled;

            Util.VerifyHr(((IDebugControl)_debugClient).GetStackTrace(0, 0, 0,
                            stackFrames, Marshal.SizeOf<DEBUG_STACK_FRAME>(), out framesFilled));

            var stackTrace = _processQuerierStrategy.ConvertToUnified(stackFrames.Take((int)framesFilled), _runtime, info, _pid);
            return stackTrace;
        }

        #endregion

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _processQuerierStrategy.Dispose();
            }
        }
    }
}

