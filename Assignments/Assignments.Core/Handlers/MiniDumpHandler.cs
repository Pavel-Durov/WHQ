using Assignments.Core.WinApi;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Assignments.Core.Model.MiniDump;

namespace Assignments.Core.Handlers
{
    public class MiniDumpHandler
    {
        const string DUMPS_DIR = "Dums";

        private static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        public void Init(string dumpFileName)
        {
            using (FileStream fileStream = File.Open(dumpFileName, System.IO.FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                _safeMemoryMappedViewHandle = MemoryMapFileHandler.MapFile(fileStream, dumpFileName);
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

        SafeMemoryMappedViewHandle _safeMemoryMappedViewHandle;


        public unsafe List<MiniDumpHandle> GetHandleData()
        {
            List<MiniDumpHandle> result = new List<MiniDumpHandle>();

            DbgHelp.MINIDUMP_HANDLE_DATA_STREAM handleData;
            IntPtr streamPointer;
            uint streamSize;

            var readStrem = ReadStream(DbgHelp.MINIDUMP_STREAM_TYPE.HandleDataStream, out handleData, out streamPointer, out streamSize, _safeMemoryMappedViewHandle);

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

       
        private unsafe MiniDumpHandle GetHandleData(DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR_2 handle, IntPtr streamPointer)
        {

            string objectName = GetString(handle.ObjectNameRva, streamPointer);
            string typeName = GetString(handle.TypeNameRva, streamPointer);

            var result = new MiniDumpHandle(handle, objectName, typeName);

            if (handle.HandleCount > 0)
            {
                //TODO: The meaning of this member depends on the handle type and the operating system.
                //This is the number of open handles to the object that this handle refers to. 
                if (handle.ObjectInfoRva > 0)
                {
                    var info = StreamHandler.ReadStruct<DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION>(handle.ObjectInfoRva, streamPointer, _safeMemoryMappedViewHandle);
                    if (info.NextInfoRva != 0)
                    {
                        var str = StreamHandler.ReadString(info.NextInfoRva, info.SizeOfInfo, _safeMemoryMappedViewHandle);
                        result.AddInfo(info, str);


                        DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION pObjectInfo = StreamHandler.ReadStruct<DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION>((uint)_baseOfView + handle.ObjectInfoRva);

                        do
                        {
                            pObjectInfo = DealWithHandleInfo(pObjectInfo, result);

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

        private DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION DealWithHandleInfo(DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION pObjectInfo, MiniDumpHandle handle)
        {
            //TODO: update handle according to InfoType value

            Debug.WriteLine(pObjectInfo.InfoType);
            switch (pObjectInfo.InfoType)
            {
                case DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniHandleObjectInformationNone:
                    break;
                case DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniThreadInformation1:
                    break;
                case DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniMutantInformation1:
                    break;
                case DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniMutantInformation2:
                    break;
                case DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniProcessInformation1:
                    break;
                case DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniProcessInformation2:
                    break;
                case DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniEventInformation1:
                    break;
                case DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniSectionInformation1:
                    break;
                case DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE.MiniHandleObjectInformationTypeMax:
                    break;
                default:
                    break;
            }
        
            if (pObjectInfo.NextInfoRva == 0)
            {
                return default(DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION);
            }
            else
            {
                pObjectInfo = StreamHandler.ReadStruct<DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION>((uint)_baseOfView + pObjectInfo.NextInfoRva);
            }
            return pObjectInfo;
        }

        private string GetString(uint rva, IntPtr streamPointer)
        {
            var typeNameMinidumpString = StreamHandler.ReadStruct<DbgHelp.MINIDUMP_STRING>(rva, streamPointer, _safeMemoryMappedViewHandle);

            return StreamHandler.ReadString(rva, typeNameMinidumpString.Length, _safeMemoryMappedViewHandle);
        }
        private IntPtr _baseOfView;
        protected unsafe bool ReadStream(DbgHelp.MINIDUMP_STREAM_TYPE streamToRead, out DbgHelp.MINIDUMP_HANDLE_DATA_STREAM streamData, out IntPtr streamPointer, out uint streamSize, SafeMemoryMappedViewHandle safeMemoryMappedViewHandle)
        {
            bool result = false;
            DbgHelp.MINIDUMP_DIRECTORY directory = new DbgHelp.MINIDUMP_DIRECTORY();
            streamData = default(DbgHelp.MINIDUMP_HANDLE_DATA_STREAM);
            streamPointer = IntPtr.Zero;
            streamSize = 0;

            try
            {
                byte* baseOfView = null;
                safeMemoryMappedViewHandle.AcquirePointer(ref baseOfView);

                result = DbgHelp.MiniDumpReadDumpStream((IntPtr)baseOfView, streamToRead, ref directory, ref streamPointer, ref streamSize);
                _baseOfView = (IntPtr)baseOfView;
                if (result)
                {
                    streamData = (DbgHelp.MINIDUMP_HANDLE_DATA_STREAM)Marshal.PtrToStructure(streamPointer, typeof(DbgHelp.MINIDUMP_HANDLE_DATA_STREAM));
                }
            }
            finally
            {
                safeMemoryMappedViewHandle.ReleasePointer();
            }

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

