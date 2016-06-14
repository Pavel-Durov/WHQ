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

        protected override UnifiedBlockingObject ReadCriticalSectionData(UnifiedStackFrame frame, ClrRuntime runtime)
        {
            UnifiedBlockingObject result = null;

            if (frame.ThreadContext != null)
            {
                if (_globalConfigs.OsVersion == WinVersions.Win_10
                  || _globalConfigs.OsVersion == WinVersions.Win_8
                  || _globalConfigs.OsVersion == WinVersions.Win_8_1)
                {
                    //TODO: Sems like Rcx isn't moved..

                    var firstParam = frame.ThreadContext.Context_amd64.Rcx;

                    result = new UnifiedBlockingObject(firstParam, UnifiedBlockingType.CriticalSectionObject);
                }
            }

            return result;
        }


        /// <summary>
        /// Original Function call example: 
        /// var mulRes2 = Functions.WaitForMultipleObjects(19, arr, true, int.MaxValue);
        ///  
        /// </summary>
        protected override void DealWithMultiple(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {
            if (frame.ThreadContext != null)
            {
                if (_globalConfigs.OsVersion == WinVersions.Win_10
                   || _globalConfigs.OsVersion == WinVersions.Win_8
                   || _globalConfigs.OsVersion == WinVersions.Win_8_1)
                {
                    //1st : handlesCount (DWORD)
                    var handlesCount = frame.ThreadContext.Context_amd64.Rbx;
                    if (handlesCount > Kernel32.Const.MAXIMUM_WAIT_OBJECTS)
                    {
                        throw new ArgumentOutOfRangeException($"Cannot await on more then : {Kernel32.Const.MAXIMUM_WAIT_OBJECTS}, given value :{handlesCount}");
                    }

                    //2nd: Handles pointer (HANDLE)
                    //RDX - Volatile -Second integer argument
                    //Assembly
                    //00007ffa`d7653a95 4c8bea          mov     r13,rdx
                    //R13 == RDX
                    var hArrayPtr = frame.ThreadContext.Context_amd64.R13;


                    //3rd: WaitAll (BOOLEAN)
                    ///R8 - Volatile - Third integer argument
                    //
                    //00007ffa`d7653a90 4489442444      mov     dword ptr [rsp+44h],r8d
                    //var thirdParam = frame.ThreadContext.Context_amd64.R15;

                    var rspPtr = frame.StackPointer + 44 + sizeof(ulong);
                    byte[] buffer = new byte[sizeof(ulong)];
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
                    var waitTime = frame.ThreadContext.Context_amd64.R12;

                    EnrichUnifiedStackFrame(frame, runtime, pid, handlesCount, hArrayPtr);
                }
            }
        }

        /// <summary>
        ///  Original Function call example: 
        ///    Kernel32.Functions.WaitForSingleObject(hEvent, waitTime);
        ///    
        /// 
        /// </summary>
        protected override void DealWithSingle(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {
            if (frame.ThreadContext != null)
            {
                if (_globalConfigs.OsVersion == WinVersions.Win_10
                    || _globalConfigs.OsVersion == WinVersions.Win_8
                    || _globalConfigs.OsVersion == WinVersions.Win_8_1)
                {
                    if (frame.ThreadContext.Is64Bit)
                    {

                        ///RCX - Volatile - First integer argument
                        ///Handler ptr
                        var handle = frame.ThreadContext.Context_amd64.Rdi;

                        ///RDX - Volatile -Second integer argument
                        ///WaitAmount
                        var watTime = frame.ThreadContext.Context_amd64.R13;
                        //Cannot be obtained - since the value is overriden:
                        //

                        //EnrichUnifiedStackFrame(frame, handle, pid);

                        //TODO:   
                        //In manged (C#) code WaitForSingleObject function called as WaitForMultipleObjects
                        //So it can be treated as WaitForMultipleObjects call convention - but with diffrent registers flow

                    }
                }
            }
        }
    }
}
