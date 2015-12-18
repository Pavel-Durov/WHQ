using System.Collections.Generic;
using System.Text;
using Assignments.Core.Model.StackFrames.UnManaged;
using Assignments.Core.Model.WCT;
using Microsoft.Diagnostics.Runtime;
using Assignments.Core.Extentions;
using Assignments.Core.msos;


namespace Assignments.Core.Model.StackFrames
{
    public class AnalyzedThreadStack
    {
        public AnalyzedThreadStack(ClrThread thread, ThreadWCTInfo wctThreadInfo, List<UnifiedStackFrame> managedStackList, List<WinApiStackFrame> nativeStackList)
        {
            this.Thread = thread;
            this.WctThreadInfo = wctThreadInfo;
            this.ManagedStackList = managedStackList;
            this.NativeStackList = nativeStackList;
        }

        #region Properties

        public List<UnifiedStackFrame> ManagedStackList { get; private set; }
        public List<WinApiStackFrame> NativeStackList { get; private set; }
        public ClrThread Thread { get; private set; }
        public ThreadWCTInfo WctThreadInfo { get; private set; }
        public bool HasBlockingObjects { get { return Thread?.BlockingObjects != null && Thread?.BlockingObjects?.Count > 0; } }

        #endregion



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
