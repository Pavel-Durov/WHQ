using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHQ.Core.Providers.ClrMd.Model
{
    interface IDebugControl
    {
        int GetStackTrace(int v1, int v2, int v3, DEBUG_STACK_FRAME[] stackFrames, int v4, out uint framesFilled);
    }
}
