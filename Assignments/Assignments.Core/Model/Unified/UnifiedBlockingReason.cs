using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignments.Core.Model.Unified
{
    /// <summary>
    /// The UnifiedBlockingReason enumeration would have everything in CLRMD’s BlockingReason, plus values from WCT_OBJECT_TYPE (mutex, critical section, and so on). Note that if there’s a thread that has CLRMD wait information, the same information may also appear in the WCT output, so we might need to think about de-duplication. We’ll think about it when you get there.
    /// </summary>
    class UnifiedBlockingReason
    {
    }
}
