using System;
using System.Runtime.InteropServices;
using WinHandlesQuerier.Core.Model.WCT;
using Advapi32;

namespace WinHandlesQuerier.Core.Handlers
{
    /// <summary>
    /// Exract blocking objects information using WCT api:
    /// doc: https://msdn.microsoft.com/en-us/library/windows/desktop/ms681622(v=vs.85).aspx
    /// </summary>
    public class WctHandler
    {
        /// <summary>
        /// Gets WCT information using WCT api by the given threadId (OS threadId)
        /// </summary>
        /// <param name="threadId"> ThreadWCTInfo  object</param>
        /// <returns>Thread id of thread target</returns>
        public ThreadWCTInfo GetBlockingObjects(uint threadId)
        {
            ThreadWCTInfo result = null;

            var g_WctIntPtr = Functions.OpenThreadWaitChainSession((int)WCT_SESSION_OPEN_FLAGS.WCT_SYNC_OPEN_FLAG, 0);


            WAITCHAIN_NODE_INFO[] NodeInfoArray = new WAITCHAIN_NODE_INFO[Const.WCT_MAX_NODE_COUNT];


            int isCycle = 0;
            int Count = Const.WCT_MAX_NODE_COUNT;

            // Make a synchronous WCT call to retrieve the wait chain.
            bool waitChainResult = Functions.GetThreadWaitChain(g_WctIntPtr,
                                    IntPtr.Zero,
                                    Const.WCTP_GETINFO_ALL_FLAGS,
                                    threadId, ref Count, NodeInfoArray, out isCycle);

            CheckCount(ref Count);

            if (waitChainResult)
            {
                result = HandleGetThreadWaitChainRsult(threadId, Count, NodeInfoArray, isCycle);
            }
            else
            {
                HandleWctRequestError();
            }

            //Finaly ...
            Functions.CloseThreadWaitChainSession(g_WctIntPtr);

            return result;
        }

        
        private ThreadWCTInfo HandleGetThreadWaitChainRsult(uint threadId, int Count, WAITCHAIN_NODE_INFO[] NodeInfoArray, int isCycle)
        {
            ThreadWCTInfo result = new ThreadWCTInfo(isCycle == 1, threadId);
            WAITCHAIN_NODE_INFO[] info = new WAITCHAIN_NODE_INFO[Count];
            Array.Copy(NodeInfoArray, info, Count);

            result.SetInfo(info);

            return result;
        }


        private void HandleWctRequestError()
        {
            var lastErrorCode = Marshal.GetLastWin32Error();

            if (lastErrorCode == (uint)SYSTEM_ERROR_CODES.ERROR_IO_PENDING)
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
            if (Count > Const.WCT_MAX_NODE_COUNT)
            {
                //Found additional nodes 
                Count = Const.WCT_MAX_NODE_COUNT;
            }
        }
    }
}
