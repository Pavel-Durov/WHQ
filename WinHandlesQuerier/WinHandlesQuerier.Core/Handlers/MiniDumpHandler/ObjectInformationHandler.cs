using System;
using System.Collections.Generic;
using WinHandlesQuerier.Core.Model.MiniDump;
using DbgHelp;
using Structs;

namespace WinHandlesQuerier.Core.Handlers.MiniDump
{
    /// <summary>
    /// Extract relevant information from address to miniDump Handle properties
    /// </summary>
    public class ObjectInformationHandler
    {
        /// <summary>
        /// Reads  sctructure form given handle if possible
        /// </summary>
        /// <param name="pObjectInfo">object info struct</param>
        /// <param name="handle">context handle</param>
        /// <param name="address">calculate rva address</param>
        /// <param name="baseOfView">base of mapped minidump file</param>
        /// <returns>Information structure or default value if no info detected</returns>
        public static unsafe MINIDUMP_HANDLE_OBJECT_INFORMATION DealWithHandleInfo(MINIDUMP_HANDLE_OBJECT_INFORMATION pObjectInfo, MiniDumpHandle handle, IntPtr address, IntPtr baseOfView)
        {
            Action<MiniDumpHandle, IntPtr> action = null;

            if (_actions.TryGetValue(pObjectInfo.InfoType, out action))
            {
                action(handle, address);

                if (pObjectInfo.NextInfoRva == 0)
                {
                    return default(MINIDUMP_HANDLE_OBJECT_INFORMATION);
                }
                else
                {
                    pObjectInfo = SafeMemoryMappedViewStreamHandler
                        .ReadStruct<MINIDUMP_HANDLE_OBJECT_INFORMATION>(IntPtr.Add(baseOfView, (int)pObjectInfo.NextInfoRva));
                }
            }

            return pObjectInfo;
        }
        /// <summary>
        /// Initializes the required actions with types as keys (MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE) in local Ductionary for MiniDump handle information handling
        /// </summary>
        static ObjectInformationHandler()
        {
            _actions = new Dictionary<MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE, Action<MiniDumpHandle, IntPtr>>();

            _actions[MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniHandleObjectInformationNone] = (handle, address) => { handle.Type = MiniDumpHandleType.NONE; };

            _actions[MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniThreadInformation1] = SetMiniThreadInformation1;

            _actions[MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniMutantInformation1] = SetMiniMutantInformation1;

            _actions[MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniMutantInformation2] = SetMiniMutantInformation2;

            _actions[MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniProcessInformation1] = SetMiniProcessInformation1;

            _actions[MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniProcessInformation2] = SetMiniProcessInformation2;

            _actions[MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniEventInformation1] = (handle, address) => { handle.Type = MiniDumpHandleType.EVENT; };

            _actions[MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniSectionInformation1] = (handle, address) => { handle.Type = MiniDumpHandleType.SECTION; };

            _actions[MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniHandleObjectInformationTypeMax] = (handle, address) => { handle.Type = MiniDumpHandleType.TYPE_MAX; };
        }

        /// <summary>
        /// Ductionary of actions with enums as types - used for extracting relevant data from given address to the referenced MiniDumpHandle properties.
        /// </summary>
        static Dictionary<MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE, Action<MiniDumpHandle, IntPtr>> _actions;
        Action<MiniDumpHandle, uint> HandleInfoTypeAction;

        #region Actions

        private static unsafe void SetMiniProcessInformation2(MiniDumpHandle handle, IntPtr address)
        {
            handle.Type = MiniDumpHandleType.PROCESS2;
            PROCESS_ADDITIONAL_INFO_2* pInfo = (PROCESS_ADDITIONAL_INFO_2*)(((char*)address) + sizeof(MINIDUMP_HANDLE_OBJECT_INFORMATION));

            handle.OwnerProcessId = pInfo->ProcessId;
            handle.OwnerThreadId = 0;
        }

        private static void SetMiniProcessInformation1(MiniDumpHandle handle, IntPtr address)
        {
            handle.Type = MiniDumpHandleType.PROCESS1;
        }

        private static unsafe void SetMiniMutantInformation2(MiniDumpHandle handle, IntPtr address)
        {
            handle.Type = MiniDumpHandleType.MUTEX2;

            MUTEX_ADDITIONAL_INFO_2* mutexInfo2 = (MUTEX_ADDITIONAL_INFO_2*)(((char*)address) + sizeof(MINIDUMP_HANDLE_OBJECT_INFORMATION));

            handle.OwnerProcessId = mutexInfo2->OwnerProcessId;
            handle.OwnerThreadId = mutexInfo2->OwnerThreadId;
        }

        private static unsafe void SetMiniMutantInformation1(MiniDumpHandle handle, IntPtr address)
        {
            handle.Type = MiniDumpHandleType.MUTEX1;

            MUTEX_ADDITIONAL_INFO_1* mutexInfo1 = (MUTEX_ADDITIONAL_INFO_1*)(((char*)address) + sizeof(MINIDUMP_HANDLE_OBJECT_INFORMATION));

            handle.MutexUnknown = new MutexUnknownFields()
            {
                Field1 = mutexInfo1->Unknown1,
                Field2 = mutexInfo1->Unknown2
            };
        }

        private static unsafe void SetMiniThreadInformation1(MiniDumpHandle handle, IntPtr address)
        {
            handle.Type = MiniDumpHandleType.THREAD;

            THREAD_ADDITIONAL_INFO* threadInfo = (THREAD_ADDITIONAL_INFO*)(((char*)address) + sizeof(MINIDUMP_HANDLE_OBJECT_INFORMATION));

            handle.OwnerProcessId = threadInfo->ProcessId;
            handle.OwnerThreadId = threadInfo->ThreadId;
        }

        #endregion

    }
}
