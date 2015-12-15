using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.Model.Stack;
using Assignments.Core.Model.Stack.Clr;
using Microsoft.Diagnostics.Runtime;
using Assignments.Core.Extentions;

namespace Assignments.Core.Model.Analyze
{
    public class ThreadAnalyzeResult
    {
        private List<ClrWaitStackFrame> managedStackList;
        private List<WinApiStackFrame> nativeStackList;
        private ClrThread thread;
        private ThreadWaitInfo wctThreadInfo;

        public ThreadAnalyzeResult(ClrThread thread, ThreadWaitInfo wctThreadInfo, List<ClrWaitStackFrame> managedStackList, List<WinApiStackFrame> nativeStackList)
        {
            this.thread = thread;
            this.wctThreadInfo = wctThreadInfo;
            this.managedStackList = managedStackList;
            this.nativeStackList = nativeStackList;
        }

        public bool HasBlockingObjects { get { return thread?.BlockingObjects != null && thread?.BlockingObjects?.Count > 0; } }



        public override string ToString()
        {
            //TODO: Consider moving this implementation from ToString Overide to some outside handler 
            StringBuilder result = new StringBuilder();

            //TODO: Append thread data

            if (HasBlockingObjects)
            {
                //Appending Blcking objects ie Extentions method
                result.Append(thread.BlockingObjects.GetAsString());
            }

            result.Append(wctThreadInfo.ToString());

            //TODO: Append Managed Stak List string
            //TODO: Append Native Stack LIst to String
                        return result.ToString();
        }

        

    }
}
