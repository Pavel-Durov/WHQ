using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignments.Core.Model.Unified
{
    /// <summary>
    /// UnifiedBlockingObject would have the properties of CLRMD’s BlockingObject and the properties you can get out of WCT, unified:
    //  List<UnifiedThread> Owners
    //  bool HasOwnershipInformation
    //  UnifiedBlockingReason WaitReason
    //  List<UnifiedThread> Waiters
    //  int RecursionCount
    //  ulong ManagedObjectAddress
    //  string KernelObjectName
    /// </summary>
    public class UnifiedBlockingObject
    {
    }
}
