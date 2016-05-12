using System;
using System.Runtime.InteropServices;

namespace Advapi32
{
    public class Const
    {
        public const int WCT_MAX_NODE_COUNT = 16;
        public const uint WCTP_GETINFO_ALL_FLAGS = 7;
        public const int WCT_OBJNAME_LENGTH = 128;
    }

    public class Functions
    {

        /// <summary>
        ///  doc: https://msdn.microsoft.com/en-us/library/windows/desktop/ms679282(v=vs.85).aspx
        /// </summary>
        /// <param name="WctIntPtr">A IntPtr to the WCT session created by the OpenThreadWaitChainSession function.</param>
        [DllImport("Advapi32.dll", SetLastError = true)]
        public static extern void CloseThreadWaitChainSession(IntPtr WctIntPtr);


        /// <summary>
        ///  Doc : https://msdn.microsoft.com/en-us/library/windows/desktop/ms680543(v=vs.85).aspx
        /// </summary>
        /// <param name="Flags">The session type. This parameter can be one of the following values. (OpenThreadChainFlags)</param>
        /// <param name="callback">If the session is asynchronous, this parameter can be a pointer to a WaitChainCallback callback function.
        /// </param>
        /// <returns>If the function succeeds, the return value is a IntPtr to the newly created session. If the function fails, the return value is NULL.To get extended error information, call GetLastError.
        //</returns>
        [DllImport("Advapi32.dll", SetLastError = true)]
        public static extern IntPtr OpenThreadWaitChainSession(UInt32 Flags, UInt32 callback);


        /// <summary>
        ///  doc: https://msdn.microsoft.com/en-us/library/windows/desktop/ms680564(v=vs.85).aspx
        /// </summary>
        /// <param name="CallStateCallback">The address of the CoGetCallState function.</param>
        /// <param name="ActivationStateCallback">The address of the CoGetActivationState function.</param>
        [DllImport("Advapi32.dll", SetLastError = true)]
        public static extern void RegisterWaitChainCOMCallback(UInt32 CallStateCallback, UInt32 ActivationStateCallback);


        ///  Doc : https://msdn.microsoft.com/en-us/library/windows/desktop/ms679364(v=vs.85).aspx
        [DllImport("Advapi32.dll", SetLastError = true)]
        public static extern bool GetThreadWaitChain(
            IntPtr WctIntPtr,
            IntPtr Context,
            UInt32 Flags,
            uint ThreadId,
            ref int NodeCount,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]
            [In, Out]
            WAITCHAIN_NODE_INFO[] NodeInfoArray,
            out int IsCycle
        );


        /// <summary>
        ///  Doc: https://msdn.microsoft.com/en-us/library/windows/desktop/ms681421(v=vs.85).aspx
        /// </summary>
        /// <param name="WctIntPtr">A IntPtr to the WCT session created by the OpenThreadWaitChainSession function.</param>
        /// <param name="Context">A optional pointer to an application-defined context structure specified by the GetThreadWaitChain function.</param>
        /// <param name="CallbackStatus">The callback status. This parameter can be one of the following values, or one of the other system </param>
        /// <param name="NodeCount">The number of nodes retrieved, up to WCT_MAX_NODE_COUNT. If the array cannot contain all the nodes of the wait chain, the function fails, CallbackStatus is ERROR_MORE_DATA, and this parameter receives the number of array elements required to contain all the nodes.</param>
        /// <param name="NodeInfoArray">An array of WAITCHAIN_NODE_INFO structures that receives the wait chain.</param>
        /// <param name="IsCycle">If the function detects a deadlock, this variable is set to TRUE; otherwise, it is set to FALSE.</param>
        [DllImport("Advapi32.dll", SetLastError = true)]
        public static extern void WaitChainCallback(
           IntPtr WctIntPtr,
           UInt32 Context,
           UInt32 CallbackStatus,
           UInt32 NodeCount,
           UInt32 NodeInfoArray,
           UInt32 IsCycle
        );


        ///Wct Async Callback
        public delegate void AppCallback(
            IntPtr WctHnd,
            IntPtr Context,
            Int32 CallbackStatus,
            Int32 NodeCount,
            IntPtr NodeInfoArray,
            bool IsCycle
        );

        [DllImport("Advapi32.dll", SetLastError = true)]
        public static extern IntPtr OpenThreadWaitChainSession(WCT_SESSION_OPEN_FLAGS Flags, [MarshalAs(UnmanagedType.FunctionPtr)]AppCallback callback);

    }

    public enum CallbackStatus
    {
        /*The caller did not have sufficient privilege to open a target thread.*/
        ERROR_ACCESS_DENIED,
        /*The asynchronous session was canceled by a call to the CloseThreadWaitChainSession function.*/
        ERROR_CANCELLED,
        /*The NodeInfoArray buffer is not large enough to contain all the nodes in the wait chain. The NodeCount parameter contains the number of nodes in the chain. The wait chain returned is still valid.*/
        ERROR_MORE_DATA,
        /*The specified thread could not be located.*/
        ERROR_OBJECT_NOT_FOUND,
        /*The operation completed successfully.*/
        ERROR_SUCCESS,
        /*The number of nodes exceeds WCT_MAX_NODE_COUNT. The wait chain returned is still valid.*/
        ERROR_TOO_MANY_THREADS
    }

    public enum OpenThreadChainFlags
    {
        WCT_OPEN_FLAG = 0,
        WCT_ASYNC_OPEN_FLAG = 1
    }

    public enum GetThreadWaitChainFlags
    {

        /*Enumerates all threads of an out-of-proc MTA COM server 
        to find the correct thread identifier.*/
        WCT_OUT_OF_PROC_COM_FLAG,
        /*Retrieves critical-section information from other processes.*/
        WCT_OUT_OF_PROC_CS_FLAG,
        /*Follows the wait chain into other processes. Otherwise, the function reports the first thread in a different process but does not retrieve additional information.*/
        WCT_OUT_OF_PROC_FLAG
    }

    public enum WCT_OBJECT_TYPE
    {
        WctCriticalSectionType = 1,
        WctSendMessageType = 2,
        WctMutexType = 3,
        WctAlpcType = 4,
        WctComType = 5,
        WctThreadWaitType = 6,
        WctProcessWaitType = 7,
        WctThreadType = 8,
        WctComActivationType = 9,
        WctUnknownType = 10,
        WctMaxType = 11,
    }

    public enum WCT_OBJECT_STATUS
    {
        WctStatusNoAccess = 1,    // ACCESS_DENIED for this object 
        WctStatusRunning = 2,     // Thread status 
        WctStatusBlocked = 3,     // Thread status 
        WctStatusPidOnly = 4,     // Thread status 
        WctStatusPidOnlyRpcss = 5,// Thread status 
        WctStatusOwned = 6,       // Dispatcher object status 
        WctStatusNotOwned = 7,    // Dispatcher object status 
        WctStatusAbandoned = 8,   // Dispatcher object status 
        WctStatusUnknown = 9,     // All objects 
        WctStatusError = 10,      // All objects 
        WctStatusMax = 11
    }

    /// <summary>
    /// Doc: https://msdn.microsoft.com/en-us/library/windows/desktop/ms681388(v=vs.85).aspx
    /// </summary>
    public enum SYSTEM_ERROR_CODES
    {
        /// <summary>
        /// Overlapped I/O operation is in progress. (997 (0x3E5))
        /// </summary>
        ERROR_IO_PENDING = 997
    }

    public enum WCT_SESSION_OPEN_FLAGS
    {
        WCT_SYNC_OPEN_FLAG = 0,
        WCT_ASYNC_OPEN_FLAG = 1
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WAITCHAIN_NODE_INFO
    {
        public WCT_OBJECT_TYPE ObjectType;
        public WCT_OBJECT_STATUS ObjectStatus;
        public _WAITCHAIN_NODE_INFO_UNION Union;
    }


    [StructLayout(LayoutKind.Explicit)]
    public struct _WAITCHAIN_NODE_INFO_UNION
    {
        [FieldOffset(0)]
        public _WAITCHAIN_NODE_INFO_LOCK_OBJECT LockObject;
        [FieldOffset(0)]
        public _WAITCHAIN_NODE_INFO_THREAD_OBJECT ThreadObject;
    }

    public unsafe struct _WAITCHAIN_NODE_INFO_LOCK_OBJECT
    {
        /*The name of the object. Object names are only available for certain object, such as mutexes. If the object does not have a name, this member is an empty string.*/
        public fixed byte ObjectName[Const.WCT_OBJNAME_LENGTH * 2];
        /*This member is reserved for future use.*/
        public UInt64 Timeout;
        /*This member is reserved for future use.*/
        public UInt32 Alertable;
    }

    public struct _WAITCHAIN_NODE_INFO_THREAD_OBJECT
    {
        /*The process identifier.*/
        public UInt32 ProcessId;
        /*The thread identifier. For COM and ALPC, this member can be 0.*/
        public UInt32 ThreadId;
        /*The wait time.*/
        public UInt32 WaitTime;
        /*The number of context switches.*/
        public UInt32 ContextSwitches;

    }
}
