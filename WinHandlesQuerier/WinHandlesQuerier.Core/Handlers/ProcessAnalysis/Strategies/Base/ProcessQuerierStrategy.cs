using WinHandlesQuerier.Core.Model.Unified;
using WinHandlesQuerier.Core.msos;
using Microsoft.Diagnostics.Runtime;
using System.Collections.Generic;
using System;
using Microsoft.Diagnostics.Runtime.Interop;
using WinHandlesQuerier.Core.Handlers.UnmanagedStackFrame.Strategies.Base;
using WinHandlesQuerier.Core.Handlers.UnmanagedStackFrame.Strategies;
using WinHandlesQuerier.Core.Handlers.ThreadContext.Strategies;
using System.Threading.Tasks;

namespace WinHandlesQuerier.Core.Handlers.StackAnalysis.Strategies
{
    public enum CPUArchitecture { x86, x64 }

    internal abstract class ProcessQuerierStrategy : IDisposable
    {
        public ProcessQuerierStrategy(IDebugClient debugClient, IDataReader dataReader, ClrRuntime runtime)
        {
            _isDispsed = false;

            _runtime = runtime;
            _dataReader = dataReader;
            _debugClient = debugClient;

            if (dataReader.GetArchitecture() == Architecture.Amd64)//Environment.Is64BitProcess
            {
                _unmanagedStackWalkerStrategy = new Unmanaged_x64_StackWalkerStrategy(_runtime);
                _threadContextStrategy = new ThreadContext_x64_Strategy();
            }
            else
            {
                _unmanagedStackWalkerStrategy = new Unmanaged_x86_StackWalkerStrategy();
                _threadContextStrategy = new ThreadContext_x86_Strategy();
            }

            _unmanagedBlockingObjectsHandler = new UnmanagedBlockingObjectsHandler(_unmanagedStackWalkerStrategy);
        }

        #region Members

        internal IDataReader _dataReader;
        internal IDebugClient _debugClient;
        internal ClrRuntime _runtime;
        internal UnmanagedStackWalkerStrategy _unmanagedStackWalkerStrategy;
        internal ThreadContextStrategy _threadContextStrategy;

        protected UnmanagedBlockingObjectsHandler _unmanagedBlockingObjectsHandler;
        protected bool _isDispsed;

        #endregion

        public abstract CPUArchitecture CPUArchitechture { get; }
        public abstract void Dispose();
        public abstract Task<List<UnifiedBlockingObject>> GetUnmanagedBlockingObjects(ThreadInfo thread, List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime);

        public virtual List<UnifiedBlockingObject> GetManagedBlockingObjects(ClrThread thread, List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime)
        {
            return _unmanagedBlockingObjectsHandler.GetManagedBlockingObjects(thread, unmanagedStack, runtime);
        }

        public void SetThreadContext(ThreadInfo specific_info)
        {
            _threadContextStrategy.SetThreadContext(specific_info, (IDebugAdvanced)_debugClient, _dataReader);
        }

        public virtual IEnumerable<UnifiedBlockingObject> GetCriticalSectionBlockingObjects(List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime)
        {
            return _unmanagedBlockingObjectsHandler.GetCriticalSectionBlockingObjects(unmanagedStack, runtime);
        }


        protected List<UnifiedBlockingObject> GetUnmanagedBlockingObjects(List<UnifiedStackFrame> unmanagedStack)
        {
            return _unmanagedBlockingObjectsHandler.GetUnmanagedBlockingObjects(unmanagedStack);
        }

        public List<UnifiedStackFrame> ConvertToUnified(IEnumerable<DEBUG_STACK_FRAME> stackFrames, ClrRuntime runtime, ThreadInfo info, uint pID)
        {
            var result = _unmanagedStackWalkerStrategy.ConvertToUnified(stackFrames, runtime, _debugClient, info, pID);
            return result;
        }


        public UnifiedStackFrame ConvertToUnified(ClrStackFrame frame, SourceLocation sourceLocation, ThreadInfo info)
        {
            var result = new UnifiedStackFrame(frame, sourceLocation);
            result.ThreadContext = info.ContextStruct;

            return result;
        }
    }
}
