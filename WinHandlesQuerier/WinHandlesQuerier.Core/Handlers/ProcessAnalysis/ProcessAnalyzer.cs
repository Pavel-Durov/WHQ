using Microsoft.Diagnostics.Runtime;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Diagnostics.Runtime.Interop;
using WinHandlesQuerier.Core.msos;
using WinHandlesQuerier.Core.Model.Unified;
using System;
using WinHandlesQuerier.Core.Model.Unified.Thread;
using WinHandlesQuerier.Core.Handlers.StackAnalysis.Strategies;
using Kernel32;
using System.Runtime.InteropServices;
using WinNativeApi.WinNT;
using WinNativeApi;

namespace WinHandlesQuerier.Core.Handlers
{
    public class ProcessAnalyzer
    {
        /// <summary>
        /// Used for live process analysis
        /// </summary>
        public ProcessAnalyzer(DataTarget dataTarget, ClrRuntime runtime, uint pid) : this(dataTarget, runtime)
        {
            PID = pid;
            _blockingObjectsFetchingStrategy = new LiveProcessQuerierStrategy(_debugClient, _dataReader, runtime);
        }

        /// <summary>
        /// Used for dump file analysis
        /// </summary>
        public ProcessAnalyzer(DataTarget dataTarget, ClrRuntime runtime, string pathToDumpFile) : this(dataTarget, runtime)
        {
            _blockingObjectsFetchingStrategy = new DumpFileQuerierStrategy(pathToDumpFile, _runtime, _debugClient, _dataReader);
        }

        private ProcessAnalyzer(DataTarget dataTarget, ClrRuntime runtime)
        {
            _debugClient = dataTarget.DebuggerInterface;
            _dataReader = dataTarget.DataReader;
            _runtime = runtime;
        }

        #region Members

        public bool IsLiveProcess { get { return _blockingObjectsFetchingStrategy is LiveProcessQuerierStrategy; } }
        ProcessQuerierStrategy _blockingObjectsFetchingStrategy;

        IDebugClient _debugClient;
        IDataReader _dataReader;
        private ClrRuntime _runtime;
        private DataTarget dataTarget;
        private ClrRuntime runtime;

        public uint PID { get; private set; }
        #endregion

        public List<UnifiedThread> Handle()
        {
            uint _numThreads = 0;
            Util.VerifyHr(((IDebugSystemObjects)_debugClient).GetNumberThreads(out _numThreads));

            List<UnifiedThread> result = new List<UnifiedThread>();

            for (uint threadIdx = 0; threadIdx < _numThreads; ++threadIdx)
            {
                ThreadInfo specific_info = GetThreadInfo(threadIdx);
                GetThreadContext(specific_info);

                if (specific_info.IsManagedThread)
                {
                    result.Add(HandleManagedThread(specific_info));
                }
                else
                {
                    result.Add(HandleUnManagedThread(specific_info));
                }
            }

            return result;
        }

        #region Thread Method

        
        public unsafe bool GetThreadContext(ThreadInfo threadInfo)
        {
            bool result = false;

            byte[] contextBytes = new byte[GetThreadContextSize()];

            if (SetCurrentThreadId(threadInfo.EngineThreadId) == DbgEng.S_OK)
            {
                var gch = GCHandle.Alloc(contextBytes, GCHandleType.Pinned);

                var h_result = ((IDebugAdvanced)_debugClient).GetThreadContext(gch.AddrOfPinnedObject(), (uint)contextBytes.Length);

                if (h_result == DbgEng.S_OK)
                {
                    try
                    {
                        threadInfo.ContextStruct = Marshal.PtrToStructure<CONTEXT_AMD64>(gch.AddrOfPinnedObject());
                        result = true;
                    }
                    finally
                    {
                        gch.Free();
                    }
                }
            }

            return result;
        }

        private uint SetCurrentThreadId(uint engineThreadId)
        {
            IDebugSystemObjects sytemObject = ((IDebugSystemObjects)_debugClient);
            uint set_h_result = (uint)sytemObject.SetCurrentThreadId(engineThreadId);

            if (set_h_result == DbgEng.E_NOINTERFACE)
            {
                throw new InvalidOperationException("No thread with the specified ID was found.");
            }
            return set_h_result;
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

        private UnifiedUnManagedThread HandleUnManagedThread(ThreadInfo specific_info)
        {
            UnifiedUnManagedThread result = null;
            var unmanagedStack = GetNativeStackTrace(specific_info);

            var blockingObjects = _blockingObjectsFetchingStrategy.GetUnmanagedBlockingObjects(specific_info, unmanagedStack, _runtime);

            result = new UnifiedUnManagedThread(specific_info, unmanagedStack, blockingObjects);

            return result;
        }

        private UnifiedManagedThread HandleManagedThread(ThreadInfo specific_info)
        {
            UnifiedManagedThread result = null;

            ClrThread clr_thread = _runtime.Threads.Where(x => x.OSThreadId == specific_info.OSThreadId).FirstOrDefault();
            if (clr_thread != null)
            {
                var managedStack = GetManagedStackTrace(clr_thread);
                List<UnifiedStackFrame> unmanagedStack = null;

                try
                {
                    unmanagedStack = GetNativeStackTrace(specific_info);
                }
                catch (Exception e)
                {
                    LogHandler.Log(e.ToString(), LOG_LEVELS.INFO);
                }


                var blockingObjs = _blockingObjectsFetchingStrategy.GetManagedBlockingObjects(clr_thread, unmanagedStack, _runtime);

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

        public List<UnifiedStackFrame> GetStackTrace(uint threadIndex)
        {
            ThreadInfo threadInfo = GetThreadInfo(threadIndex);
            List<UnifiedStackFrame> unifiedStackTrace = new List<UnifiedStackFrame>();
            List<UnifiedStackFrame> nativeStackTrace = GetNativeStackTrace(threadInfo);
            if (threadInfo.IsManagedThread)
            {
                List<UnifiedStackFrame> managedStackTrace = GetManagedStackTrace(threadInfo.ManagedThread);
                int managedFrame = 0;
                for (int nativeFrame = 0; nativeFrame < nativeStackTrace.Count; ++nativeFrame)
                {
                    bool found = false;
                    for (int temp = managedFrame; temp < managedStackTrace.Count; ++temp)
                    {
                        if (nativeStackTrace[nativeFrame].InstructionPointer == managedStackTrace[temp].InstructionPointer)
                        {
                            managedStackTrace[temp].LinkedStackFrame = nativeStackTrace[nativeFrame];
                            unifiedStackTrace.Add(managedStackTrace[temp]);
                            managedFrame = temp + 1;
                            found = true;
                            break;
                        }
                        else if (managedFrame > 0)
                        {
                            // We have already seen at least one managed frame, and we're about
                            // to skip a managed frame because we didn't find a matching native
                            // frame. In this case, add the managed frame into the stack anyway.
                            unifiedStackTrace.Add(managedStackTrace[temp]);
                            managedFrame = temp + 1;
                            found = true;
                            break;
                        }
                    }
                    // We didn't find a matching managed frame, so add the native frame directly.
                    if (!found)
                        unifiedStackTrace.Add(nativeStackTrace[nativeFrame]);
                }
            }
            else
            {
                return nativeStackTrace;
            }
            return unifiedStackTrace;
        }

        private List<UnifiedStackFrame> GetManagedStackTrace(ClrThread thread)
        {
            return (from frame in thread.StackTrace
                    let sourceLocation = SymbolCache.GetFileAndLineNumberSafe(frame)
                    select new UnifiedStackFrame(frame, sourceLocation, (uint)thread.ManagedThreadId)
                    ).ToList();
        }

        private unsafe List<UnifiedStackFrame> GetNativeStackTrace(ThreadInfo info)
        {
            Util.VerifyHr(((IDebugSystemObjects)_debugClient).SetCurrentThreadId(info.EngineThreadId));

            DEBUG_STACK_FRAME[] stackFrames = new DEBUG_STACK_FRAME[200];
            uint framesFilled;
            Util.VerifyHr(((IDebugControl)_debugClient).GetStackTrace(0, 0, 0, stackFrames, stackFrames.Length, out framesFilled));

            var stackTrace = _blockingObjectsFetchingStrategy.ConvertToUnified(stackFrames, framesFilled, _runtime, info.OSThreadId, PID);
            return stackTrace;
        }

        #endregion
    }
}

