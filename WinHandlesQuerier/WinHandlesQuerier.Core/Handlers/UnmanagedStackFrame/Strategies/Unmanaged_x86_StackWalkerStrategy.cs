using Assignments.Core.Handlers.UnmanagedStackFrame.Strategies.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Runtime;
using WinHandlesQuerier.Core.Model.Unified;
using System.Runtime.InteropServices;
using WinBase;
using WinHandlesQuerier.Core.Exceptions;

namespace Assignments.Core.Handlers.UnmanagedStackFrame.Strategies
{
    class Unmanaged_x86_StackWalkerStrategy : UnmanagedStackWalkerStrategy
    {
        protected override UnifiedBlockingObject ReadCriticalSectionData(UnifiedStackFrame frame, ClrRuntime runtime)
        {
            UnifiedBlockingObject result = null;
            var paramz = GetNativeParams(frame, runtime, ENTER_CRITICAL_SECTION_FUNCTION_PARAM_COUNT);

            var address = Convert(paramz[0]);

            byte[] buffer = new byte[Marshal.SizeOf<CRITICAL_SECTION>()];

            int read;

            if (!runtime.ReadMemory(address, buffer, buffer.Length, out read) || read != buffer.Length)
                throw new AccessingNonReadableMemmory($"Address : {address}");

            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            try
            {
                var section = Marshal.PtrToStructure<CRITICAL_SECTION>(gch.AddrOfPinnedObject());
                result = new UnifiedBlockingObject(section, address);
            }
            finally
            {
                gch.Free();
            }
            return result;
        }

        protected List<byte[]> GetNativeParams(UnifiedStackFrame stackFrame, ClrRuntime runtime, int paramCount)
        {
            List<byte[]> result = new List<byte[]>();

            var offset = stackFrame.FrameOffset; //Base Pointer - % EBP
            byte[] paramBuffer;
            int bytesRead = 0;
            offset += 4;

            for (int i = 0; i < paramCount; i++)
            {
                paramBuffer = new byte[4];
                offset += (uint)IntPtr.Size;
                if (runtime.ReadMemory(offset, paramBuffer, 4, out bytesRead))
                {
                    result.Add(paramBuffer);
                }
            }

            return result;
        }

        public List<byte[]> ReadFromMemmory(uint startAddress, uint count, ClrRuntime runtime)
        {
            List<byte[]> result = new List<byte[]>();
            int sum = 0;
            //TODO: Check if dfor can be inserted into the REadMemmory result (seems to be..)
            for (int i = 0; i < count; i++)
            {
                byte[] readedBytes = new byte[4];
                if (runtime.ReadMemory(startAddress, readedBytes, 4, out sum))
                {
                    result.Add(readedBytes);
                }
                else
                {
                    throw new AccessingNonReadableMemmory(string.Format("Accessing Unreadable memorry at {0}", startAddress));
                }
                //Advancing the pointer by 4 (32-bit system)
                startAddress += (uint)IntPtr.Size;
            }
            return result;
        }

        protected override void DealWithMultiple(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {
            var paramz = GetNativeParams(frame, runtime, WAIT_FOR_MULTIPLE_OBJECTS_PARAM_COUNT);
            frame.NativeParams = paramz;
            frame.Handles = new List<UnifiedHandle>();

            var HandlesCunt = BitConverter.ToUInt32(paramz[0], 0);
            var HandleAddress = BitConverter.ToUInt32(paramz[1], 0);

            var handles = ReadFromMemmory(HandleAddress, HandlesCunt, runtime);
            foreach (var handle in handles)
            {
                uint handleUint = Convert(handle);

                UnifiedHandle unifiedHandle = GenerateUnifiedHandle(handleUint, pid);
                if (unifiedHandle != null)
                {
                    frame.Handles.Add(unifiedHandle);
                }
            }
        }

        protected override void DealWithSingle(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {
            var paramz = GetNativeParams(frame, runtime, WAIT_FOR_SINGLE_OBJECT_PARAM_COUNT);

            var handleUint = Convert(paramz[0]);

            UnifiedHandle unifiedHandle = GenerateUnifiedHandle(handleUint, pid);
            if (unifiedHandle != null)
            {
                frame.Handles = new List<UnifiedHandle>();
                frame.Handles.Add(unifiedHandle);
            }
        }
    }
}