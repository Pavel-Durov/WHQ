using Microsoft.Diagnostics.Runtime;
using System;
using WinHandlesQuerier.Core.Model.Unified;

namespace WinHandlesQuerier.Core.Handlers.UnmanagedStackFrameWalker.AMD64
{
    class StackFrameParmsFetchStrategy_Win_7 : StackFrameParmsFetchStrategy
    {
        public StackFrameParmsFetchStrategy_Win_7(ClrRuntime runtime) : base(runtime)
        {
            throw new NotImplementedException();
        }

        /* WaitForSingleObject
        * 
        * 1st: RCX (handle) 
        * 000007fe`fd24102b 488bf9          mov     rdi,rcx
        * 2nd - RDX (Timeout)
        * 000007fe`fd241029 8bf2            mov     esi,edx
        */

        /* EnterCriticalSection 
        * 
        * 1st: RCX (CRITICAL_SECTION ptr) 
        * 00007ffd`b57310cb 488bf9          mov     rdi,rcx
        */


        internal override Params GetWaitForMultipleObjectsParams(UnifiedStackFrame frame)
        {
            throw new NotImplementedException();


            Params result = new Params();
            //RCX, RDX, R8, and R9

            //throw new NotImplementedException("This Stretegy is under construction...");

            // RDX
            //000007fe`fd2414a2 488bda mov     rbx,rdx
            //RBX - Nonvolatile Must be preserved by callee
            //var hArrayPtr = frame.ThreadContext.Context_amd64.Rbx;
            //result.Second = hArrayPtr;

            //R8
            //000007fe`fd24149f 458bf0          mov     r14d,r8d
            //R14 - NonvolatileMust be preserved by callee
            result.Third = frame.ThreadContext.Context_amd64.R14;


            //R9
            //waittime
            return result;
        }
    }
}
