using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.msos;
using Microsoft.Diagnostics.Runtime;
using Assignments.Core.Model.StackFrame;
using Assignments.Core.Exceptions;

namespace Assignments.Core.PrintHandles.Factory
{
    public class MultiWaitStackFrameHandler : StackFrameHandler
    {
        public WinApiMultiWaitStackFrame GetStackFrameParams(UnifiedStackFrame item, ClrRuntime runtime)
        {
            WinApiMultiWaitStackFrame result = new WinApiMultiWaitStackFrame();
            var nativeParams = this.GetNativeParams(item, runtime, 4);

            if (nativeParams != null && nativeParams.Count > 0)
            {
                result.HandlesCunt = BitConverter.ToUInt32(nativeParams[0], 0);
                result.HandleAddress = BitConverter.ToUInt32(nativeParams[1], 0);
                result.WaitallFlag = BitConverter.ToUInt32(nativeParams[2], 0);
                result.WaitTimeout = BitConverter.ToUInt32(nativeParams[3], 0);
                result.ByteParams = GetParams(result.HandleAddress, result.HandlesCunt, runtime);
            }
            return result;
        }

        public override List<byte[]> GetNativeParams(UnifiedStackFrame stackFrame, ClrRuntime runtime, int paramCount)
        {
            return base.GetNativeParams(stackFrame, runtime, 4);
        }

        protected uint[] GetParams(UInt32 handleAddress, UInt32 handlesCunt, ClrRuntime runtime)
        {
            uint[] result = new uint[handlesCunt];
            //Reading n times from memmory, advansing by 4 bytes each time
            byte[] readedBytes = null;
            int count = 0;
            for (int i = 0; i < handlesCunt; i++)
            {
                readedBytes = new byte[4];

                if (runtime.ReadMemory(handleAddress, readedBytes, 4, out count))
                {
                    uint byteValue = BitConverter.ToUInt32(readedBytes, 0);
                    result[i] = byteValue;
                }
                else
                {
                    throw new AccessingNonReadableMemmory(string.Format("Accessing Unreadable memorry at {0}", handleAddress));
                    //Print(ConsoleColor.Red, "Unreadable memorry");
                }
                //Advancing the pointer by 4 (32-bit system)
                handleAddress += 4;
            }
            return result;
        }

    }
}
