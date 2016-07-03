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
        public StackFrameParmsFetchStrategy_Win_7(ClrRuntime runtime) : base(runtime)
        {
        }

        //RCX, RDX, R8, and R9
        internal override Params GetenterCriticalSectionParam(UnifiedStackFrame frame)
        {
            Params result = new Params();
            //000007fd`949b106b 488bf9 mov     rdi,rcx
            //Rdi is Nonvolatile register
            //result.First = frame.ThreadContext.Context_amd64.Rdi;

            return result;
        }

        internal override Params GetWaitForMultipleObjectsParams(UnifiedStackFrame frame)
        {
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

        internal override Params GetWaitForSingleObjectParams(UnifiedStackFrame frame)
        {
            Params result = new Params();
            //000007fe`fd24102b 488bf9 mov     rdi,rcx
            result.First = frame.ThreadContext.Context_amd64.Rdi;
          
            ///Rsi is a Nonvolatile register
            //result.Second = frame.ThreadContext.Context_amd64.Rsi;

            return result;
        }
    }
}
