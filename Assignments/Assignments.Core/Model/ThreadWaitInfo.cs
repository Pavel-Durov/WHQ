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
        //Task :
        //  This is also a good time to build an object model to represent the wait information 
        //  (instead of just printing it to the console). You're free to do your own design, 
        //  but I would suggest something similar to the CLRMD API (which has a ClrThread class with a BlockingObjects collection).

        public ThreadWaitInfo(ClrThread thread)
        {
            if (thread != null)
            {
                ThreadOsId = thread.OSThreadId;
                Address = thread.Address;

            }
        }

        #region Properties + Data members

        public uint ThreadOsId { get; private set; }
        public ulong Address { get; private set; }

        List<BlockingObject> _blockingObjects;

        public List<BlockingObject> BlockingObjects
        {
            get
            {
                if(_blockingObjects == null)
                {
                    _blockingObjects = new List<BlockingObject>();
                }
                return _blockingObjects;
            }
        }

        

        #endregion

        void AddInfo(WAITCHAIN_NODE_INFO wctInfo)
        {
            BlockingObject temp = new BlockingObject();
            //item.Union.LockObject



            BlockingObjects.Add(temp);
        }

        internal void SetInfo(WAITCHAIN_NODE_INFO[] info)
        {
            if(info != null)
            {
                foreach (var item in info)
                {
                    AddInfo(item);
                }
            }
        }
    }
}

//
// Summary:
//     Represents a managed lock within the runtime.
//public abstract class BlockingObject
//{
//    protected BlockingObject();

//    //
//    // Summary:
//    //     Returns true if this lock has only one owner. Returns false if this lock may
//    //     have multiple owners (for example, readers on a RW lock).
//    public abstract bool HasSingleOwner { get; }
//    //
//    // Summary:
//    //     The object associated with the lock.
//    public abstract ulong Object { get; }
//    //
//    // Summary:
//    //     The thread which currently owns the lock. This is only valid if Taken is true
//    //     and only valid if HasSingleOwner is true.
//    public abstract ClrThread Owner { get; }
//    //
//    // Summary:
//    //     Returns the list of owners for this object.
//    public abstract IList<ClrThread> Owners { get; }
//    //
//    // Summary:
//    //     The reason why it's blocking.
//    public abstract BlockingReason Reason { get; internal set; }
//    //
//    // Summary:
//    //     The recursion count of the lock (only valid if Locked is true).
//    public abstract int RecursionCount { get; }
//    //
//    // Summary:
//    //     Whether or not the object is currently locked.
//    public abstract bool Taken { get; }
//    //
//    // Summary:
//    //     Returns the list of threads waiting on this object.
//    public abstract IList<ClrThread> Waiters { get; }
//}
