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
            //RCX, RDX, R8, and R9


            Params result = new Params();
            
            //1st: RCX (Wait Objects count) 
            //000007fe`fd24155a 48894c2408      mov     qword ptr [rsp+8],rcx


            //2nd - RDX (Array ptr)


            //3rd - R8: WaitAll (BOOLEAN)
            //Overiden By:
            //000007fe`fd241570 4c8bc1 mov     r8,rcx

            //4th - R9: Timeout (DWORD) 
            //Overiden By:
            //kernel32!WaitForMultipleObjects + 0x98:
            //00000000`76dc1208 458bcc mov     r9d,r12d

            return result;
        }
    }
}
