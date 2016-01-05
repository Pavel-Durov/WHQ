using Assignments.Core.Model.StackFrames;
using Assignments.Core.Model.Unified.Thread;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.msos;
using Microsoft.Diagnostics.Runtime;
using Assignments.Core.Handlers.WCT;
using Assignments.Core.Handlers;
using Assignments.Core.Model.WCT;

namespace Assignments.Core.Model.Unified
{
    public class UnifiedResult
    {
        public UnifiedResult(ClrThread thread, List<UnifiedStackFrame> managedStack, List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime)
        {
            _wctApi = new WctApiHandler();
            Thread = new UnifiedManagedThread(thread);
            StackTrace = new List<UnifiedStackFrame>(managedStack);

            StackTrace.AddRange(unmanagedStack);
            BlockingObjects = GetBlockingObjects(thread, runtime);
        }


        WctApiHandler _wctApi;

        public UnifiedThread Thread { get; private set; }
        public List<UnifiedStackFrame> StackTrace { get; private set; }
        public List<UnifiedBlockingObject> BlockingObjects { get; private set; }



        private List<UnifiedBlockingObject> GetBlockingObjects(ClrThread thread, ClrRuntime runtime)
        {
            List<UnifiedBlockingObject> result = new List<UnifiedBlockingObject>();

            //Clr Blocking Objects
            var clr_blockingObjects = GetClrBlockingObjects(thread, runtime);
            if (clr_blockingObjects != null)
            {
                result.AddRange(clr_blockingObjects);
            }
            //WCT API Blocking Objects
            var wct_blockingObjects = GetWCTBlockingObject(thread);

            if (wct_blockingObjects != null)
            {
                result.AddRange(wct_blockingObjects);
            }

            return result;
        }

        private List<UnifiedBlockingObject> GetWCTBlockingObject(ClrThread thread)
        {
            List<UnifiedBlockingObject> result = null;

            ThreadWCTInfo wct_threadInfo = null;
            if (_wctApi.GetBlockingObjects(thread, out wct_threadInfo))
            {
                result = new List<UnifiedBlockingObject>();

                var wctThreadInfo = _wctApi.CollectWaitInformation(thread);
                if (wctThreadInfo.WctBlockingObjects?.Count > 0)
                {
                    foreach (var blockingObj in wctThreadInfo.WctBlockingObjects)
                    {
                        result.Add(new UnifiedBlockingObject(blockingObj));
                    }
                }
            }

            return result;
        }

        private List<UnifiedBlockingObject> GetClrBlockingObjects(ClrThread thread, ClrRuntime runtime)
        {
            List<UnifiedBlockingObject> result = null;
            if (thread.BlockingObjects?.Count > 0)
            {
                // ClrHeap heap = runtime.GetHeap();
                result = new List<UnifiedBlockingObject>();

                foreach (var item in thread.BlockingObjects)
                {
                    //ClrType type = heap.GetObjectType(item.Object);
                    result.Add(new UnifiedBlockingObject(item));//, type.Name));
                }
            }
            return result;
        }
    }
}
