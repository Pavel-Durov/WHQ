using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignments.Core.Handlers.WCT
{

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
    /// <summary>
    /// Doc: http://winappdbg.sourceforge.net/doc/v1.4/reference/winappdbg.win32.advapi32-pysrc.html
    /// </summary>
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



}
