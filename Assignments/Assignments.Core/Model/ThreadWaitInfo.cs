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
            Thread = thread;
        }

        #region Properties + Data members

       public  ClrThread Thread { get; private set; }

        List<BlockingObject> _blockingObjects;

        //public abstract IList<uint> Waiters { get; }

        public List<BlockingObject> WctBlockingObjects
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



        internal void SetInfo(WAITCHAIN_NODE_INFO[] info)
        {
            if(info != null)
            {
                foreach (var item in info)
                {
                    var block = new BlockingObject(item);
                    WctBlockingObjects.Add(block);
                }
            }
            //TODO: Fill the relevant information 

            //WctBlockingObjects.
        }
    }
}
//          BlockingObject API
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





//
// Summary:
//     Represents a managed thread in the target process. Note this does not wrap purely
//     native threads in the target process (that is, threads which have never run managed
//     code before).
//public abstract class ClrThread
//{
//    protected ClrThread();




//    //
//    // Summary:
//    //     Returns the exception currently on the thread. Note that this field may be null.
//    //     Also note that this is basically the "last thrown exception", and may be stale...meaning
//    //     the thread could be done processing the exception but a crash dump was taken
//    //     before the current exception was cleared off the field.
//    public abstract ClrException CurrentException { get; }
//    //
//    // Summary:
//    //     The suspension state of the thread according to the runtime.
//    public abstract GcMode GcMode { get; }
//    //
//    // Summary:
//    //     Returns true if this thread was aborted.
//    public abstract bool IsAborted { get; }
//    //
//    // Summary:
//    //     Returns true if an abort was requested for this thread (such as Thread.Abort,
//    //     or AppDomain unload).
//    public abstract bool IsAbortRequested { get; }
//    //
//    // Summary:
//    //     Returns true if the thread is alive in the process, false if this thread was
//    //     recently terminated.
//    public abstract bool IsAlive { get; }
//    //
//    // Summary:
//    //     Returns true if this thread is a background thread. (That is, if the thread does
//    //     not keep the managed execution environment alive and running.)
//    public abstract bool IsBackground { get; }
//    //
//    // Summary:
//    //     Returns true if the Clr runtime called CoIntialize for this thread.
//    public abstract bool IsCoInitialized { get; }
//    //
//    // Summary:
//    //     Returns if this thread is the debugger helper thread.
//    public abstract bool IsDebuggerHelper { get; }
//    //
//    // Summary:
//    //     Returns true if the debugger has suspended the thread.
//    public abstract bool IsDebugSuspended { get; }
//    //
//    // Summary:
//    //     Returns true if this is the finalizer thread.
//    public abstract bool IsFinalizer { get; }
//    //
//    // Summary:
//    //     Returns if this thread is a GC thread. If the runtime is using a server GC, then
//    //     there will be dedicated GC threads, which this will indicate. For a runtime using
//    //     the workstation GC, this flag will only be true for a thread which is currently
//    //     running a GC (and the background GC thread).
//    public abstract bool IsGC { get; }
//    //
//    // Summary:
//    //     Returns true if the GC is attempting to suspend this thread.
//    public abstract bool IsGCSuspendPending { get; }
//    //
//    // Summary:
//    //     Returns true if the thread is a COM multithreaded apartment.
//    public abstract bool IsMTA { get; }
//    //
//    // Summary:
//    //     Returns true if this thread is currently the thread shutting down the runtime.
//    public abstract bool IsShutdownHelper { get; }
//    //
//    // Summary:
//    //     Returns true if this thread is in a COM single threaded apartment.
//    public abstract bool IsSTA { get; }
//    //
//    // Summary:
//    //     Returns if this thread currently suspending the runtime.
//    public abstract bool IsSuspendingEE { get; }
//    //
//    // Summary:
//    //     Returns true if this thread is a threadpool IO completion port.
//    public abstract bool IsThreadpoolCompletionPort { get; }
//    //
//    // Summary:
//    //     Returns true if this is the threadpool gate thread.
//    public abstract bool IsThreadpoolGate { get; }
//    //
//    // Summary:
//    //     Returns true if this thread is a threadpool timer thread.
//    public abstract bool IsThreadpoolTimer { get; }
//    //
//    // Summary:
//    //     Returns true if this is a threadpool wait thread.
//    public abstract bool IsThreadpoolWait { get; }
//    //
//    // Summary:
//    //     Returns true if this is a threadpool worker thread.
//    public abstract bool IsThreadpoolWorker { get; }
//    //
//    // Summary:
//    //     Returns true if this thread was created, but not started.
//    public abstract bool IsUnstarted { get; }
//    //
//    // Summary:
//    //     Returns true if the user has suspended the thread (using Thread.Suspend).
//    public abstract bool IsUserSuspended { get; }
//    //
//    // Summary:
//    //     The number of managed locks (Monitors) the thread has currently entered but not
//    //     left. This will be highly inconsistent unless the process is stopped.
//    public abstract uint LockCount { get; }
//    //
//    // Summary:
//    //     The managed thread ID (this is equivalent to System.Threading.Thread.ManagedThreadId
//    //     in the target process).
//    public abstract int ManagedThreadId { get; }
//    //

//    //
//    // Summary:
//    //     Gets the runtime associated with this thread.
//    public abstract ClrRuntime Runtime { get; }
//    //
//    // Summary:
//    //     The base of the stack for this thread, or 0 if the value could not be obtained.
//    public abstract ulong StackBase { get; }
//    //
//    // Summary:
//    //     The limit of the stack for this thread, or 0 if the value could not be obtained.
//    public abstract ulong StackLimit { get; }
//    //
//    // Summary:
//    //     Returns the managed stack trace of the thread. Note that this property may return
//    //     incomplete data in the case of a bad stack unwind or if there is a very large
//    //     number of methods on the stack. (This is usually caused by a stack overflow on
//    //     the target thread, stack corruption which leads to a bad stack unwind, or other
//    //     inconsistent state in the target debuggee.) Note: This property uses a heuristic
//    //     to attempt to detect bad unwinds to stop enumerating frames by inspecting the
//    //     stack pointer and instruction pointer of each frame to ensure the stack walk
//    //     is "making progress". Additionally we cap the number of frames returned by this
//    //     method as another safegaurd. This means we may not have all frames even if the
//    //     stack walk was making progress. If you want to ensure that you receive an un-clipped
//    //     stack trace, you should use EnumerateStackTrace instead of this property, and
//    //     be sure to handle the case of repeating stack frames.
//    public abstract IList<ClrStackFrame> StackTrace { get; }
//    //
//    // Summary:
//    //     The TEB (thread execution block) address in the process.
//    public abstract ulong Teb { get; }

//    //
//    // Summary:
//    //     Enumerates the GC references (objects) on the stack. This is equivalent to EnumerateStackObjects(true).
//    //
//    // Returns:
//    //     An enumeration of GC references on the stack as the GC sees them.
//    public abstract IEnumerable<ClrRoot> EnumerateStackObjects();
//    //
//    // Summary:
//    //     Enumerates the GC references (objects) on the stack.
//    //
//    // Parameters:
//    //   includePossiblyDead:
//    //     Include all objects found on the stack. Passing false attempts to replicate the
//    //     behavior of the GC, reporting only live objects.
//    //
//    // Returns:
//    //     An enumeration of GC references on the stack as the GC sees them.
//    public abstract IEnumerable<ClrRoot> EnumerateStackObjects(bool includePossiblyDead);
//    //
//    // Summary:
//    //     Enumerates a stack trace for a given thread. Note this method may loop infinitely
//    //     in the case of stack corruption or other stack unwind issues which can happen
//    //     in practice. When enumerating frames out of this method you should be careful
//    //     to either set a maximum loop count, or to ensure the stack unwind is making progress
//    //     by ensuring that ClrStackFrame.StackPointer is making progress (though it is
//    //     expected that sometimes two frames may return the same StackPointer in some corner
//    //     cases).
//    //
//    // Returns:
//    //     An enumeration of stack frames.
//    public abstract IEnumerable<ClrStackFrame> EnumerateStackTrace();
//}
