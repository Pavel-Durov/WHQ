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
using Assignments.Core.Model;

namespace Assignments.Core.Handlers.WCT
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
        const int WCT_MAX_NODE_COUNT = 16;
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

            uint threadID = thread.OSThreadId;

            WAITCHAIN_NODE_INFO[] NodeInfoArray = new WAITCHAIN_NODE_INFO[WCT_MAX_NODE_COUNT];


            int isCycle = 0;
            int count = WCT_MAX_NODE_COUNT;

            // Make a synchronous WCT call to retrieve the wait chain.
            bool result = GetThreadWaitChain(g_WctHandle,
                                    IntPtr.Zero,
                                    WCTP_GETINFO_ALL_FLAGS,
                                    threadID, ref count, NodeInfoArray, out isCycle);

            if (!result)
            {
                //error
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
        ///  Original Doc: https://msdn.microsoft.com/en-us/library/windows/desktop/ms679360(v=vs.85).aspx
        ///  System errr codes: https://msdn.microsoft.com/en-us/library/windows/desktop/ms681381(v=vs.85).aspx
        /// </summary>
        /// <returns>The return value is the calling thread's last-error code.</returns>
        [DllImport("Kernel32.dll")]
        public static extern DWORD GetLastError();


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

        #endregion
    }
}
