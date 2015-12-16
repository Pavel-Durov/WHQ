using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.Handlers.WCT;
using Assignments.Core.Utils;

namespace Assignments.Core.Model.WCT
{
    public class WaitChainInfoObject
    {
        public WaitChainInfoObject(WAITCHAIN_NODE_INFO item)
        {
            ObjectStatus = item.ObjectStatus;
            ObjectType = item.ObjectType;

            //ThreadObject data
            this.ThreadId = item.Union.ThreadObject.ThreadId;
            this.ProcessId = item.Union.ThreadObject.ProcessId;
            this.ContextSwitches = item.Union.ThreadObject.ContextSwitches;
            this.WaitTime = item.Union.ThreadObject.WaitTime;

            //LockObject data
            TimeOut = item.Union.LockObject.Timeout;
            AlertTable = item.Union.LockObject.Alertable;
            unsafe
            {
                ObjectName = StringUtil.ConvertUnsafeCStringToString(item.Union.LockObject.ObjectName, Encoding.Unicode);
            }
        }

        public WCT_OBJECT_STATUS ObjectStatus { get; private set; }
        public WCT_OBJECT_TYPE ObjectType { get; private set; }

        public bool IsBlocked { get { return ObjectStatus == WCT_OBJECT_STATUS.WctStatusBlocked; } }

        public uint ThreadId { get; private set; }
        public uint ProcessId { get; private set; }
        public string ObjectName { get; private set; }
        public uint WaitTime { get; private set; }
        public uint ContextSwitches { get; private set; }

        /// <summary>
        /// This member is reserved for future use.
        /// </summary>
        public ulong TimeOut { get; private set; }
        /// <summary>
        /// This member is reserved for future use.
        /// </summary>
        public uint AlertTable { get; private set; }


    }



}
