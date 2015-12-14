using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.msos;
using Assignments.Core.PrintHandles.Factory;
using Assignments.Core.PrintHandles;
using Microsoft.Diagnostics.Runtime;
using Assignments.Core.Utils;

namespace Assignments.Core.Model.ThreadStack
{
    public class AnalyzedUnManagedStack : AnalyzedStack
    {
        internal override void Fill(IEnumerable<UnifiedStackFrame> stackTrace, ClrRuntime runtime, ClrThread thread)
        {
          
            foreach (var frame in stackTrace)
            {
                base.Add(frame);

                string hexValue = HexUtil.GetBytesAshex(ConsoleColor.Green, WinApiCallsInspector.GetNativeParams(frame, runtime, 4));

                DealWithNativeFrame(frame, runtime);
                Console.WriteLine();
            }
        }


        private static void DealWithNativeFrame(UnifiedStackFrame frame, ClrRuntime runtime)
        {
            if (WinApiCallsInspector.CheckForWinApiCalls(frame, WinApiCallsInspector.WAIT_FOR_SINGLE_OBJECT_KEY))
            {
                SingleWaitStackFrameHandler singleHandler = new SingleWaitStackFrameHandler();
                var paramz = singleHandler.GetStackFrameParams(frame, runtime);
            }
            else if (WinApiCallsInspector.CheckForWinApiCalls(frame, WinApiCallsInspector.WAIT_FOR_MULTIPLE_OBJECTS_KEY))
            {
                MultiWaitStackFrameHandler multiHandler = new MultiWaitStackFrameHandler();
                var paramz = multiHandler.GetStackFrameParams(frame, runtime);
            }
            else
            {
                //Do nothing...
            }
        }
    }
}
