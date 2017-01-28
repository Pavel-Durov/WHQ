using WHQ.Core.Handlers.UnmanagedStackFrame.Strategies.Base;
using System;
using System.Collections.Generic;
using WHQ.Core.Model.Unified;
using System.Runtime.InteropServices;
using WinBase;
using Microsoft.Diagnostics.Runtime;
using NtDll;

namespace WHQ.Core.Handlers.UnmanagedStackFrame.Strategies
{
    /// <summary>
    /// This class is responsible for fetching function parameters.
    /// 
    /// Since it's x86 StackWalkerStrategy, 
    /// it relies on x86 Calling Convention - passing parameters to function on the stack. 
    /// </summary>
    internal class Unmanaged_x86_StackWalkerStrategy : UnmanagedStackWalkerStrategy
    {
        internal override UnifiedBlockingObject GetNtDelayExecutionBlockingObject(UnifiedStackFrame frame, ClrRuntime runtime)
        {
            var parameters = GetNativeParams(frame, runtime, NTDELAY_EXECUTION_FUNCTION_PARAM_COUNT);
            var largeIntegerAddress = Convert(parameters[1]);

            var largeInt = ReadStructureFromAddress<LARGE_INTEGER>(largeIntegerAddress, runtime);

            //QuadPart is a negated amount of nanoseconds
            var awaitMs = (-largeInt.QuadPart) / 10000;

            return new UnifiedBlockingObject(awaitMs);
        }

        protected override UnifiedBlockingObject GetCriticalSectionBlockingObject(UnifiedStackFrame frame, ClrRuntime runtime)
        {
            var parameters = GetNativeParams(frame,runtime, ENTER_CRITICAL_SECTION_FUNCTION_PARAM_COUNT);
            var criticalSectionAddress = Convert(parameters[0]);

            var section = ReadStructureFromAddress<CRITICAL_SECTION>(criticalSectionAddress, runtime);
            return new UnifiedBlockingObject(section, criticalSectionAddress);
        }

        private T ReadStructureFromAddress<T>(ulong address, ClrRuntime runtime)
        {
            T result = default(T);

            byte[] buffer = new byte[Marshal.SizeOf(typeof(T))];
            int read;

            if (!runtime.ReadMemory(address, buffer, buffer.Length, out read) || read != buffer.Length)
                throw new Exception($"Error reading structure data from address: 0x{address.ToString("X")}");

            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                result = Marshal.PtrToStructure<T>(gch.AddrOfPinnedObject());
            }
            finally
            {
                gch.Free();
            }

            return result;
        }

        List<byte[]> GetNativeParams(UnifiedStackFrame stackFrame, ClrRuntime runtime, int paramCount)
        {
            List<byte[]> result = new List<byte[]>();

            var offset = stackFrame.FrameOffset; //Base Pointer - % EBP
            byte[] paramBuffer;
            int bytesRead = 0;
            offset += 4;

            for (int i = 0; i < paramCount; i++)
            {
                paramBuffer = new byte[IntPtr.Size];
                offset += (uint)IntPtr.Size;
                if (runtime.ReadMemory(offset, paramBuffer, 4, out bytesRead))
                {
                    result.Add(paramBuffer);
                }
            }

            return result;
        }

        protected override void DealWithCriticalSection(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {
            var paramz = GetNativeParams(frame, runtime, ENTER_CRITICAL_SECTION_FUNCTION_PARAM_COUNT);
            var criticalSectionPtr = Convert(paramz[0]);

            EnrichUnifiedStackFrame(frame, criticalSectionPtr, pid);
        }


        protected override void DealWithMultiple(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {
            var paramz = GetNativeParams(frame, runtime, WAIT_FOR_MULTIPLE_OBJECTS_PARAM_COUNT);
            frame.NativeParams = paramz;
            frame.Handles = new List<UnifiedHandle>();

            var HandlesCunt = BitConverter.ToUInt32(paramz[0], 0);
            var HandleAddress = BitConverter.ToUInt32(paramz[1], 0);

            EnrichUnifiedStackFrame(frame, runtime, pid, HandlesCunt, HandleAddress);
        }

        protected override void DealWithSingle(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {
            var paramz = GetNativeParams(frame, runtime, WAIT_FOR_SINGLE_OBJECT_PARAM_COUNT);
            var handle = Convert(paramz[0]);

            EnrichUnifiedStackFrame(frame, handle, pid);
        }
    }
}