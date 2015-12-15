using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.msos;
using Microsoft.Diagnostics.Runtime;
using Assignments.Core.Model.Stack;
using Assignments.Core.Exceptions;

namespace Assignments.Core.PrintHandles.Factory
{
    public class MultiWaitStackFrameHandler
    {
        public static WinApiMultiWaitStackFrame GetStackFrameParams(UnifiedStackFrame item, ClrRuntime runtime)
        {
            WinApiMultiWaitStackFrame result = new WinApiMultiWaitStackFrame();
            var nativeParams = StackFrameHandler.GetNativeParams(item, runtime, 4);

            if (nativeParams != null && nativeParams.Count > 0)
            {
                result.HandlesCunt = BitConverter.ToUInt32(nativeParams[0], 0);
                result.HandleAddress = BitConverter.ToUInt32(nativeParams[1], 0);
                result.WaitallFlag = BitConverter.ToUInt32(nativeParams[2], 0);
                result.Timeout = BitConverter.ToUInt32(nativeParams[3], 0);
                result.ByteParams = StackFrameHandler.GetParams(result.HandleAddress, result.HandlesCunt, runtime);
            }
            return result;
        }
    }
}
