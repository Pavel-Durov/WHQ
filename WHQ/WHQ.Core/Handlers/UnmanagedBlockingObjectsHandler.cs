using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHQ.Core.Model.Unified;
using WHQ.Core.Handlers.UnmanagedStackFrame.Strategies.Base;
using WHQ.Core.Model.WCT;
using WHQ.Core.msos;
using WHQ.Core.Model.MiniDump;
using WHQ.Providers.ClrMd.Model;

namespace WHQ.Core.Handlers
{
    internal class UnmanagedBlockingObjectsHandler
    {
        public UnmanagedBlockingObjectsHandler(UnmanagedStackWalkerStrategy strategy)
        {
            _unmanagedStackWalkerStrategy = strategy;
        }

        UnmanagedStackWalkerStrategy _unmanagedStackWalkerStrategy;

        internal List<UnifiedBlockingObject> GetManagedBlockingObjects(ClrThread thread, List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime)
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
                if (frame?.Handles?.Count > 0)
                {
                    foreach (var handle in frame.Handles)
                    {
                        result.Add(new UnifiedBlockingObject(handle.Id, handle.ObjectName, handle.Type));
                    }
                }
            }
            return result;
        }

        internal List<UnifiedBlockingObject> GetUnmanagedBlockingObjects(ThreadWCTInfo wct_threadInfo, List<UnifiedStackFrame> unmanagedStack)
        {
            List<UnifiedBlockingObject> result = null;

            if (wct_threadInfo?.WctBlockingObjects.Count > 0)
            {
                result = new List<UnifiedBlockingObject>();

                if (wct_threadInfo.WctBlockingObjects?.Count > 0)
                {
                    foreach (var blockingObj in wct_threadInfo.WctBlockingObjects)
                    {
                        result.Add(new UnifiedBlockingObject(blockingObj));
                    }
                }
            }

            result.AddRange(GetUnmanagedBlockingObjects(unmanagedStack));

            return result;
        }

        internal List<UnifiedBlockingObject> GetUnmanagedBlockingObjects(ThreadInfo thread, List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime, List<MiniDumpHandle> DumpHandles)
        {

            List<UnifiedBlockingObject> result = new List<UnifiedBlockingObject>();

            result.AddRange(GetUnmanagedBlockingObjects(unmanagedStack));

            foreach (var item in DumpHandles)
            {
                result.Add(new UnifiedBlockingObject(item));
            }

            CheckForCriticalSections(result, unmanagedStack, runtime);

            return result;
        }

        internal void CheckForCriticalSections(List<UnifiedBlockingObject> list, List<UnifiedStackFrame> stack, ClrRuntime runtime)
        {
            var criticalSectionObjects = GetCriticalSectionBlockingObjects(stack, runtime);

            if (criticalSectionObjects.Any())
            {
                if (list == null)
                    list = new List<UnifiedBlockingObject>();

                list.AddRange(criticalSectionObjects);
            }
        }

        public virtual IEnumerable<UnifiedBlockingObject> GetCriticalSectionBlockingObjects(List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime)
        {
            foreach (var item in unmanagedStack)
            {
                UnifiedBlockingObject blockObject;

                if (_unmanagedStackWalkerStrategy.GetCriticalSectionBlockingObject(item, runtime, out blockObject))
                {
                    yield return blockObject;
                }
            }
        }

        internal List<UnifiedBlockingObject> GetUnmanagedBlockingObjects(List<UnifiedStackFrame> unmanagedStack)
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


    }
}
