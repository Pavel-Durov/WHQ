using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.Handlers.WCT;

namespace Assignments.Core.Model.Blocking
{
    public class LockObjectInfo
    {
        private _WAITCHAIN_NODE_INFO_LOCK_OBJECT lockObject;

        public LockObjectInfo(_WAITCHAIN_NODE_INFO_LOCK_OBJECT lockObject)
        {

            //TODO deal with the ushort convertion...
            //ObjectName = lockObject.ObjectName;
            TimeOut = lockObject.Timeout;
            AlertTable = lockObject.Alertable;
        }
        /// <summary>
        ///The name of the object. Object names are only available for certain object, such as mutexes. If the object does not have a name, this member is an empty string
        /// </summary>
        public object ObjectName{ get; private set; }
        /// <summary>
        /// This member is reserved for future use.
        /// </summary>
        public ulong TimeOut{ get; private set; }
        /// <summary>
        /// This member is reserved for future use.
        /// </summary>
        public uint AlertTable{ get; private set; }

    }
}
