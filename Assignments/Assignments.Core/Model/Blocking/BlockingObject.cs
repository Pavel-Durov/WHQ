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
        public BlockingObject(WAITCHAIN_NODE_INFO item)
        {

            LockObject = new LockObjectInfo(item.Union.LockObject);
            ThreadObject = new ThreadObjectInfo(item.Union.ThreadObject);
            ObjectStatus = item.ObjectStatus;
            ObjectType = item.ObjectType;
        }


        public LockObjectInfo LockObject { get; private set; }
        public ThreadObjectInfo ThreadObject { get; private set; }
        public WCT_OBJECT_STATUS ObjectStatus { get; private set; }
        public WCT_OBJECT_TYPE ObjectType { get; private set; }


    }
}
