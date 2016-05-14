using System.Collections.Generic;
using WinHandlesQuerier.Core.Model.Unified;
using WinHandlesQuerier.Core.msos;
using Microsoft.Diagnostics.Runtime;
using WinHandlesQuerier.Core.Model.WCT;

namespace WinHandlesQuerier.Core.Handlers.StackAnalysis.Strategies
{
    public class LiveProcessQuerierStrategy : ProcessQuerierStrategy
    {
        public LiveProcessQuerierStrategy(uint pid)
        {
            _wctApi = new WctHandler();
        }

        WctHandler _wctApi;


        public override List<UnifiedBlockingObject> GetUnmanagedBlockingObjects(ThreadInfo thread, List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime)
        {
            List<UnifiedBlockingObject> result = null;

            ThreadWCTInfo wct_threadInfo = _wctApi.GetBlockingObjects(thread.OSThreadId);

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

            result.AddRange(base.GetUnmanagedBlockingObjects(unmanagedStack));

            return result;
        }
    }
}
