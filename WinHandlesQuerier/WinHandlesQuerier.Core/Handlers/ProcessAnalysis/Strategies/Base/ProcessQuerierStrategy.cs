using WinHandlesQuerier.Core.Model.Unified;
using WinHandlesQuerier.Core.msos;
using Microsoft.Diagnostics.Runtime;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.Diagnostics.Runtime.Interop;
using Assignments.Core.Handlers.UnmanagedStackFrame.Strategies.Base;
using Assignments.Core.Handlers.UnmanagedStackFrame.Strategies;

namespace WinHandlesQuerier.Core.Handlers.StackAnalysis.Strategies
{
    public abstract class ProcessQuerierStrategy
    {
        public ProcessQuerierStrategy(IDebugClient debugClient, IDataReader dataReader, ClrRuntime runtime)
        {
            _runtime = runtime;
            _dataReader = dataReader;
            _debugClient = debugClient;

            if (Environment.Is64BitProcess)
            {
                //TODO: 64bit Logic Implement 
                _unmanagedStackWalkerStrategy = new Unmanaged_x64_StackWalkerStrategy((IDebugAdvanced)debugClient, _dataReader, runtime);
            }
            else
            {
                _unmanagedStackWalkerStrategy = new Unmanaged_x86_StackWalkerStrategy();
            }
        }

        IDataReader _dataReader;
        IDebugClient _debugClient;
        ClrRuntime _runtime;
        UnmanagedStackWalkerStrategy _unmanagedStackWalkerStrategy;

        public virtual List<UnifiedBlockingObject> GetManagedBlockingObjects(ClrThread thread, List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime)
        {
            List<UnifiedBlockingObject> result = new List<UnifiedBlockingObject>();
            if (thread.BlockingObjects?.Count > 0)
            {
                foreach (var item in thread.BlockingObjects)
                {
                    result.Add(new UnifiedBlockingObject(item));
                }
            }

            CheckForCriticalSections(result, unmanagedStack, runtime);

            foreach (var frame in unmanagedStack)
            {
                if(frame?.Handles?.Count > 0)
                {
                    foreach (var handle in frame.Handles)
                    {
                        result.Add(new UnifiedBlockingObject(handle.Id, handle.ObjectName, handle.Type));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Walks the given stackFrameList and checks if it's contains CRITICAL_SECTION calls
        /// </summary>
        protected void CheckForCriticalSections(List<UnifiedBlockingObject> list, List<UnifiedStackFrame> stack, ClrRuntime runtime)
        {
            var criticalSectionObjects = GetCriticalSections(stack, runtime);

            if (criticalSectionObjects.Any())
            {
                if (list == null)
                    list = new List<UnifiedBlockingObject>();

                list.AddRange(criticalSectionObjects);
            }
        }

        protected List<UnifiedBlockingObject> GetUnmanagedBlockingObjects(List<UnifiedStackFrame> unmanagedStack)
        {
            List<UnifiedBlockingObject> result = new List<UnifiedBlockingObject>();

            var framesWithHandles = from c in unmanagedStack
                                    where c.Handles?.Count > 0
                                    select c;

            foreach (var frame in framesWithHandles)
            {
                foreach (var handle in frame.Handles)
                {
                    result.Add(new UnifiedBlockingObject(handle.Id, handle.ObjectName, handle.Type));
                }
            }

            return result;
        }

        public virtual IEnumerable<UnifiedBlockingObject> GetCriticalSections(List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime)
        {
            foreach (var item in unmanagedStack)
            {
                UnifiedBlockingObject blockObject;

                if (_unmanagedStackWalkerStrategy.CheckForCriticalSectionCalls(item, runtime, out blockObject))
                {
                    yield return blockObject;
                }
            }
        }

        public abstract List<UnifiedBlockingObject> GetUnmanagedBlockingObjects(ThreadInfo thread, List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime);

        internal List<UnifiedStackFrame> ConvertToUnified(DEBUG_STACK_FRAME[] stackFrames, uint framesFilled, ClrRuntime _runtime, uint threadPtr, uint pID)
        {
            return _unmanagedStackWalkerStrategy.ConvertToUnified(stackFrames, framesFilled, _runtime, _debugClient, threadPtr, pID);
        }
    }
}
