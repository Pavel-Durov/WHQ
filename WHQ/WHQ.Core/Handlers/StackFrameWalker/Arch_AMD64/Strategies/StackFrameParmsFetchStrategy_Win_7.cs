
using System;
using WHQ.Core.Model.Unified;
using WHQ.Providers.ClrMd.Model;

namespace WHQ.Core.Handlers.UnmanagedStackFrameWalker.AMD64
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

            //1st: RCX (Wait Objects count) 
            //000007fe`fd24155a 48894c2408      mov     qword ptr [rsp+8],rcx
            var rspPtr = frame.StackPointer + 8;
            result.Third = base.ReadULong(rspPtr);


            //2nd - RDX (Array ptr)

            //3rd - R8: WaitAll (BOOLEAN)

            //4th - R9: Timeout (DWORD) 

            return result;
        }
    }
}
