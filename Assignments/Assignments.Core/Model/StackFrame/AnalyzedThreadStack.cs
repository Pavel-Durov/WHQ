using System.Collections.Generic;
using Assignments.Core.Model.StackFrames.UnManaged;
using Assignments.Core.Model.WCT;
using Microsoft.Diagnostics.Runtime;
using Assignments.Core.Model.Unified;
using Assignments.Core.Model.Unified.Thread;
using Assignments.Core.Model.Unified.Stack;

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



            //Refactoring
            //this.Refactoring_Thread = new UnifiedManagedThread(thread);
        }

        #region Properties

        public List<UnifiedStackFrame> ManagedStackList { get; private set; }
        public List<WinApiStackFrame> NativeStackList { get; private set; }
        //public ClrThread Thread { get; private set; }
        public ThreadWCTInfo WctThreadInfo { get; private set; }

        public bool HasBlockingObjects { get { return Thread?.BlockingObjects != null && Thread?.BlockingObjects?.Count > 0; } }

        public ClrThread Thread { get; private set; }

        #endregion






        #region Refactoring 

        public UnifiedThread Refactoring_Thread { get; private set; }

        public UnifiedStackTrace Refactoring_StackTrace { get; private set; }

        public List<UnifiedStackFrame> Refactoring_StackFrames { get; private set; }

        public List<UnifiedBlockingObject> Refactoring_BlockingObjects { get; private set; }

        #endregion
    }
}
