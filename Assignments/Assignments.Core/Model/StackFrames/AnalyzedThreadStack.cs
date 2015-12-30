using System.Collections.Generic;
using Assignments.Core.Model.StackFrames.UnManaged;
using Assignments.Core.Model.WCT;
using Microsoft.Diagnostics.Runtime;
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
    }
}
