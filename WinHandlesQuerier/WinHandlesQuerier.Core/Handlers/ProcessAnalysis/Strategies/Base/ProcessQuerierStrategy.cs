using WinHandlesQuerier.Core.Model.Unified;
using WinHandlesQuerier.Core.msos;
using Microsoft.Diagnostics.Runtime;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.Diagnostics.Runtime.Interop;
using WinHandlesQuerier.Core.Handlers.UnmanagedStackFrame.Strategies.Base;
using WinHandlesQuerier.Core.Handlers.UnmanagedStackFrame.Strategies;
using WinNativeApi;
using System.Runtime.InteropServices;
using WinNativeApi.WinNT;
using WinHandlesQuerier.Core;
using WinHandlesQuerier.Core.Handlers.ThreadContext.Strategies;
using WinHandlesQuerier.Core.Handlers;

namespace WinHandlesQuerier.Core.Handlers.StackAnalysis.Strategies
{
    public enum CPUArchitecture { x86, x64 }

    internal abstract class ProcessQuerierStrategy
    {
        public ProcessQuerierStrategy(IDebugClient debugClient, IDataReader dataReader, ClrRuntime runtime)
        {
            _runtime = runtime;
            _dataReader = dataReader;
            _debugClient = debugClient;

            if (dataReader.GetArchitecture() == Architecture.Amd64)//Environment.Is64BitProcess
            {
                _unmanagedStackWalkerStrategy = new Unmanaged_x64_StackWalkerStrategy();
                _threadContextStrategy = new ThreadContext_x64_Strategy();
            }
            else
            {
                _unmanagedStackWalkerStrategy = new Unmanaged_x86_StackWalkerStrategy();
                _threadContextStrategy = new ThreadContext_x86_Strategy();
            }

            _unmanagedBlockingObjectsHandler = new UnmanagedBlockingObjectsHandler(_unmanagedStackWalkerStrategy);
        }

        internal IDataReader _dataReader;
        internal IDebugClient _debugClient;
        internal ClrRuntime _runtime;
        internal UnmanagedStackWalkerStrategy _unmanagedStackWalkerStrategy;
        internal ThreadContextStrategy _threadContextStrategy;
        protected UnmanagedBlockingObjectsHandler _unmanagedBlockingObjectsHandler;

        public abstract CPUArchitecture CPUArchitechture { get; }

        public virtual List<UnifiedBlockingObject> GetManagedBlockingObjects(ClrThread thread, List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime)
        {
            return _unmanagedBlockingObjectsHandler.GetManagedBlockingObjects(thread, unmanagedStack, runtime);
        }

        internal void GetThreadContext(ThreadInfo specific_info)
        {
            _threadContextStrategy.GetThreadContext(specific_info, (IDebugAdvanced)_debugClient, _dataReader);
        }

        public virtual IEnumerable<UnifiedBlockingObject> GetCriticalSectionBlockingObjects(List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime)
        {
            return _unmanagedBlockingObjectsHandler.GetCriticalSectionBlockingObjects(unmanagedStack, runtime);
        }


        protected List<UnifiedBlockingObject> GetUnmanagedBlockingObjects(List<UnifiedStackFrame> unmanagedStack)
        {
            return _unmanagedBlockingObjectsHandler.GetUnmanagedBlockingObjects(unmanagedStack);
        }

        public abstract List<UnifiedBlockingObject> GetUnmanagedBlockingObjects(ThreadInfo thread, List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime);



        internal List<UnifiedStackFrame> ConvertToUnified(IEnumerable<DEBUG_STACK_FRAME> stackFrames, ClrRuntime runtime, ThreadInfo info, uint pID)
        {
            var result = _unmanagedStackWalkerStrategy.ConvertToUnified(stackFrames, runtime, _debugClient, info, pID);
            return result;
        }


        internal UnifiedStackFrame ConvertToUnified(ClrStackFrame frame, SourceLocation sourceLocation, ThreadInfo info)
        {
            var result = new UnifiedStackFrame(frame, sourceLocation);
            result.ThreadContext = info.ContextStruct;

            return result;
        }
    }
}
