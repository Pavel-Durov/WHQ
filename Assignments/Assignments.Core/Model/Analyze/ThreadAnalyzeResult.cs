using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.Model.Stack;
using Assignments.Core.Model.Stack.Clr;
using Microsoft.Diagnostics.Runtime;
using Assignments.Core.Extentions;
using Assignments.Core.msos;


namespace Assignments.Core.Model.Analyze
{
    public class ThreadAnalyzeResult
    {
        public  List<UnifiedStackFrame> ManagedStackList;
        public  List<WinApiStackFrame> NativeStackList;
        public  ClrThread Thread;
        public  ThreadWCTInfo WctThreadInfo;

        public ThreadAnalyzeResult(ClrThread thread, ThreadWCTInfo wctThreadInfo, List<UnifiedStackFrame> managedStackList, List<WinApiStackFrame> nativeStackList)
        {
            this.Thread = thread;
            this.WctThreadInfo = wctThreadInfo;
            this.ManagedStackList = managedStackList;
            this.NativeStackList = nativeStackList;
        }

        public bool HasBlockingObjects { get { return Thread?.BlockingObjects != null && Thread?.BlockingObjects?.Count > 0; } }



        public override string ToString()
        {
            //TODO: Consider moving this implementation from ToString Overide to some outside handler 
            StringBuilder result = new StringBuilder();

            //Appending thread data

            result.AppendWithNewLine(Thread.AsString());

            if (HasBlockingObjects)
            {
                //Appending Blcking objects ie Extentions method
                result.Append(Thread.BlockingObjects.AsString());
            }

            result.AppendWithNewLine(WctThreadInfo.ToString());

            ///Appending Managed Stack frame list string
            result.AppendWithNewLine(ManagedStackList.AsString());

            ///Appending UnManaged Stack frame list string
            result.AppendWithNewLine(NativeStackList.AsString<WinApiStackFrame>());

            return result.ToString();
        }

        

    }
}
