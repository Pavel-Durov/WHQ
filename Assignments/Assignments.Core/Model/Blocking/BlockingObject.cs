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

        public LockObjectInfo LockObject { get;private  set; }
        public ThreadObjectInfo ThreadObject { get; set; }

        public BlockingObject(WAITCHAIN_NODE_INFO item)
        {

            LockObject = new LockObjectInfo(item.Union.LockObject);
            ThreadObject = new ThreadObjectInfo(item.Union.ThreadObject);
           
        }


      

    }
}
