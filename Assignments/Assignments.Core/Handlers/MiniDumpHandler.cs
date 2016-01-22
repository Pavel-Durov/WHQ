using Assignments.Core.WinApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO.MemoryMappedFiles;
using System.Diagnostics;
using Assignments.Core.Model.MiniDump;

namespace Assignments.Core.Handlers
{
    public class MiniDumpHandler
    {
        const string DUMPS_DIR = "Dums";

        private static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        public void Handle(uint pid)
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
                    var safeMemoryMappedViewHandle = MapFile(fs, fileName);
                    var result = ReadHandleData(safeMemoryMappedViewHandle);
                }
            }
        }

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




        public List<MiniDumpHandle> ReadHandleData(SafeMemoryMappedViewHandle safeMemoryMappedViewHandle)
        {
            List<MiniDumpHandle> result = new List<MiniDumpHandle>();

            DbgHelp.MINIDUMP_HANDLE_DATA_STREAM handleData;
            IntPtr streamPointer;
            uint streamSize;

            var readStrem = ReadStream<DbgHelp.MINIDUMP_HANDLE_DATA_STREAM>(DbgHelp.MINIDUMP_STREAM_TYPE.HandleDataStream, out handleData, out streamPointer, out streamSize, safeMemoryMappedViewHandle);

            if (!readStrem)
            {
                Debug.WriteLine($"Can't read stream ! ");
            }

            //Advancing the pointer
            streamPointer = streamPointer + (int)handleData.SizeOfHeader;

            if (handleData.SizeOfDescriptor == Marshal.SizeOf(typeof(DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR)))
            {
                DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR[] handles = ReadArray<DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR>(streamPointer, (int)handleData.NumberOfDescriptors, safeMemoryMappedViewHandle);

                foreach (var handle in handles)
                {
                    result.Add(new MiniDumpHandle(handle));
                }
            }
            else if (handleData.SizeOfDescriptor == Marshal.SizeOf(typeof(DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR_2)))
            {
                DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR_2[] handles = ReadArray<DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR_2>(streamPointer, (int)handleData.NumberOfDescriptors, safeMemoryMappedViewHandle);

                foreach (var handle in handles)
                {
                    result.Add(new MiniDumpHandle(handle));
                }

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


        protected unsafe bool ReadStream<T>(DbgHelp.MINIDUMP_STREAM_TYPE streamToRead, out T streamData, out IntPtr streamPointer, out uint streamSize, SafeMemoryMappedViewHandle safeMemoryMappedViewHandle)
        {
            DbgHelp.MINIDUMP_DIRECTORY directory = new DbgHelp.MINIDUMP_DIRECTORY();
            streamData = default(T);
            streamPointer = IntPtr.Zero;
            streamSize = 0;

            try
            {
                byte* baseOfView = null;
                safeMemoryMappedViewHandle.AcquirePointer(ref baseOfView);

                if (baseOfView == null)
                    throw new Exception("Unable to aquire pointer to memory mapped view");

                if (!DbgHelp.MiniDumpReadDumpStream((IntPtr)baseOfView, streamToRead, ref directory, ref streamPointer, ref streamSize))
                {
                    int lastError = Marshal.GetLastWin32Error();

                    if (lastError == DbgHelp.ERR_ELEMENT_NOT_FOUND)
                    {
                        return false;
                    }
                    else
                        throw new Win32Exception(lastError);
                }

                streamData = (T)Marshal.PtrToStructure(streamPointer, typeof(T));

            }
            finally
            {
                safeMemoryMappedViewHandle.ReleasePointer();
            }

            return true;
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

