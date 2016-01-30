﻿using System;
using System.Runtime.InteropServices;
using Microsoft.Diagnostics.Runtime;
using Assignments.Core.Model.WCT;
using Assignments.Core.WinApi;
using Assignments.Core.Model.Unified;
using System.Collections.Generic;

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




        #region WCT Async Call

        //IntPtr _eventHandler;

        ///// <summary>
        ///// Causes process hang 
        ///// </summary>
        ///// <param name="thread"></param>
        //internal void CollectWaitAsyncInformation(ClrThread thread)
        //{
            
        //    var handle = Advapi32.OpenThreadWaitChainSession(WCT_SESSION_OPEN_FLAGS.WCT_ASYNC_OPEN_FLAG, AppCallback);

        //    uint threadID = thread.OSThreadId;

        //    WAITCHAIN_NODE_INFO[] NodeInfoArray = new WAITCHAIN_NODE_INFO[WctApiConst.WCT_MAX_NODE_COUNT];

        //    int isCycle = 0;
        //    int Count = WctApiConst.WCT_MAX_NODE_COUNT;

        //    _eventHandler = Kernel32.CreateEvent(IntPtr.Zero, true, true, "MyEvent");

        //    //This is where the applciation hangs
        //    bool waitChainResult = Advapi32.GetThreadWaitChain(handle,
        //                            _eventHandler, 0,
        //                            threadID, ref Count, NodeInfoArray, out isCycle);

        //    CheckCount(ref Count);

        //    if (!waitChainResult)
        //    {

        //        var lastErrorCode = WinApi.Advapi32.GetLastError();

        //        if (lastErrorCode == (uint)SYSTEM_ERROR_CODES.ERROR_IO_PENDING)
        //        {
        //            // Wait for callback to run...
        //            WinApi.Kernel32.WaitForSingleObject(_eventHandler, int.MaxValue);
        //        }
        //        Kernel32.WaitForSingleObject(_eventHandler, uint.MaxValue);
        //    }
        //}

        ///////////////////////////////////////////////////////////
        //// WCT Async CallBack implementation

        //static void AppCallback(IntPtr WctHnd, IntPtr Context, Int32 CallbackStatus, Int32 NodeCount, IntPtr NodeInfoArray, bool IsCycle)
        //{

        //}

        #endregion
    }
}