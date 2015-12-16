using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.Handlers.WCT;
using Microsoft.Diagnostics.Runtime;
using Assignments.Core.Extentions;

namespace Assignments.Core.Model
{
    public class ThreadWCTInfo
    {
        public ThreadWCTInfo(bool isDeadLock, uint threadId)
        {
            IsDeadLocked = isDeadLock;
            ThreadId = threadId;
        }

        public bool IsDeadLocked { get; private set; }
        public uint ThreadId { get; private set; }

        List<WaitChainInfoObject> _blockingObjects;

        public List<WaitChainInfoObject> WctBlockingObjects
        {
            get
            {
                if (_blockingObjects == null)
                {
                    _blockingObjects = new List<WaitChainInfoObject>();
                }
                return _blockingObjects;
            }
        }

        internal void SetInfo(WAITCHAIN_NODE_INFO[] info)
        {
            if (info != null)
            {
                foreach (var item in info)
                {
                    var block = new WaitChainInfoObject(item);
                    WctBlockingObjects.Add(block);
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendWithNewLine($"ThreadId: { ThreadId}");
            sb.AppendWithNewLine($"Is DeadLocked : {IsDeadLocked}");

            for (int i = 0; i < WctBlockingObjects.Count; i++)
            {
                var item = WctBlockingObjects[i];
                sb.AppendWithNewLine($" -- {i} -- ");
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
