using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Assignments.Core.Handlers.WCT
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WAITCHAIN_NODE_INFO
    {
        public WCT_OBJECT_TYPE ObjectType;
        public WCT_OBJECT_STATUS ObjectStatus;


        LockObject InfoLockObject;
        ThreadObject InfoThreadObject;

    }

    public struct LockObject
    {
        /*The name of the object. Object names are only available for certain object, such as mutexes. If the object does not have a name, this member is an empty string.*/
        string ObjectName;
        /*This member is reserved for future use.*/
        UInt64 Timeout;
        /*This member is reserved for future use.*/
        UInt32 Alertable;
    }

    public struct ThreadObject
    {
        /*The process identifier.*/
        UInt32 ProcessId;
        /*The thread identifier. For COM and ALPC, this member can be 0.*/
        UInt32 ThreadId;
        /*The wait time.*/
        UInt32 WaitTime;
        /*The number of context switches.*/
        UInt32 ContextSwitches;
    }

}
