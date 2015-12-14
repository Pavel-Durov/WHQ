using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.Handlers.WCT;
using Microsoft.Diagnostics.Runtime;

namespace Assignments.Core.Model
{
    public class ThreadWaitInfo
    {
        public ThreadWaitInfo(ClrThread thread, bool isDeadLock)
        {
            Thread = thread;
            IsDeadLocked = isDeadLock;
        }

        public ClrThread Thread { get; private set; }
        public bool IsDeadLocked { get; private set; }

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
    }
}
