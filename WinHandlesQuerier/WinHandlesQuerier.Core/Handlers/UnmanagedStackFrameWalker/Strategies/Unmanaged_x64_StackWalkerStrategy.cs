using Assignments.Core.Handlers.UnmanagedStackFrame.Strategies.Base;
using Microsoft.Diagnostics.Runtime;
using WinHandlesQuerier.Core.Model.Unified;
using Assignments.Core.Infra;
using System;

namespace Assignments.Core.Handlers.UnmanagedStackFrame.Strategies
{
    /// <summary>
    /// This class is responsible for fetching function parameters.
    /// 
    /// Since it's x64 StackWalkerStrategy, 
    /// it relies on x64 Calling Convention - passing parameters to function using CPU registers. 
    /// 
    ///https://msdn.microsoft.com/en-us/library/9z1stfyw.aspx
    ///RCX - Volatile - First integer argument
    ///RDX - Volatile -Second integer argument
    ///R8 - Volatile - Third integer argument
    ///R9 - Volatile - Fourth integer argument
    /// </summary>
    internal class Unmanaged_x64_StackWalkerStrategy : UnmanagedStackWalkerStrategy
    {
        public Unmanaged_x64_StackWalkerStrategy()
        {
            _globalConfigs = Config.GetInstance();
        }

        Config _globalConfigs;

        /// <summary>
        /// EnterCriticalSection(ref section);
        /// </summary>
        protected override UnifiedBlockingObject ReadCriticalSectionData(UnifiedStackFrame frame, ClrRuntime runtime)
        {
            UnifiedBlockingObject result = null;

            if (frame.ThreadContext != null)
            {
                //1nd: CriticalSeciton pointer (HANDLE)
                //RCX - Volatile -Second integer argument
                //
                //Assembly : 
                //00007ff8`c74baa6b 488bf9          mov     rdi,rcx
                //Rdi is Nonvolatile register
                var firstParam = frame.ThreadContext.Context_amd64.Rdi;
                result = new UnifiedBlockingObject(firstParam, UnifiedBlockingType.CriticalSectionObject);
            }

            return result;
        }


        /// <summary>
        /// Original Function call example: 
        /// var mulRes2 = WaitForMultipleObjects(19, arr, true, int.MaxValue);
        ///  
        /// </summary>
        protected override void DealWithMultiple(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {
            if (frame.ThreadContext != null)
            {
                //1st : handlesCount (DWORD)
                //Assembly:
                //00007ffb`bb183a98 8bd9            mov     ebx,ecx
                //
                //Rbx = Ebx , Rbx is a Nonvolatile register
                var handlesCount = frame.ThreadContext.Context_amd64.Rbx;
                if (handlesCount > Kernel32.Const.MAXIMUM_WAIT_OBJECTS)
                {
                    throw new ArgumentOutOfRangeException($"Cannot await on more then : {Kernel32.Const.MAXIMUM_WAIT_OBJECTS}, given value :{handlesCount}");
                }

                //2nd: Handles pointer (HANDLE)
                //RDX - Volatile -Second integer argument
                //Assembly
                //00007ffa`d7653a95 4c8bea          mov     r13,rdx
                //
                //R13 is a Nonvolatile register
                var hArrayPtr = frame.ThreadContext.Context_amd64.R13;


                //3rd: WaitAll (BOOLEAN)
                ///R8 - Volatile - Third integer argument
                //
                //00007ffa`d7653a90 4489442444      mov     dword ptr [rsp+44h],r8d
                //
                //ThirdParam fetched from the stack.
                
                var rspPtr = frame.StackPointer + 44 + (ulong)IntPtr.Size;
                byte[] buffer = new byte[IntPtr.Size];
                int read = 0;
                bool waitAllFlagParam;
                if (runtime.ReadMemory(rspPtr, buffer, buffer.Length, out read))
                {
                    waitAllFlagParam = BitConverter.ToBoolean(buffer, 0);
                }

                //4th: Timeout (DWORD) 
                //R9 - Volatile - Fourth integer argument
                //Assembly:
                //00007ffa`d7653a8d 458be1          mov     r12d,r9d
                //
                //R12 is a Nonvolatile register
                var waitTime = frame.ThreadContext.Context_amd64.R12;

                EnrichUnifiedStackFrame(frame, runtime, pid, handlesCount, hArrayPtr);
            }
        }

        /// <summary>
        ///  Original Function call example: 
        ///    WaitForSingleObject(hEvent, waitTime);
        ///    
        /// 
        /// </summary>
        protected override void DealWithSingle(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {
            if (frame.ThreadContext != null)
            {
                ///RCX - Volatile - First integer argument
                ///Handler ptr
                ///
                ///00007ff8`c74baa6b 488bf9          mov     rdi,rcx
                ///
                ///Rdi is a Nonvolatile register
                var handle = frame.ThreadContext.Context_amd64.Rdi;

                ///RDX - Volatile -Second integer argument
                ///WaitAmount
                ///Rsi is a Nonvolatile register
                var watTime = frame.ThreadContext.Context_amd64.Rsi;
                EnrichUnifiedStackFrame(frame, handle, pid);
            }
        }
    }
}
