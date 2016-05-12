using System;
using System.Collections.Generic;
using Microsoft.Diagnostics.Runtime;
using WinHandlesQuerier.Core.Exceptions;
using WinHandlesQuerier.Core.Model.Unified;
using System.Runtime.InteropServices;
using Microsoft.Diagnostics.Runtime.Interop;
using Assignments.Core.Infra;
using WinBase;

namespace WinHandlesQuerier.Core.Handlers
{
    public class UnmanagedStackFrameWalker
    {
        public const string WAIT_FOR_SINGLE_OBJECTS_FUNCTION_NAME = "WaitForSingleObject";
        public const string WAIT_FOR_MULTIPLE_OBJECTS_FUNCTION_NAME = "WaitForMultipleObjects";
        public const string ENTER_CRITICAL_SECTION_FUNCTION_NAME = "EnterCriticalSection";

        const int ENTER_CRITICAL_SECTION_FUNCTION_PARAM_COUNT = 1;
        const int WAIT_FOR_SINGLE_OBJECT_PARAM_COUNT = 2;
        const int WAIT_FOR_MULTIPLE_OBJECTS_PARAM_COUNT = 4;
        
        internal static List<UnifiedStackFrame> Walk(DEBUG_STACK_FRAME[] stackFrames, uint framesFilled, ClrRuntime runtime, IDebugSymbols2 debugClient, uint pid = Constants.INVALID_PID)
        {
            List<UnifiedStackFrame> stackTrace = new List<UnifiedStackFrame>();
            for (uint i = 0; i < framesFilled; ++i)
            {
                var frame = new UnifiedStackFrame(stackFrames[i], (IDebugSymbols2)debugClient);
                Inpsect(frame, runtime, pid);
                stackTrace.Add(frame);
            }
            return stackTrace;
        }

        static void Inpsect(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {
           
            if (CheckForWinApiCalls(frame, WAIT_FOR_SINGLE_OBJECTS_FUNCTION_NAME))
            {
                DealWithSingle(frame, runtime, pid);
            }
            else if (CheckForWinApiCalls(frame, WAIT_FOR_MULTIPLE_OBJECTS_FUNCTION_NAME))
            {
                DealWithMultiple(frame, runtime, pid);
            }
            else
            {
                //CheckForCriticalSections(frame, runtime, result);
            }
        }

        public static bool CheckForCriticalSectionCalls(UnifiedStackFrame frame, ClrRuntime runtime, out UnifiedBlockingObject blockingObject)
        {
            bool result = false;

            if (CheckForWinApiCalls(frame, ENTER_CRITICAL_SECTION_FUNCTION_NAME))
            {
                blockingObject = ReadCriticalSectionData(frame, runtime);
                result = blockingObject != null;
            }
            else
            {
                blockingObject = null;
            }

            return result;
        }

        private static UnifiedBlockingObject ReadCriticalSectionData(UnifiedStackFrame frame, ClrRuntime runtime)
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

        private static void DealWithSingle(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
        {
            var paramz = GetNativeParams(frame, runtime, WAIT_FOR_SINGLE_OBJECT_PARAM_COUNT);
          
            var handleUint = Convert(paramz[0]);
            
            UnifiedHandle unifiedHandle = GenerateUnifiedHandle(handleUint, pid);
            if(unifiedHandle != null)
            {
                frame.Handles = new List<UnifiedHandle>();
                frame.Handles.Add(unifiedHandle);
            }
        }

        private static void DealWithMultiple(UnifiedStackFrame frame, ClrRuntime runtime, uint pid)
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

        private static UnifiedHandle GenerateUnifiedHandle(uint handleUint, uint pid)
        {
            UnifiedHandle result;

            if (pid != Constants.INVALID_PID)
            {
                var typeName = NtQueryHandler.GetHandleType((IntPtr)handleUint, pid);
                var handleName = NtQueryHandler.GetHandleObjectName((IntPtr)handleUint, pid);

                result = new UnifiedHandle(handleUint, typeName, handleName);
            }
            else
            {
                result = new UnifiedHandle(handleUint);
            }
            return result;
        }

        private static uint Convert(byte[] bits)
        {
            return BitConverter.ToUInt32(bits, 0);
        }

        public static bool CheckForWinApiCalls(UnifiedStackFrame c, string key)
        {
            bool result = c != null
                && !String.IsNullOrEmpty(c.Method)
                && c.Method != null && c.Method.Contains(key);

            return result;
        }

        public static List<byte[]> GetNativeParams(UnifiedStackFrame stackFrame, ClrRuntime runtime, int paramCount)
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

        public static List<byte[]> ReadFromMemmory(uint startAddress, uint count, ClrRuntime runtime)
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


    }
}
