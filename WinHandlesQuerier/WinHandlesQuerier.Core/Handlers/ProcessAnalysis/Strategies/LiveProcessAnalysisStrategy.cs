using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHandlesQuerier.Core.Model.Unified;
using WinHandlesQuerier.Core.Model.Unified.Thread;
using WinHandlesQuerier.Core.msos;
using Microsoft.Diagnostics.Runtime;
using WinHandlesQuerier.Core.Model.WCT;
using WinHandlesQuerier.Core.Exceptions;

namespace WinHandlesQuerier.Core.Handlers.StackAnalysis.Strategies
{
    public class LiveProcessAnalysisStrategy : ProcessAnalysisStrategy
    {
        public LiveProcessAnalysisStrategy(uint pid) 
        {
            _wctApi = new WctApiHandler();
        }

        WctApiHandler _wctApi;


        public override List<UnifiedBlockingObject> GetUnmanagedBlockingObjects(ThreadInfo thread, List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime)
        {
            List<UnifiedBlockingObject> result = null;

            ThreadWCTInfo wct_threadInfo = null;
            if (_wctApi.GetBlockingObjects(thread.OSThreadId, out wct_threadInfo))
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
            
            return result;
        }
    }
}
