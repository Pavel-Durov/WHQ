using System;
using System.Runtime.InteropServices;

namespace Assignments.Core.Handlers.WCT
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WAITCHAIN_NODE_INFO
    {
        public WCT_OBJECT_TYPE ObjectType;
        public WCT_OBJECT_STATUS ObjectStatus;
        public _WAITCHAIN_NODE_INFO_UNION Union;
    }


    [StructLayout(LayoutKind.Explicit)]
    public struct _WAITCHAIN_NODE_INFO_UNION
    {
        [FieldOffset(0)]
        public _WAITCHAIN_NODE_INFO_LOCK_OBJECT LockObject;
        [FieldOffset(0)]
        public _WAITCHAIN_NODE_INFO_THREAD_OBJECT ThreadObject;
    }

    public unsafe struct _WAITCHAIN_NODE_INFO_LOCK_OBJECT
    {
        /*The name of the object. Object names are only available for certain object, such as mutexes. If the object does not have a name, this member is an empty string.*/
        public fixed ushort ObjectName[WctApiConst.WCT_OBJNAME_LENGTH];
        /*This member is reserved for future use.*/
        public UInt64 Timeout;
        /*This member is reserved for future use.*/
        public UInt32 Alertable;
    }

    public struct _WAITCHAIN_NODE_INFO_THREAD_OBJECT
    {
        /*The process identifier.*/
        public UInt32 ProcessId;
        /*The thread identifier. For COM and ALPC, this member can be 0.*/
        public UInt32 ThreadId;
        /*The wait time.*/
        public UInt32 WaitTime;
        /*The number of context switches.*/
        public UInt32 ContextSwitches;
    }

}
