using System.Collections.Generic;
using Advapi32;

namespace WinHandlesQuerier.Core.Model.WCT
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
