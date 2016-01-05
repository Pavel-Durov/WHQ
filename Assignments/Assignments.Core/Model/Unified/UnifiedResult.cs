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

        public UnifiedResult()
        {
            Data = new Dictionary<uint, AnalyzedThreadStack>();
        }

        WctApiHandler _wctApi;


        public WctApiHandler WctApi
        {
            get
            {
                if (_wctApi == null)
                {
                    _wctApi = new WctApiHandler();
                }
                return _wctApi;
            }
            set { _wctApi = value; }
        }


        public Dictionary<uint, AnalyzedThreadStack> Data { get; set; }

        internal void Add(ThreadInfo info)
        {
            if (info.IsManagedThread)
            {
                //TODO: Set real data
                UnifiedThread thread = new UnifiedManagedThread(info);
                Data[info.OSThreadId] = null;
            }
            else
            {
                //TODO: Set real data
                UnifiedThread thread = new UnifiedManagedThread(info);
                Data[info.OSThreadId] = null;
            }
        }

        internal void Add(ClrThread thread, List<UnifiedStackFrame> managedStack, List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime)
        {
            //UnifiedThread (1 per thread)
            UnifiedThread unified_thread = new UnifiedManagedThread(thread);


            var list = new List<UnifiedStackFrame>(managedStack);
            list.AddRange(unmanagedStack);

            //UnifiedStackTrace (1 per thread)
            UnifiedStackTrace treace = new UnifiedStackTrace(list);

            //Clr Blocking Objects
            List<UnifiedBlockingObject> clr_blockingObjects = GetClrBlockingObjects(thread, runtime);

            //WCT API Blocking Objects
            List<UnifiedBlockingObject> wct_blockingObjects = GetWCTBlockingObject(thread);





            //var wctThreadInfo = WctApi.CollectWaitInformation(thread);


            //var nativeStackList = UnmanagedStackFrameHandler.Analyze(unmanagedStack, runtime, thread);
            //var analyzed =  new AnalyzedThreadStack(thread, wctThreadInfo, managedStack, nativeStackList);

        }

        private List<UnifiedBlockingObject> GetWCTBlockingObject(ClrThread thread)
        {
            List<UnifiedBlockingObject> result = null;

            ThreadWCTInfo wct_threadInfo = null;
            if (WctApi.GetBlockingObjects(thread, out wct_threadInfo))
            {
                result = new List<UnifiedBlockingObject>();

                var wctThreadInfo = WctApi.CollectWaitInformation(thread);
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
