using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

//Maping native types to managed 
using HANDLE = System.IntPtr;
using DWORD = System.UInt32;
using LPBOOL = System.UInt32;
using LARGE_INTEGER = System.UInt64;
using BOOL = System.UInt32;
using LPDWORD = System.UInt32;
using HWCT = System.IntPtr;
using DWORD_PTR = System.UInt32;
using PWAITCHAIN_NODE_INFO = System.UInt32;
using Microsoft.Diagnostics.Runtime;
using System.Diagnostics;

namespace Assignments.Core.Handlers
{
    /// <summary>
    /// about: https://msdn.microsoft.com/en-us/library/windows/desktop/ms681622(v=vs.85).aspx
    /// use: https://msdn.microsoft.com/en-us/library/windows/desktop/ms681418(v=vs.85).aspx
    /// </summary>
    public class WctApiHandler
    {
        public WctApiHandler()
        {
            //https://msdn.microsoft.com/en-us/library/cc308564.aspx
            //TODO: Debugging with WTC 

            //1.Create a WTC session.
            //2.Get the processes and threads to analyze.
            //3.Get the wait chain for each thread.
            //4.Close the WTC Session
        }

        //http://winappdbg.sourceforge.net/doc/v1.4/reference/winappdbg.win32.advapi32-module.html
        const uint WCT_MAX_NODE_COUNT = 16;
        const uint WCTP_GETINFO_ALL_FLAGS = 7;

        public object Debbuger { get; private set; }


        #region CollectWaitInformation C++ implementation
        //Source: https://msdn.microsoft.com/en-us/library/windows/desktop/ms681418(v=vs.85).aspx

        /*            
         WAITCHAIN_NODE_INFO NodeInfoArray[WCT_MAX_NODE_COUNT];
         DWORD Count, i;
         BOOL IsCycle;

         printf("%d: ", ThreadId);

         Count = WCT_MAX_NODE_COUNT;

         // Make a synchronous WCT call to retrieve the wait chain.
         if (!GetThreadWaitChain(g_WctHandle,NULL,WCTP_GETINFO_ALL_FLAGS,ThreadId,&Count,NodeInfoArray,&IsCycle))
         {
             printf("Error (0X%x)\n", GetLastError());
             return;
         }

         // Check if the wait chain is too big for the array we passed in.
         if (Count > WCT_MAX_NODE_COUNT)
         {
             printf("Found additional nodes %d\n", Count);
             Count = WCT_MAX_NODE_COUNT;
         }

         // Loop over all the nodes returned and print useful information.
         for (i = 0; i < Count; i++)
         {
             switch (NodeInfoArray[i].ObjectType)
             {
                 case WctThreadType:
                     // A thread node contains process and thread ID.
                     printf("[%x:%x:%s]->",
                            NodeInfoArray[i].ThreadObject.ProcessId,
                            NodeInfoArray[i].ThreadObject.ThreadId,
                            ((NodeInfoArray[i].ObjectStatus == WctStatusBlocked) ? "b" : "r"));
                     break;

                 default:
                     // A synchronization object.

                     // Some objects have names...
                     if (NodeInfoArray[i].LockObject.ObjectName[0] != L'\0')
         {
             printf("[%s:%S]->",
                    STR_OBJECT_TYPE[NodeInfoArray[i].ObjectType - 1].Desc,
                    NodeInfoArray[i].LockObject.ObjectName);
         }
         else
         {
             printf("[%s]->",
                    STR_OBJECT_TYPE[NodeInfoArray[i].ObjectType - 1].Desc);
         }
         if (NodeInfoArray[i].ObjectStatus == WctStatusAbandoned)
         {
             printf("<abandoned>");
         }
         break;
     }
 }


*/

        #endregion

        internal void CollectWaitInformation(ClrThread thread)
        {
            var g_WctHandle = OpenThreadWaitChainSession(0, 0);

            if (g_WctHandle == HANDLE.Zero)
            {
                //Error opening wait chain session
            }

            uint threadID = thread.OSThreadId;

            WAITCHAIN_NODE_INFO[] NodeInfoArray = new WAITCHAIN_NODE_INFO[WCT_MAX_NODE_COUNT];
            
            //byte[] array = null;
            //IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(NodeInfoArray));
            //Marshal.Copy(byteArray, 0, intPtr, Marshal.SizeOf(NodeInfoArray));

            //Func(intPtr);

            //Marshal.FreeHGlobal(intPtr);


            //HANDLE unmanagedPointer = Marshal.AllocHGlobal(NodeInfoArray.Length);

            // Call unmanaged code
            //Marshal.FreeHGlobal(unmanagedPointer);


            
            int isCycle = 0;
            int count = 0;

            // Make a synchronous WCT call to retrieve the wait chain.
            bool result = GetThreadWaitChain(g_WctHandle,
                                    IntPtr.Zero,
                                    WCTP_GETINFO_ALL_FLAGS,
                                    threadID, ref count, NodeInfoArray, out isCycle);

            if (result)
            {

                //error
            }
            else
            {
                //OK
            }

            //Finaly ...
            CloseSession(g_WctHandle);
        }


        private void CloseSession(HANDLE wctHandle)
        {
            CloseThreadWaitChainSession(wctHandle);
        }


        #region External Advapi32 calls


        /// <summary>
        /// Original doc: https://msdn.microsoft.com/en-us/library/windows/desktop/ms679282(v=vs.85).aspx
        /// </summary>
        /// <param name="WctHandle">A handle to the WCT session created by the OpenThreadWaitChainSession function.</param>
        [DllImport("Advapi32.dll")]
        public static extern void CloseThreadWaitChainSession(HANDLE WctHandle);



        /// <summary>
        /// Original Doc : https://msdn.microsoft.com/en-us/library/windows/desktop/ms680543(v=vs.85).aspx
        /// </summary>
        /// <param name="Flags">The session type. This parameter can be one of the following values.</param>
        /// <param name="callback">If the session is asynchronous, this parameter can be a pointer to a WaitChainCallback callback function.
        /// </param>
        /// <returns>If the function succeeds, the return value is a handle to the newly created session. If the function fails, the return value is NULL.To get extended error information, call GetLastError.
        //</returns>
        [DllImport("Advapi32.dll")]
        public static extern HANDLE OpenThreadWaitChainSession(OpenThreadChainFlags Flags, DWORD callback);


        /// <summary>
        /// Original doc: https://msdn.microsoft.com/en-us/library/windows/desktop/ms680564(v=vs.85).aspx
        /// </summary>
        /// <param name="CallStateCallback">The address of the CoGetCallState function.</param>
        /// <param name="ActivationStateCallback">The address of the CoGetActivationState function.</param>
        [DllImport("Advapi32.dll")]
        public static extern void RegisterWaitChainCOMCallback(UInt32 CallStateCallback, UInt32 ActivationStateCallback);


        /// <summary>
        /// Original Doc : https://msdn.microsoft.com/en-us/library/windows/desktop/ms679364(v=vs.85).aspx
        /// </summary>
        /// <param name="WctHandle"></param>
        /// <param name="Context"></param>
        /// <param name="flags"></param>
        /// <param name="ThreadId"></param>
        /// <param name="NodeCount"></param>
        /// <param name="NodeInfoArray"></param>
        /// <param name="IsCycle"></param>
        /// <returns></returns>
        [DllImport("Advapi32.dll")]
        public static extern bool GetThreadWaitChain(
            IntPtr WctHandle,
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
        /// Original Doc: https://msdn.microsoft.com/en-us/library/windows/desktop/ms681421(v=vs.85).aspx
        /// </summary>
        /// <param name="WctHandle">A handle to the WCT session created by the OpenThreadWaitChainSession function.</param>
        /// <param name="Context">A optional pointer to an application-defined context structure specified by the GetThreadWaitChain function.</param>
        /// <param name="CallbackStatus">The callback status. This parameter can be one of the following values, or one of the other system </param>
        /// <param name="NodeCount">The number of nodes retrieved, up to WCT_MAX_NODE_COUNT. If the array cannot contain all the nodes of the wait chain, the function fails, CallbackStatus is ERROR_MORE_DATA, and this parameter receives the number of array elements required to contain all the nodes.</param>
        /// <param name="NodeInfoArray">An array of WAITCHAIN_NODE_INFO structures that receives the wait chain.</param>
        /// <param name="IsCycle">If the function detects a deadlock, this variable is set to TRUE; otherwise, it is set to FALSE.</param>
        [DllImport("Advapi32.dll")]
        public static extern void WaitChainCallback(
           HWCT WctHandle,
           DWORD_PTR Context,
           DWORD CallbackStatus,
           LPDWORD NodeCount,
           PWAITCHAIN_NODE_INFO NodeInfoArray,
           LPBOOL IsCycle
        );


        [StructLayout(LayoutKind.Sequential)]
        public struct WAITCHAIN_NODE_INFO
        {
            public WCT_OBJECT_TYPE ObjectType;
            public WCT_OBJECT_STATUS ObjectStatus;


            public struct LockObject
            {
                /*The name of the object. Object names are only available for certain object, such as mutexes. If the object does not have a name, this member is an empty string.*/
                string ObjectName;
                /*This member is reserved for future use.*/
                LARGE_INTEGER Timeout;
                /*This member is reserved for future use.*/
                BOOL Alertable;
            }

            public struct ThreadObject
            {
                /*The process identifier.*/
                DWORD ProcessId;
                /*The thread identifier. For COM and ALPC, this member can be 0.*/
                DWORD ThreadId;
                /*The wait time.*/
                DWORD WaitTime;
                /*The number of context switches.*/
                DWORD ContextSwitches;
            }
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
            WctCriticalSectionType,
            WctSendMessageType,
            WctMutexType,
            WctAlpcType,
            WctComType,
            WctThreadWaitType,
            WctProcessWaitType,
            WctThreadType,
            WctComActivationType,
            WctUnknownType
        }

        public enum WCT_OBJECT_STATUS
        {
            WctStatusNoAccess,
            WctStatusRunning,
            WctStatusBlocked,
            WctStatusPidOnly,
            WctStatusPidOnlyRpcss,
            WctStatusOwned,
            WctStatusNotOwned,
            WctStatusAbandoned,
            WctStatusUnknown,
            WctStatusError
        }





        #endregion



    }
}
