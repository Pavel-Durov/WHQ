using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.msos;
using Microsoft.Diagnostics.Runtime;

namespace Assignments.Core.Model.ThreadStack
{
    public class AnalyzedManagedStack : AnalyzedStack
    {
        internal override void Fill(IEnumerable<UnifiedStackFrame> stackTrace, ClrRuntime runtime, ClrThread thread)
        {

            if (thread.BlockingObjects != null && thread?.BlockingObjects?.Count > 0)
            {
                foreach (var bobj in thread.BlockingObjects)
                {
                    //print(bobj);
                }
            }

            foreach (var frame in stackTrace)
            {
                base.Add(frame);
            }
        }
    }
}
