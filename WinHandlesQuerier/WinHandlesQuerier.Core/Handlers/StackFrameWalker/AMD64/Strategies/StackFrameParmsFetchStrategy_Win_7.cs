using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHandlesQuerier.Core.Model.Unified;

namespace WinHandlesQuerier.Core.Handlers.UnmanagedStackFrameWalker.AMD64
{
    class StackFrameParmsFetchStrategy_Win_7 : StackFrameParmsFetchStrategy
    {
        internal override Params GetenterCriticalSectionParam(UnifiedStackFrame frame)
        {
            Params result = new Params();

            throw new NotImplementedException("This Stretegy is under construction...");

            return result;
        }

        internal override Params GetWaitForMultipleObjectsParams(UnifiedStackFrame frame, ClrRuntime runtime)
        {
            Params result = new Params();

            throw new NotImplementedException("This Stretegy is under construction...");

            return result;
        }

        internal override Params GetWaitForSingleObjectParams(UnifiedStackFrame frame)
        {
            Params result = new Params();

            throw new NotImplementedException("This Stretegy is under construction...");

            return result;
        }
    }
}
