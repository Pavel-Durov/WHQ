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



}
