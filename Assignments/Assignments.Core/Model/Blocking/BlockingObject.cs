using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.Handlers.WCT;
using Assignments.Core.Model.Blocking;

namespace Assignments.Core.Model
{
    public class BlockingObject
    {
     
        public BlockingObject()
        {

        }

        public InfoLockObject LockObject { get;private  set; }

        public BlockingObject(WAITCHAIN_NODE_INFO item)
        {

            LockObject = new InfoLockObject(item.Union.LockObject)
        }


        public unsafe struct _WAITCHAIN_NODE_INFO_LOCK_OBJECT
        {
            /*The name of the object. Object names are only available for certain object, such as mutexes. If the object does not have a name, this member is an empty string.*/
            public fixed ushort ObjectName[WctApiConst.WCT_OBJNAME_LENGTH];
            /*This member is reserved for future use.*/
            UInt64 Timeout;
            /*This member is reserved for future use.*/
            UInt32 Alertable;
        }

    }
}
