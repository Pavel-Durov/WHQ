using Microsoft.Diagnostics.Runtime;
using System;
using WinHandlesQuerier.Core.Model.Unified;

namespace Assignments.Core.Handlers.UnmanagedStackFrameWalker.Strategies.x64.FunctionParamsFetchStrategies
{

    class StackFrameParmsFetchStrategy_Win_10 : StackFrameParmsFetchStrategy
    {
        internal override Params GetenterCriticalSectionParam(UnifiedStackFrame frame)
        {
            Params result = new Params();
            //00007ffd`b57310cb 488bf9          mov     rdi,rcx
            //Rdi is Nonvolatile register
            var criticalSectionPtr = frame.ThreadContext.Context_amd64.Rdi;

            result.First = criticalSectionPtr;
            return result;
        }

        internal override Params GetWaitForMultipleObjectsParams(UnifiedStackFrame frame, ClrRuntime runtime)
        {
            Params result = new Params();
            //RCX, RDX, R8, and R9

            //RCX
            //00007ffd`b57312e5 8bd9            mov     ebx,ecx
            var handlesCount = frame.ThreadContext.Context_amd64.Rbx;

            if (handlesCount > Kernel32.Const.MAXIMUM_WAIT_OBJECTS)
            {
                throw new ArgumentOutOfRangeException($"Cannot await for more then : {Kernel32.Const.MAXIMUM_WAIT_OBJECTS}, given value :{handlesCount}");
            }
            result.First = handlesCount;

            //RDX
            //00007ffd`b57312e2 4c8bea          mov     r13,rdx
            var hArrayPtr = frame.ThreadContext.Context_amd64.R13;
            result.Second = hArrayPtr;

            //3rd: WaitAll (BOOLEAN)
            ///R8 - Volatile - Third integer argument
            //
            //00007ffa`d7653a90 4489442444      mov     dword ptr [rsp+44h],r8d
            //
            //this one fetched from the stack.
            var rspPtr = frame.StackPointer + 44 + (ulong)IntPtr.Size;
            byte[] buffer = new byte[IntPtr.Size];
            int read = 0;
            bool waitAllFlagParam = false;
            if (runtime.ReadMemory(rspPtr, buffer, buffer.Length, out read))
            {
                waitAllFlagParam = BitConverter.ToBoolean(buffer, 0);
            }

            result.Third = waitAllFlagParam ? (ulong)1 : (ulong)0;



            //4th: Timeout (DWORD) 
            //R9 - Volatile - Fourth integer argument
            //Assembly:
            //00007ffa`d7653a8d 458be1          mov     r12d,r9d
            //
            //R12 is a Nonvolatile register
            var waitTime = frame.ThreadContext.Context_amd64.R12;
            result.Fourth = waitTime;

            return result;
        }

        internal override Params GetWaitForSingleObjectParams(UnifiedStackFrame frame)
        {
            Params result = new Params();
            ///RCX - Volatile - First integer argument
            ///00007ff8`c74baa6b 488bf9          mov     rdi,rcx
            ///Rdi is a Nonvolatile register
            result.First = frame.ThreadContext.Context_amd64.Rdi;
            ///RDX - Volatile -Second integer argument
            ///Rsi is a Nonvolatile register
            result.Second = frame.ThreadContext.Context_amd64.Rsi;

            return result;
        }
    }
}
