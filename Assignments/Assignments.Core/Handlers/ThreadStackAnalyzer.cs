using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.msos;
using Microsoft.Diagnostics.Runtime;
using Assignments.Core.PrintHandles.Factory;
using Assignments.Core.PrintHandles;
using Assignments.Core.Model.ThreadStack;
using Assignments.Core.Model.StackFrame;

namespace Assignments.Core.Handlers
{
    public class ThreadStackAnalyzer
    {

        public static void DealWithNativeFrame(List<UnifiedStackFrame> list, ClrRuntime runtime, ClrThread thread)
        {

            SingleWaitStackFrameHandler singleHandler = new SingleWaitStackFrameHandler();
            MultiWaitStackFrameHandler multiHandler = new MultiWaitStackFrameHandler();

            foreach (var frame in list)
            {
                if (WinApiCallsInspector.CheckForWinApiCalls(frame, WinApiCallsInspector.WAIT_FOR_SINGLE_OBJECT_KEY))
                {
                    WinApiSingleWaitStackFrame singleParams = singleHandler.GetStackFrameParams(frame, runtime);
                }
                else if (WinApiCallsInspector.CheckForWinApiCalls(frame, WinApiCallsInspector.WAIT_FOR_MULTIPLE_OBJECTS_KEY))
                {
                    WinApiMultiWaitStackFrame multiParams = multiHandler.GetStackFrameParams(frame, runtime);
                }
            }
        }
        public static void DealWithManagedFrame(List<UnifiedStackFrame> list, ClrRuntime runtime, ClrThread thread)
        {
            //Checks wheather current thread has blocking objects
            if (thread.BlockingObjects != null && thread?.BlockingObjects?.Count > 0)
            {
                foreach (var bObj in thread.BlockingObjects)
                {
                    //TODO: Deal with BlockingObjects collection
                }
            }

            foreach (var frame in list)
            {
                //TODO: Continue
                object result = DealWithFrame(frame);
            }
        }

        private static object DealWithFrame(UnifiedStackFrame frame)
        {
            //TODO: Implementations
            return null;
        }
    }
}
