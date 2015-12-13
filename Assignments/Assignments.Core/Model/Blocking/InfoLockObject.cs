using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.Handlers.WCT;

namespace Assignments.Core.Model.Blocking
{
    public class InfoLockObject
    {
        private _WAITCHAIN_NODE_INFO_LOCK_OBJECT lockObject;

        public InfoLockObject(_WAITCHAIN_NODE_INFO_LOCK_OBJECT lockObject)
        {

            //TODO deal with the ushort convertion...
            //ObjectName = lockObject.ObjectName;
            TimeOut = lockObject.Timeout;
            AlertTable = lockObject.Alertable;
        }

        public object ObjectName{ get; private set; }
        public ulong TimeOut{ get; private set; }
        public uint AlertTable{ get; private set; }
    }
}
