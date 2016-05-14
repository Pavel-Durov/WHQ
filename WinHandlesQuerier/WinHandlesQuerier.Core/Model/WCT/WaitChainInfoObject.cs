using System.Runtime.InteropServices;
using System;
using WinHandlesQuerier.Core.Model.Unified;
using Advapi32;

namespace WinHandlesQuerier.Core.Model.WCT
{
    public class WaitChainInfoObject
    {
        public WaitChainInfoObject(WAITCHAIN_NODE_INFO item)
        {
            ObjectStatus = item.ObjectStatus;
            ObjectType = item.ObjectType;


            //LockObject stuct data
            TimeOut = item.Union.LockObject.Timeout;
            AlertTable = item.Union.LockObject.Alertable;

            if (item.ObjectType == WCT_OBJECT_TYPE.WctThreadType)
            { //Use the ThreadObject part of the union
                this.ThreadId = item.Union.ThreadObject.ThreadId;
                this.ProcessId = item.Union.ThreadObject.ProcessId;
                this.ContextSwitches = item.Union.ThreadObject.ContextSwitches;
                this.WaitTime = item.Union.ThreadObject.WaitTime;

            }
            else
            {//Use the LockObject  part of the union
                unsafe
                {
                    ObjectName = Marshal.PtrToStringUni((IntPtr)item.Union.LockObject.ObjectName);
                }
            }

        }


        public WCT_OBJECT_STATUS ObjectStatus { get; private set; }
        public WCT_OBJECT_TYPE ObjectType { get; private set; }

        /// <summary>
        /// Is Current Objects Status is WCT_OBJECT_STATUS.WctStatusBlocked
        /// </summary>
        public bool IsBlocked { get { return ObjectStatus == WCT_OBJECT_STATUS.WctStatusBlocked; } }
        /// <summary>
        /// The thread identifier. For COM and ALPC, this member can be 0.
        /// </summary>
        public uint ThreadId { get; private set; }
        /// <summary>
        /// The process identifier.
        /// </summary>
        public uint ProcessId { get; private set; }
        /// <summary>
        /// The name of the object. Object names are only available for certain object, such as mutexes. 
        /// If the object does not have a name, this member is an empty string.
        /// </summary>
        public string ObjectName { get; private set; }
        /// <summary>
        /// The wait time.
        /// </summary>
        public uint WaitTime { get; private set; }
        /// <summary>
        /// The number of context switches.
        /// </summary>
        public uint ContextSwitches { get; private set; }
        /// <summary>
        /// This member is reserved for future use.
        /// </summary>
        public ulong TimeOut { get; private set; }
        /// <summary>
        /// This member is reserved for future use.
        /// </summary>
        public uint AlertTable { get; private set; }
        public UnifiedBlockingReason UnifiedType
        {
            get
            {
                var wctIndex = (int)this.ObjectType;
                return (UnifiedBlockingReason)(UnifiedBlockingObject.BLOCK_REASON_WCT_SECTION_START_INDEX + wctIndex);
            }
        }
    }



}
