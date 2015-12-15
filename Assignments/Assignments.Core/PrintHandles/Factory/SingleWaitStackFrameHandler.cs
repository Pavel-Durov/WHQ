using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.msos;
using Microsoft.Diagnostics.Runtime;
using Assignments.Core.Model.Stack;

namespace Assignments.Core.PrintHandles.Factory
{
    public class SingleWaitStackFrameHandler
    {
        public static WinApiSingleWaitStackFrame GetStackFrameParams(UnifiedStackFrame item, ClrRuntime runtime)
        {
            WinApiSingleWaitStackFrame result = new WinApiSingleWaitStackFrame();

            var nativeParams = StackFrameHandler.GetNativeParams(item, runtime, 2);

            if (nativeParams != null && nativeParams.Count > 0)
            {
                result.HandleAddress = BitConverter.ToUInt32(nativeParams[0], 0);
                result.Timeout= BitConverter.ToUInt32(nativeParams[1], 0);
            }

            return result;
        }
    }
}
