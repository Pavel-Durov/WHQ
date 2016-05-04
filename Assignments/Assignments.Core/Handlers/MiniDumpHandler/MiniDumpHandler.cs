using Assignments.Core.WinApi;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Assignments.Core.Model.MiniDump;

namespace Assignments.Core.Handlers.MiniDump
{
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

        public void Init(string dumpFileName)
        {
            using (FileStream fileStream = File.Open(dumpFileName, System.IO.FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                _safeMemoryMappedViewHandle = MemoryMapFileHandler.MapFile(fileStream, dumpFileName);
                GetSystemInfo();
            }
        }

        public void Init(uint pid)
        {
            var handle = Kernel32.OpenProcess(Kernel32.ProcessAccessFlags.All, false, pid);

            RecheckDirectory();

            string fileName = null;
            string fullfileName = GetDumpFileName(pid, ref fileName);

            using (FileStream fs = new FileStream(fullfileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Write))
            {
                bool miniDumpCreated = DbgHelp.MiniDumpWriteDump(handle, pid, fs.SafeFileHandle, DbgHelp.MINIDUMP_TYPE.MiniDumpWithHandleData, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

                if (miniDumpCreated)
                {
                    _safeMemoryMappedViewHandle = MemoryMapFileHandler.MapFile(fs, fileName);
                }
            }
        }

        #region MiniDump Handles

        public unsafe List<MiniDumpHandle> GetHandles()
        {
            List<MiniDumpHandle> result = new List<MiniDumpHandle>();

            DbgHelp.MINIDUMP_HANDLE_DATA_STREAM handleData;
            IntPtr streamPointer;
            uint streamSize;

            var readStrem = StreamHandler.ReadStream(DbgHelp.MINIDUMP_STREAM_TYPE.HandleDataStream, out handleData, out streamPointer, out streamSize, _safeMemoryMappedViewHandle, out _baseOfView);

            if (!readStrem)
            {
                Debug.WriteLine($"Can't read stream ! ");
            }

            //Advancing the pointer
            streamPointer = streamPointer + (int)handleData.SizeOfHeader;

            if (handleData.SizeOfDescriptor == Marshal.SizeOf(typeof(DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR)))
            {
                DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR[] handles = StreamHandler.ReadArray<DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR>(streamPointer, (int)handleData.NumberOfDescriptors, _safeMemoryMappedViewHandle);

                foreach (var handle in handles)
                {
                    result.Add(new MiniDumpHandle(handle));
                }
            }
            else if (handleData.SizeOfDescriptor == Marshal.SizeOf(typeof(DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR_2)))
            {
                DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR_2[] handles = StreamHandler.ReadArray<DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR_2>(streamPointer, (int)handleData.NumberOfDescriptors, _safeMemoryMappedViewHandle);

                foreach (var handle in handles)
                {
                    MiniDumpHandle temp = GetHandleData(handle, streamPointer);

                    result.Add(temp);
                }
            }


            return result;
        }

        private MiniDumpHandle GetHandleData(DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR_2 handle, IntPtr streamPointer)
        {
            string objectName = GetMiniDumpString(handle.ObjectNameRva, streamPointer);
            string typeName = GetMiniDumpString(handle.TypeNameRva, streamPointer);

            var result = new MiniDumpHandle(handle, objectName, typeName);

            if (handle.HandleCount > 0)
            {
                if (handle.ObjectInfoRva > 0)
                {
                    var info = StreamHandler.ReadStruct<DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION>(handle.ObjectInfoRva, streamPointer, _safeMemoryMappedViewHandle);
                    if (info.NextInfoRva != 0)
                    {
                        uint address = (uint)_baseOfView + handle.ObjectInfoRva;
                        DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION pObjectInfo = StreamHandler.ReadStruct<DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION>(address);

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


        public MiniDumpSystemInfo GetSystemInfo()
        {
            MiniDumpSystemInfo result = null;
            DbgHelp.MINIDUMP_SYSTEM_INFO systemInfo;
            IntPtr streamPointer;
            uint streamSize;

            bool readResult = StreamHandler.ReadStream<DbgHelp.MINIDUMP_SYSTEM_INFO>(DbgHelp.MINIDUMP_STREAM_TYPE.SystemInfoStream, out systemInfo, out streamPointer, out streamSize, _safeMemoryMappedViewHandle);

            if (readResult)
            {
                result = new MiniDumpSystemInfo(systemInfo);
            }

            return result;
        }


        public List<MiniDumpModule> GetModuleList()
        {
            DbgHelp.MINIDUMP_MODULE_LIST moduleList;
            IntPtr streamPointer;
            uint streamSize;
            List<MiniDumpModule> result = null;

            if (StreamHandler.ReadStream<DbgHelp.MINIDUMP_MODULE_LIST>(DbgHelp.MINIDUMP_STREAM_TYPE.ModuleListStream, out moduleList, out streamPointer, out streamSize, _safeMemoryMappedViewHandle))
            {
                //skiping the NumberOfModules field (which is 4 bytes)
                var offset = streamPointer + 4;
                var modules = StreamHandler.ReadArray<DbgHelp.MINIDUMP_MODULE>(offset, (int)moduleList.NumberOfModules, _safeMemoryMappedViewHandle);

                result = new List<MiniDumpModule>();

                foreach (var module in modules)
                {
                    var name = StreamHandler.ReadString(module.ModuleNameRva, _safeMemoryMappedViewHandle);
                    result.Add(new MiniDumpModule(module, name));
                }
            }
            else
            {
                //TODO: throw somthing here
            }

            return result;
        }


        private string GetMiniDumpString(uint rva, IntPtr streamPointer)
        {
            string result = String.Empty;
            try
            {
                var typeNameMinidumpString = StreamHandler.ReadStruct<DbgHelp.MINIDUMP_STRING>(rva, streamPointer, _safeMemoryMappedViewHandle);

                result = StreamHandler.ReadString(rva, typeNameMinidumpString.Length, _safeMemoryMappedViewHandle);
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

