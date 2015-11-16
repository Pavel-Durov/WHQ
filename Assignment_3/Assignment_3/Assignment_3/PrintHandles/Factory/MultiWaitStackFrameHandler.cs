using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignment_3.msos;
using Microsoft.Diagnostics.Runtime;

namespace Assignment_3.PrintHandles.Factory
{
    class MultiWaitStackFrameHandler : StackFrameHandler
    {
        public override void Print(UnifiedStackFrame item, ClrRuntime runtime)
        {
            var nativeParams = this.GetNativeParams(item, runtime, 4);

            if (nativeParams != null && nativeParams.Count > 0)
            {
                var handlesCunt = BitConverter.ToUInt32(nativeParams[0], 0);
                var handleAddress = BitConverter.ToUInt32(nativeParams[1], 0);
                var waitallFlag = BitConverter.ToUInt32(nativeParams[2], 0);
                var waitTimeout = BitConverter.ToUInt32(nativeParams[3], 0);

                if (handlesCunt > 0)
                {
                    base.Print(handleAddress, handlesCunt, runtime);
                }
            }
        }

        public override List<byte[]> GetNativeParams(UnifiedStackFrame stackFrame, ClrRuntime runtime, int paramCount)
        {
            return base.GetNativeParams(stackFrame, runtime, 4);
        }
    }
}
