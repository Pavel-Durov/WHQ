using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.msos;
using Microsoft.Diagnostics.Runtime;
using Assignments.Core.Model.StackFrame;

namespace Assignments.Core.PrintHandles.Factory
{
    public class SingleWaitStackFrameHandler : StackFrameHandler
    {
        public WinApiSingleWaitStackFrame GetStackFrameParams(UnifiedStackFrame item, ClrRuntime runtime)
        {
            WinApiSingleWaitStackFrame result = new WinApiSingleWaitStackFrame();

            var nativeParams = this.GetNativeParams(item, runtime, 2);

            if (nativeParams != null && nativeParams.Count > 0)
            {
                result.HandleAddress = BitConverter.ToUInt32(nativeParams[0], 0);
                result.TimeoutMilliseconds = BitConverter.ToUInt32(nativeParams[1], 0);
            }

            return result;
        }

        public override List<byte[]> GetNativeParams(UnifiedStackFrame stackFrame, ClrRuntime runtime, int paramCount)
        {
            return base.GetNativeParams(stackFrame, runtime, 2);
        }
    }
}
