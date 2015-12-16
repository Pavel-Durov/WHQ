using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.msos;
using Microsoft.Diagnostics.Runtime;
using Assignments.Core.PrintHandles.Factory;
using Assignments.Core.PrintHandles;
using Assignments.Core.Model.Stack;
using Assignments.Core.Model.Stack.Clr;

namespace Assignments.Core.Handlers
{
    public class UnmanagedStackFrameHandler
    {
        public static List<WinApiStackFrame> Analyze(List<UnifiedStackFrame> list, ClrRuntime runtime, ClrThread thread)
        {
            List<WinApiStackFrame> result = new List<WinApiStackFrame>();

            foreach (var frame in list)
            {
                if (WinApiCallsInspector.CheckForWinApiCalls(frame, WinApiCallsInspector.WAIT_FOR_SINGLE_OBJECT_KEY))
                {
                    WinApiSingleWaitStackFrame singleParams = SingleWaitStackFrameHandler.GetStackFrameParams(frame, runtime);
                    result.Add(singleParams);
                }
                else if (WinApiCallsInspector.CheckForWinApiCalls(frame, WinApiCallsInspector.WAIT_FOR_MULTIPLE_OBJECTS_KEY))
                {
                    WinApiMultiWaitStackFrame multiParams = MultiWaitStackFrameHandler.GetStackFrameParams(frame, runtime);
                    result.Add(multiParams);
                }
            }
            return result;
        }
    }
}
