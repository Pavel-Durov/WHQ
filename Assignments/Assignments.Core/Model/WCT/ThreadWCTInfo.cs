using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.Handlers.WCT;
using Microsoft.Diagnostics.Runtime;
using Assignments.Core.Extentions;
using Assignments.Core.Model.WCT;

namespace Assignments.Core.Model.WCT
{
    public class ThreadWCTInfo
    {
        public ThreadWCTInfo(bool isDeadLock, uint threadId)
        {
            IsDeadLocked = isDeadLock;
            ThreadId = threadId;
        }

        /// <summary>
        /// Specifies whether the Wait Chain is Cyclic - Deadlock
        /// </summary>
        public bool IsDeadLocked { get; private set; }
        /// <summary>
        /// OS Id of the thread
        /// </summary>
        public uint ThreadId { get; private set; }

        List<WaitChainInfoObject> _waiChainOBjects;

        /// <summary>
        /// A Collection of Wait Chain object which collected by GetThreadWaitChain api function
        /// </summary>
        public List<WaitChainInfoObject> WaitChainObject
        {
            get
            {
                if (_waiChainOBjects == null)
                {
                    _waiChainOBjects = new List<WaitChainInfoObject>();
                }
                return _waiChainOBjects;
            }
        }

        /// <summary>
        /// Adds WAITCHAIN_NODE_INFO as WaitChainInfoObject to local collection
        /// </summary>
        /// <param name="info"></param>
        internal void SetInfo(WAITCHAIN_NODE_INFO[] info)
        {
            if (info != null)
            {
                foreach (var item in info)
                {
                    var block = new WaitChainInfoObject(item);
                    WaitChainObject.Add(block);
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendWithNewLine();
            sb.AppendWithNewLine($"ThreadId: { ThreadId}");
            sb.AppendWithNewLine($"Is DeadLocked : {IsDeadLocked}");

            for (int i = 0; i < WaitChainObject.Count; i++)
            {
                var item = WaitChainObject[i];

                sb.AppendWithNewLine();

                sb.AppendWithNewLine($"WCT WAITCHAIN NODES INFO");
                sb.AppendWithNewLine();
                sb.AppendWithNewLine($"i = {i}) ");
                sb.AppendWithNewLine($"Context Switches: { item.ContextSwitches}");
                sb.AppendWithNewLine($"WaitTime: { item.WaitTime}");
                sb.AppendWithNewLine($"TimeOut: { item.TimeOut}");
                sb.AppendWithNewLine($"ObjectType: { item.ObjectType}");
                sb.AppendWithNewLine($"ObjectStatus: { item.ObjectStatus}");
                sb.AppendWithNewLine($"ObjectName: { item.ObjectName}");
                sb.AppendWithNewLine($"AlertTable: { item.AlertTable}");
            }

            return sb.ToString();
        }
    }
}
