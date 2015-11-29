using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HANDLE = System.IntPtr;
using DWORD = System.Double;
using LARGE_INTEGER = System.UInt64;
using BOOL = System.Boolean;

namespace Assignment_4.Model
{
    /// <summary>
    /// about: https://msdn.microsoft.com/en-us/library/windows/desktop/ms681622(v=vs.85).aspx
    /// use: https://msdn.microsoft.com/en-us/library/windows/desktop/ms681418(v=vs.85).aspx
    /// </summary>
    public class WctApi
    {
        public WctApi()
        {
            //https://msdn.microsoft.com/en-us/library/cc308564.aspx
            //Debugging with WTC 

            //1.Create a WTC session.
            //2.Get the processes and threads to analyze.
            //3.Get the wait chain for each thread.
            //4.Close the WTC Session

            DebuggingSteps();
        }

        private void DebuggingSteps()
        {
            var wctHandle = OpenThreadWaitChainSession(0, 0);
            if (wctHandle == HANDLE.Zero)
            {
                //Error opening wait chain session
            }
            else
            {

            }

        }


        private HANDLE OpenSession()
        {
            var wctHandle = OpenThreadWaitChainSession(0, 0);

            if (wctHandle == HANDLE.Zero)
            {
                //Error opening wait chain session
            }

            return wctHandle;
        }

        private void ClosseSession(HANDLE wctHandle)
        {
            CloseThreadWaitChainSession(wctHandle);
        }


        #region External Advapi32 calls


        //VOID WINAPI CloseThreadWaitChainSession(_In_ HWCT WctHandle);
        [DllImport("Advapi32.dll")]
        public static extern void CloseThreadWaitChainSession(HANDLE WctHandle);


        //HWCT WINAPI OpenThreadWaitChainSession(_In_ DWORD  Flags, _In_opt_ PWAITCHAINCALLBACK callback);
        [DllImport("Advapi32.dll")]
        public static extern HANDLE OpenThreadWaitChainSession(double Flags, double callback);



        //VOID WINAPI RegisterWaitChainCOMCallback(_In_ PCOGETCALLSTATE CallStateCallback,
        //_In_ PCOGETACTIVATIONSTATE ActivationStateCallback);



        enum WCT_OBJECT_TYPE
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

        enum WCT_OBJECT_STATUS
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

        [StructLayout(LayoutKind.Sequential)]
        private struct _WAITCHAIN_NODE_INFO
        {
            WCT_OBJECT_TYPE ObjectType;
            WCT_OBJECT_STATUS ObjectStatus;


            struct LockObject
            {
                /*The name of the object. Object names are only available for certain object, such as mutexes. If the object does not have a name, this member is an empty string.*/
                string ObjectName;
                /*This member is reserved for future use.*/
                LARGE_INTEGER Timeout;
                /*This member is reserved for future use.*/
                BOOL Alertable;
            }

            struct ThreadObject
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


    

        #endregion

        //BOOL WINAPI GetThreadWaitChain(
        //_In_ HWCT                 WctHandle,
        //_In_opt_ DWORD_PTR            Context,
        //_In_ DWORD                Flags,
        //_In_ DWORD                ThreadId,
        //_Inout_ LPDWORD              NodeCount,
        //_Out_ PWAITCHAIN_NODE_INFO NodeInfoArray,
        //_Out_ LPBOOL               IsCycle

        //);


    }
}
