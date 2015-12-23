using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.msos;
using Microsoft.Diagnostics.Runtime;
using Assignments.Core.Model.StackFrames.UnManaged;
using Assignments.Core.Exceptions;

namespace Assignments.Core.Handlers
{
    public class UnmanagedStackFrameHandler
    {
        public UnmanagedStackFrameHandler()
        {

        }

        public List<WinApiStackFrame> Analyze(List<UnifiedStackFrame> list, ClrRuntime runtime, ClrThread thread)
        {
            List<WinApiStackFrame> result = new List<WinApiStackFrame>();

            foreach (var frame in list)
            {
                WinApiStackFrame frameParams = null;

                if (CheckForWinApiCalls(frame, WinApiSingleWaitStackFrame.FUNCTION_NAME))
                {
                    frameParams = GetSingleStackFrameParams(frame, runtime);
                    frameParams.Params = GetNativeParams(frame, runtime, 2);
                }
                else if (CheckForWinApiCalls(frame, WinApiMultiWaitStackFrame.FUNCTION_NAME))
                {
                    frameParams = GetMultipleStackFrameParams(frame, runtime);
                }

                frameParams = new WinApiStackFrame();

                frameParams.Frame = frame;

                if (frameParams.Params == null)
                {
                    frameParams.Params = GetNativeParams(frame, runtime, 4);
                }

                if (frameParams != null)
                {
                    result.Add(frameParams);
                }
            }
            return result;
        }

        public static bool CheckForWinApiCalls(UnifiedStackFrame c, string key)
        {
            bool result = c != null
                && !String.IsNullOrEmpty(c.Method)
                && c.Method != null && c.Method.Contains(key);

            return result;
        }


        private static void PrintBytesAsHex(ConsoleColor color, List<byte[]> parms)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < parms.Count; i++)
            {
                int byteValue = BitConverter.ToInt32(parms[i], 0);

                var msg = String.Format("p{0}= 0x{1:x}", i, byteValue);
                sb.Append(msg);
            }

            Console.WriteLine(sb.ToString());
        }

        public WinApiMultiWaitStackFrame GetMultipleStackFrameParams(UnifiedStackFrame frame, ClrRuntime runtime)
        {
            //TODO : Check if GetNativeParams and ReadFromMemmory functions are identical

            WinApiMultiWaitStackFrame result = new WinApiMultiWaitStackFrame();
            var nativeParams = GetNativeParams(frame, runtime, 4);

            if (nativeParams != null && nativeParams.Count > 0)
            {
                result.Frame = frame;
                result.HandlesCunt = BitConverter.ToUInt32(nativeParams[0], 0);
                result.HandleAddress = BitConverter.ToUInt32(nativeParams[1], 0);
                result.WaitallFlag = BitConverter.ToUInt32(nativeParams[2], 0);
                result.Timeout = BitConverter.ToUInt32(nativeParams[3], 0);
                result.Params = GetNativeParams(result.Frame, runtime, (int)result.HandlesCunt);
            }
            return result;
        }


        public WinApiSingleWaitStackFrame GetSingleStackFrameParams(UnifiedStackFrame frame, ClrRuntime runtime)
        {
            WinApiSingleWaitStackFrame result = new WinApiSingleWaitStackFrame();

            var nativeParams = GetNativeParams(frame, runtime, 2);

            if (nativeParams != null && nativeParams.Count > 0)
            {
                result.Frame = frame;
                result.HandleAddress = BitConverter.ToUInt32(nativeParams[0], 0);
                result.Timeout = BitConverter.ToUInt32(nativeParams[1], 0);
            }

            return result;
        }


        /// <summary>
        /// Iterates paramCount times and reads the value from memmory using runtime.ReadMemory function
        /// </summary>
        /// <param name="stackFrame"></param>
        /// <param name="runtime"></param>
        /// <param name="paramCount">Number of params of the passed stackFrame</param>
        /// <returns></returns>
        public List<byte[]> GetNativeParams(UnifiedStackFrame stackFrame, ClrRuntime runtime, int paramCount)
        {
            List<byte[]> result = new List<byte[]>();

            var offset = stackFrame.FrameOffset; //Base Pointer - % EBP
            byte[] paramBuffer;
            int bytesRead = 0;
            offset += (ulong)IntPtr.Size;

            for (int i = 0; i < paramCount; i++)
            {
                paramBuffer = new byte[4];
                offset += (ulong)IntPtr.Size;

                if (runtime.ReadMemory(offset, paramBuffer, 4, out bytesRead))
                {
                    result.Add(paramBuffer);
                }
            }

            return result;
        }
    }
}
