using System;
using System.Runtime.InteropServices;
using Assignments.Core.Model.WCT;
using Assignments.Core.WinApi;

namespace Assignments.Core.Handlers
{
    public class WctApiHandler
    {
        internal bool GetBlockingObjects(uint threadId, out ThreadWCTInfo info)
        {
            info = CollectWaitInformation(threadId);
            return info.WctBlockingObjects?.Count > 0;
        }

        internal ThreadWCTInfo CollectWaitInformation(uint threadId)
        {
            ThreadWCTInfo result = null;

            var g_WctIntPtr = Advapi32.OpenThreadWaitChainSession((int)Advapi32.WCT_SESSION_OPEN_FLAGS.WCT_SYNC_OPEN_FLAG, 0);


            Advapi32.WAITCHAIN_NODE_INFO[] NodeInfoArray = new Advapi32.WAITCHAIN_NODE_INFO[Advapi32.WCT_MAX_NODE_COUNT];


            int isCycle = 0;
            int Count = Advapi32.WCT_MAX_NODE_COUNT;

            // Make a synchronous WCT call to retrieve the wait chain.
            bool waitChainResult = Advapi32.GetThreadWaitChain(g_WctIntPtr,
                                    IntPtr.Zero,
                                    Advapi32.WCTP_GETINFO_ALL_FLAGS,
                                    threadId, ref Count, NodeInfoArray, out isCycle);

            CheckCount(ref Count);

            if (waitChainResult)
            {
                result = HandleGetThreadWaitChainRsult(threadId, Count, NodeInfoArray, isCycle);
            }
            else
            {
                HandleWctRequestError(g_WctIntPtr);
            }

            //Finaly ...
            Advapi32.CloseThreadWaitChainSession(g_WctIntPtr);

            return result;
        }

        
        private ThreadWCTInfo HandleGetThreadWaitChainRsult(uint threadId, int Count, Advapi32.WAITCHAIN_NODE_INFO[] NodeInfoArray, int isCycle)
        {
            ThreadWCTInfo result = new ThreadWCTInfo(isCycle == 1, threadId);
            Advapi32.WAITCHAIN_NODE_INFO[] info = new Advapi32.WAITCHAIN_NODE_INFO[Count];
            Array.Copy(NodeInfoArray, info, Count);

            result.SetInfo(info);

            return result;
        }


        private void HandleWctRequestError(IntPtr g_WctIntPtr)
        {
            var lastErrorCode = Marshal.GetLastWin32Error();

            if (lastErrorCode == (uint)Advapi32.SYSTEM_ERROR_CODES.ERROR_IO_PENDING)
            {
                //TODO: Follow this doc: https://msdn.microsoft.com/en-us/library/windows/desktop/ms681421(v=vs.85).aspx
            }
            else
            {
                //TODO : Ifdentify code error and responce accordingly
            }
        }

        private void CheckCount(ref int Count)
        {
            // Check if the wait chain is too big for the array we passed in.
            if (Count > Advapi32.WCT_MAX_NODE_COUNT)
            {
                //Found additional nodes 
                Count = Advapi32.WCT_MAX_NODE_COUNT;
            }
        }
    }
}
