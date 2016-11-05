using System;
using WHQ.Core.Model.MiniDump;
using DbgHelp;
using Structs;
using System.Runtime.InteropServices;

namespace WHQ.Core.Handlers.MiniDump
{
    /// <summary>
    /// Extract relevant object information from address to miniDump Handle properties
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
        public static unsafe MINIDUMP_HANDLE_OBJECT_INFORMATION DealWithHandleInfo(MINIDUMP_HANDLE_OBJECT_INFORMATION pObjectInfo, DumpHandle handle, IntPtr address, IntPtr baseOfView)
        {
            switch (pObjectInfo.InfoType)
            {
                case MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniHandleObjectInformationNone:
                    SetMiniHandleObjectInformationNone(handle, address); break;
                case MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniThreadInformation1:
                    SetMiniThreadInformation1(handle, address); break;
                case MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniMutantInformation1:
                    SetMiniMutantInformation1(handle, address); break;
                case MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniMutantInformation2:
                    SetMiniMutantInformation2(handle, address); break;
                case MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniProcessInformation1:
                    SetMiniProcessInformation1(handle, address); break;
                case MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniProcessInformation2:
                    SetMiniProcessInformation2(handle, address); break;
                case MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniEventInformation1:
                    SetMiniEventInformation1(handle, address); break;
                case MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniSectionInformation1:
                    SetMiniSectionInformation1(handle, address); break;
                case MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniHandleObjectInformationTypeMax: SetMiniHandleObjectInformationTypeMax(handle, address); break;
                default:
                    break;
            }

            if (pObjectInfo.NextInfoRva == 0)
            {
                return default(MINIDUMP_HANDLE_OBJECT_INFORMATION);
            }
            else
            {
                pObjectInfo = SafeMemoryMappedViewStreamHandler
                    .ReadStruct<MINIDUMP_HANDLE_OBJECT_INFORMATION>(IntPtr.Add(baseOfView, (int)pObjectInfo.NextInfoRva));
            }

            return pObjectInfo;
        }

        #region Actions

        private static void SetMiniHandleObjectInformationTypeMax(DumpHandle handle, IntPtr address)
        {
            handle.Type = DumpHandleType.TYPE_MAX;
        }

        private static void SetMiniSectionInformation1(DumpHandle handle, IntPtr address)
        {
            handle.Type = DumpHandleType.SECTION;
        }

        private static void SetMiniEventInformation1(DumpHandle handle, IntPtr address)
        {
            handle.Type = DumpHandleType.EVENT;
        }

        private static void SetMiniHandleObjectInformationNone(DumpHandle handle, IntPtr address)
        {
            handle.Type = DumpHandleType.NONE;
        }

        private static unsafe void SetMiniProcessInformation2(DumpHandle handle, IntPtr address)
        {
            handle.Type = DumpHandleType.PROCESS2;

            var additional_info = Marshal.PtrToStructure<PROCESS_ADDITIONAL_INFO_2>(address);
            handle.OwnerProcessId = additional_info.ProcessId;
            handle.OwnerThreadId = 0;
        }

        private static void SetMiniProcessInformation1(DumpHandle handle, IntPtr address)
        {
            handle.Type = DumpHandleType.PROCESS1;
        }

        private static void SetMiniMutantInformation2(DumpHandle handle, IntPtr address)
        {
            handle.Type = DumpHandleType.MUTEX2;

            var additional_info = Marshal.PtrToStructure<MUTEX_ADDITIONAL_INFO_2>(address);
            handle.OwnerProcessId = additional_info.OwnerProcessId;
            handle.OwnerThreadId = additional_info.OwnerThreadId;
        }

        private static void SetMiniMutantInformation1(DumpHandle handle, IntPtr address)
        {
            handle.Type = DumpHandleType.MUTEX1;

            var additional_info = Marshal.PtrToStructure<MUTEX_ADDITIONAL_INFO_1>(address);

            handle.MutexUnknown = new MutexUnknownFields()
            {
                Field1 = additional_info.Unknown1,
                Field2 = additional_info.Unknown2
            };
        }

        private static void SetMiniThreadInformation1(DumpHandle handle, IntPtr address)
        {
            handle.Type = DumpHandleType.THREAD;

            var additional_info = Marshal.PtrToStructure<THREAD_ADDITIONAL_INFO>(address);
            handle.OwnerProcessId = additional_info.ProcessId;
            handle.OwnerThreadId = additional_info.ThreadId;
        }

        #endregion

    }
}
