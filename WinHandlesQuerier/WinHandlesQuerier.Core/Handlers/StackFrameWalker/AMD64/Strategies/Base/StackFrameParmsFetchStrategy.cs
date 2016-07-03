using Microsoft.Diagnostics.Runtime;
using WinHandlesQuerier.Core.Model.Unified;

namespace WinHandlesQuerier.Core.Handlers.UnmanagedStackFrameWalker.AMD64
{
    internal abstract class StackFrameParmsFetchStrategy
    {
        internal abstract Params GetWaitForMultipleObjectsParams(UnifiedStackFrame frame, ClrRuntime runtime);
        internal abstract Params GetWaitForSingleObjectParams(UnifiedStackFrame frame);
        internal abstract Params GetenterCriticalSectionParam(UnifiedStackFrame frame);
    }

    internal struct Params
    {
        internal ulong First;
        internal ulong Second;
        internal ulong Third;
        internal ulong Fourth;
    }
}
