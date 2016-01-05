using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignments.Core.Model.Unified
{
    /// <summary>
    /// (1 per thread) UnifiedThread
    /// </summary>
    public class UnifiedStackTrace
    {
        private List<UnifiedStackFrame> managedStack;

        public UnifiedStackTrace(List<UnifiedStackFrame> managedStack)
        {
            this.managedStack = managedStack;
        }
    }
}
