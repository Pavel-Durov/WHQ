using Assignments.Core.WinApi;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.IO.MemoryMappedFiles;
using System.Diagnostics;
using Assignments.Core.Model.MiniDump;
using System.Threading.Tasks;

namespace Assignments.Core.Handlers
{
    public class MiniDumpHandler
    {
        const string DUMPS_DIR = "Dums";

        private static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        public MiniDumpHandler()
        {

        }

        public void Init(uint pid)
        {
            var handle = Kernel32.OpenProcess(Kernel32.ProcessAccessFlags.All, false, pid);

            RecheckDirectory();

            string fileName = null;
            string fullfileName = GetDumpFileName(pid, ref fileName);

            using (FileStream fs = new FileStream(fullfileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Write))
            {
                bool miniDumpCreated = DbgHelp.MiniDumpWriteDump(handle, pid, fs.SafeFileHandle, DbgHelp.MINIDUMP_STREAM_TYPE.ModuleListStream, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

                if (miniDumpCreated)
                {
                    _safeMemoryMappedViewHandle = MapFile(fs, fileName);
                    GetHandleData();
                }
            }
        }

        SafeMemoryMappedViewHandle _safeMemoryMappedViewHandle;

        private SafeMemoryMappedViewHandle MapFile(FileStream fs, string fileName)
        {
            MemoryMappedFile mappedFile = MemoryMappedFile.CreateFromFile(fs, fileName, 0, MemoryMappedFileAccess.Read, null, HandleInheritability.None, false);

            SafeMemoryMappedViewHandle mappedFileView = Kernel32.MapViewOfFile(mappedFile.SafeMemoryMappedFileHandle, Kernel32.FileMapAccess.FileMapRead, 0, 0, IntPtr.Zero);

            Kernel32.MEMORY_BASIC_INFORMATION memoryInfo = default(Kernel32.MEMORY_BASIC_INFORMATION);

            if (Kernel32.VirtualQuery(mappedFileView, ref memoryInfo, (IntPtr)Marshal.SizeOf(memoryInfo)) == IntPtr.Zero)
            {
                Debug.WriteLine($"error:  {Marshal.GetLastWin32Error()}");
            }

            if (mappedFileView.IsInvalid)
            {
                Debug.WriteLine($"MapViewOfFile IsInvalid, error:  {Marshal.GetLastWin32Error()}");
            }

            mappedFileView.Initialize((ulong)memoryInfo.RegionSize);

            return mappedFileView;
        }




        public List<MiniDumpHandle> GetHandleData()
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
                DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR[] handles = ReadArray<DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR>(streamPointer, (int)handleData.NumberOfDescriptors, _safeMemoryMappedViewHandle);

                foreach (var handle in handles)
                {
                    result.Add(new MiniDumpHandle(handle));
                }
            }
            else if (handleData.SizeOfDescriptor == Marshal.SizeOf(typeof(DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR_2)))
            {
                DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR_2[] handles = ReadArray<DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR_2>(streamPointer, (int)handleData.NumberOfDescriptors, _safeMemoryMappedViewHandle);

                foreach (var handle in handles)
                {
                    var temp = new MiniDumpHandle(handle);

                    var info = ReadInfo(handle.ObjectInfoRva, streamPointer);
                    temp.AddInfo(info);

                    result.Add(temp);
                }
            }

            return result;
        }

        public unsafe DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION ReadInfo(uint rva, IntPtr streamPtr)
        {
            DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION result = new DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION();

            try
            {
                byte* baseOfView = null;
                _safeMemoryMappedViewHandle.AcquirePointer(ref baseOfView);
                ulong offset = (ulong)streamPtr - (ulong)baseOfView;
                result = _safeMemoryMappedViewHandle.Read<DbgHelp.MINIDUMP_HANDLE_OBJECT_INFORMATION>(offset);
            }
            finally
            {
                _safeMemoryMappedViewHandle.ReleasePointer();
            }

            return result;
        }



        protected internal unsafe T[] ReadArray<T>(IntPtr absoluteAddress, int count, SafeMemoryMappedViewHandle safeHandle) where T : struct
        {
            T[] readItems = new T[count];

            try
            {
                byte* baseOfView = null;
                safeHandle.AcquirePointer(ref baseOfView);
                ulong offset = (ulong)absoluteAddress - (ulong)baseOfView;
                safeHandle.ReadArray<T>(offset, readItems, 0, count);
            }
            finally
            {
                safeHandle.ReleasePointer();
            }

            return readItems;
        }


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

