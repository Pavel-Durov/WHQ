using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignment_3.msos;
using Microsoft.Diagnostics.Runtime;

namespace Assignment_3.PrintHandles.Factory
{
    class SingleWaitStackFrameHandler : StackFrameHandler
    {
        public override void Print(UnifiedStackFrame item, ClrRuntime runtime)
        {
            var nativeParams = this.GetNativeParams(item, runtime, 2);

            if (nativeParams != null && nativeParams.Count > 0)
            {
                var handleAddress = BitConverter.ToUInt32(nativeParams[0], 0);
                var dwMilliseconds = BitConverter.ToUInt32(nativeParams[1], 0);

                // base.Print(handleAddress, 1, runtime);
                Console.WriteLine("handleAddress : {0}, timeout in ms : {1}", handleAddress, dwMilliseconds);
            }
        }

        public override List<byte[]> GetNativeParams(UnifiedStackFrame stackFrame, ClrRuntime runtime, int paramCount)
        {
            return base.GetNativeParams(stackFrame, runtime, 2);
        }
    }
}
