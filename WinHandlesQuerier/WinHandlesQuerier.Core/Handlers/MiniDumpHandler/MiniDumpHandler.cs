
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Diagnostics;
using WinHandlesQuerier.Core.Model.MiniDump;
using DbgHelp;
using System.Threading.Tasks;

namespace WinHandlesQuerier.Core.Handlers.MiniDump
{
    /// <summary>
    /// This class extracts data from dump file uisng native windows api DbgHelp.h function. 
    /// </summary>
    public class MiniDumpHandler
    {
        const string DUMPS_DIR = "Dums";

        #region Members

        private static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        SafeMemoryMappedViewHandle _safeMemoryMappedViewHandle;
        private IntPtr _baseOfView;

        #endregion

        public MiniDumpHandler(uint pid) { Init(pid); }

        public MiniDumpHandler(string dumpFileName) { Init(dumpFileName); }

        /// <summary>
        /// Initialization of SafeMemoryMappedViewHandle by the filePath 
        /// </summary>
        /// <param name="dumpFileName">full path to dump file</param>
        public void Init(string dumpFileName)
        {
            using (FileStream fileStream = File.Open(dumpFileName, System.IO.FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                _safeMemoryMappedViewHandle = MemoryMapFileHandler.MapFile(fileStream, dumpFileName);
                GetSystemInfo();
            }
        }

        /// <summary>
        /// Initialization of SafeMemoryMappedViewHandle by PID of live process
        /// </summary>
        /// <param name="pid">PID of life process</param>
        public void Init(uint pid)
        {
            var handle = Kernel32.Functions.OpenProcess(Kernel32.ProcessAccessFlags.All, false, pid);

            RecheckDirectory();

            string fileName = null;
            string fullfileName = GetDumpFileName(pid, ref fileName);

            using (FileStream fs = new FileStream(fullfileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Write))
            {
                bool miniDumpCreated = Functions.MiniDumpWriteDump(handle, pid, fs.SafeFileHandle, MINIDUMP_TYPE.MiniDumpWithHandleData, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

                if (miniDumpCreated)
                {
                    _safeMemoryMappedViewHandle = MemoryMapFileHandler.MapFile(fs, fileName);
                }
            }
        }

        #region MiniDump Handles

        /// <summary>
        /// Reads handles informations from previously inited SafeMemoryMappedViewHandle
        /// </summary>
        /// <returns>List of handles</returns>
        public async Task<List<MiniDumpHandle>> GetHandles()
        {
            List<MiniDumpHandle> result = new List<MiniDumpHandle>();

            MINIDUMP_HANDLE_DATA_STREAM handleData;
            IntPtr streamPointer;
            uint streamSize;

            var readStrem = SafeMemoryMappedViewStreamHandler.ReadStream(MINIDUMP_STREAM_TYPE.HandleDataStream, out handleData, out streamPointer, out streamSize, _safeMemoryMappedViewHandle, out _baseOfView);

            if (!readStrem)
            {
                Debug.WriteLine($"Can't read stream ! ");
            }

            //Advancing the pointer
            streamPointer = streamPointer + (int)handleData.SizeOfHeader;

            if (handleData.SizeOfDescriptor == Marshal.SizeOf(typeof(MINIDUMP_HANDLE_DESCRIPTOR)))
            {
                MINIDUMP_HANDLE_DESCRIPTOR[] handles = await SafeMemoryMappedViewStreamHandler.ReadArray<MINIDUMP_HANDLE_DESCRIPTOR>(streamPointer, (int)handleData.NumberOfDescriptors, _safeMemoryMappedViewHandle);

                foreach (var handle in handles)
                {
                    result.Add(new MiniDumpHandle(handle));
                }
            }
            else if (handleData.SizeOfDescriptor == Marshal.SizeOf(typeof(MINIDUMP_HANDLE_DESCRIPTOR_2)))
            {
                MINIDUMP_HANDLE_DESCRIPTOR_2[] handles = await SafeMemoryMappedViewStreamHandler.ReadArray<MINIDUMP_HANDLE_DESCRIPTOR_2>(streamPointer, (int)handleData.NumberOfDescriptors, _safeMemoryMappedViewHandle);

                foreach (var handle in handles)
                {
                    MiniDumpHandle temp = await GetHandleData(handle, streamPointer);

                    result.Add(temp);
                }
            }


            return result;
        }

        /// <summary>
        /// Constructs handles wrapped class with MINIDUMP_HANDLE_DESCRIPTOR_2 struct data
        /// </summary>
        /// <param name="handle">minidump struct descriptor</param>
        /// <param name="streamPointer">stream pointer</param>
        /// <returns></returns>
        private async Task<MiniDumpHandle> GetHandleData(MINIDUMP_HANDLE_DESCRIPTOR_2 handle, IntPtr streamPointer)
        {
            string objectName = await GetMiniDumpString(handle.ObjectNameRva, streamPointer);
            string typeName = await GetMiniDumpString(handle.TypeNameRva, streamPointer);

            var result = new MiniDumpHandle(handle, objectName, typeName);

            if (handle.HandleCount > 0)
            {
                if (handle.ObjectInfoRva > 0)
                {
                    var info = await SafeMemoryMappedViewStreamHandler.ReadStruct<MINIDUMP_HANDLE_OBJECT_INFORMATION>(handle.ObjectInfoRva, streamPointer, _safeMemoryMappedViewHandle);
                    if (info.NextInfoRva != 0)
                    {
                        IntPtr address = IntPtr.Add(_baseOfView, handle.ObjectInfoRva);
                        MINIDUMP_HANDLE_OBJECT_INFORMATION pObjectInfo = SafeMemoryMappedViewStreamHandler.ReadStruct<MINIDUMP_HANDLE_OBJECT_INFORMATION>(address);

                        do
                        {
                            pObjectInfo = ObjectInformationHandler.DealWithHandleInfo(pObjectInfo, result, address, _baseOfView);
                            if (pObjectInfo.NextInfoRva == 0) break;
                        }
                        while (pObjectInfo.NextInfoRva != 0 && pObjectInfo.SizeOfInfo != 0);
                    }

                }

            }

            if (handle.PointerCount > 0)
            {
                //TODO: The meaning of this member depends on the handle type and the operating system.
                //This is the number kernel references to the object that this handle refers to. 
            }

            if (handle.GrantedAccess > 0)
            {
                //TODO: The meaning of this member depends on the handle type and the operating system.
            }

            if (handle.Attributes > 0)
            {
                //TODO: 
                //The attributes for the handle, this corresponds to OBJ_INHERIT, OBJ_CASE_INSENSITIVE, etc. 
            }

            return result;
        }

        #endregion

        /// <summary>
        /// Fetches System Info from the mapped dump file (using MINIDUMP_SYSTEM_INFO struct)
        /// </summary>
        /// <returns>System Info</returns>
        public async Task<MiniDumpSystemInfo> GetSystemInfo()
        {
            return await Task<MiniDumpSystemInfo>.Run(() =>
            {
                MiniDumpSystemInfo result = null;
                MINIDUMP_SYSTEM_INFO systemInfo;
                IntPtr streamPointer;
                uint streamSize;

                bool readResult = SafeMemoryMappedViewStreamHandler.ReadStream<MINIDUMP_SYSTEM_INFO>(MINIDUMP_STREAM_TYPE.SystemInfoStream, out systemInfo, out streamPointer, out streamSize, _safeMemoryMappedViewHandle);

                if (readResult)
                {
                    result = new MiniDumpSystemInfo(systemInfo);
                }

                return result;
            });
        }

        /// <summary>
        /// fetches module list from the mapped dump file
        /// </summary>
        /// <returns></returns>
        public async Task<List<MiniDumpModule>> GetModuleList()
        {
            MINIDUMP_MODULE_LIST moduleList;
            IntPtr streamPointer;
            uint streamSize;
            List<MiniDumpModule> result = null;

            if (SafeMemoryMappedViewStreamHandler.ReadStream<MINIDUMP_MODULE_LIST>(MINIDUMP_STREAM_TYPE.ModuleListStream, out moduleList, out streamPointer, out streamSize, _safeMemoryMappedViewHandle))
            {
                //skiping the NumberOfModules field (which is 4 bytes)
                var offset = streamPointer + 4;
                var modules = await SafeMemoryMappedViewStreamHandler.ReadArray<MINIDUMP_MODULE>(offset, (int)moduleList.NumberOfModules, _safeMemoryMappedViewHandle);

                result = new List<MiniDumpModule>();

                foreach (var module in modules)
                {
                    var name = await SafeMemoryMappedViewStreamHandler.ReadString(module.ModuleNameRva, _safeMemoryMappedViewHandle);
                    result.Add(new MiniDumpModule(module, name));
                }
            }
            else
            {
                //TODO: throw somthing here
            }

            return result;
        }


        private async Task<string> GetMiniDumpString(Int32 rva, IntPtr streamPointer)
        {
            string result = String.Empty;
            try
            {
                var typeNameMinidumpString = await SafeMemoryMappedViewStreamHandler.ReadStruct<MINIDUMP_STRING>(rva, streamPointer, _safeMemoryMappedViewHandle);
                result = await SafeMemoryMappedViewStreamHandler.ReadString(rva, typeNameMinidumpString.Length, _safeMemoryMappedViewHandle);
            }
            catch (Exception ex) { }

            return result;
        }

        private static string GetDumpFileName(uint pid, ref string fileName)
        {
            fileName = $"dump-{pid}.mdmp";
            var local = Path.Combine(DUMPS_DIR, fileName);
            return Path.GetFullPath(local);
        }

        private static void RecheckDirectory()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(DUMPS_DIR);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
        }
    }
}

