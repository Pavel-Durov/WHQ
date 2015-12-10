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

        public ThreadWaitInfo(ClrThread thread)
        {
            if (thread != null)
            {
                ThreadOsId = thread.OSThreadId;
                Address = thread.Address;

            }
        }

        #region Properties

        public uint ThreadOsId { get; private set; }
        public ulong Address { get; private set; }


        #endregion

        internal void AddInfo(WctApiHandler.WAITCHAIN_NODE_INFO wctInfo)
        {
            
        }
    }
}
