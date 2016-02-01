using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.Model.Unified;
using Assignments.Core.Model.Unified.Thread;
using Assignments.Core.msos;
using Microsoft.Diagnostics.Runtime;
using Assignments.Core.Model.WCT;

namespace Assignments.Core.Handlers.StackAnalysis.Strategies
{
    public class StackAnalysisLiveProcessStrategy : StackAnalysisStrategy
    {
        public StackAnalysisLiveProcessStrategy()
        {
            _wctApi = new WctApiHandler();
        }
        public override List<UnifiedBlockingObject> GetBlockingObjects(ClrThread thread)
        {
            return GetWCTBlockingObject(thread.OSThreadId);
        }

        public override UnifiedUnManagedThread HandleUnManagedThread(ThreadInfo info)
        {
            throw new NotImplementedException();
        }

        WctApiHandler _wctApi;

        private List<UnifiedBlockingObject> GetWCTBlockingObject(uint threadId)
        {
            List<UnifiedBlockingObject> result = null;

            ThreadWCTInfo wct_threadInfo = null;
            if (_wctApi.GetBlockingObjects(threadId, out wct_threadInfo))
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
