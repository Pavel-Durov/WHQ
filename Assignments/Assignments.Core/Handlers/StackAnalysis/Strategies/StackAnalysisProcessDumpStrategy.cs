using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.Model.Unified;
using Assignments.Core.Model.Unified.Thread;
using Assignments.Core.msos;
using Microsoft.Diagnostics.Runtime;

namespace Assignments.Core.Handlers.StackAnalysis.Strategies
{
    public class StackAnalysisProcessDumpStrategy : StackAnalysisStrategy
    {
        public override List<UnifiedBlockingObject> GetBlockingObjects(ClrThread thread)
        {
            throw new NotImplementedException();
        }

        public override UnifiedUnManagedThread HandleUnManagedThread(ThreadInfo info)
        {
            throw new NotImplementedException();
        }
    }
}
