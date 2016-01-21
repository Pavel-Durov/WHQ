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

namespace Assignments.Core.Handlers
{
    public class MiniDumpHandler
    {
        const string DUMPS_DIR = "Dums";

        private static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        public void Handle(uint pid)
        {
            var handle = Kernel32.OpenProcess(Kernel32.ProcessAccessFlags.All, false, pid);

            RecCheckDirectory();

            string fullfileName = GetDumpFileName(pid);

            using (FileStream fs = new FileStream(fullfileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Write))
            {
                bool createdFile = DbgHelp.MiniDumpWriteDump(handle, pid, fs.SafeFileHandle, DbgHelp.MINIDUMP_STREAM_TYPE.ModuleListStream, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                if (createdFile)
                {
                    var safeMemoryMappedViewHandle = MapFile(fs, fullfileName);

                    ReadHandleData(safeMemoryMappedViewHandle);
                }
            }
        }

        private SafeMemoryMappedViewHandle MapFile(FileStream fs, string fullfileName)
        {
            MemoryMappedFile minidumpMappedFile = MemoryMappedFile.CreateFromFile(fs, Path.GetFileName(fullfileName), 0, MemoryMappedFileAccess.Read, null, HandleInheritability.None, false);

            SafeMemoryMappedViewHandle mappedFileView = Kernel32.MapViewOfFile(minidumpMappedFile.SafeMemoryMappedFileHandle, Kernel32.FileMapAccess.FileMapRead, 0, 0, IntPtr.Zero);

            Kernel32.MEMORY_BASIC_INFORMATION memoryInformation = default(Kernel32.MEMORY_BASIC_INFORMATION);

            if (Kernel32.VirtualQuery(mappedFileView, ref memoryInformation, (IntPtr)Marshal.SizeOf(memoryInformation)) == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            if (mappedFileView.IsInvalid)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            mappedFileView.Initialize((ulong)memoryInformation.RegionSize);
            return mappedFileView;
        }




        public void ReadHandleData(SafeMemoryMappedViewHandle safeMemoryMappedViewHandle)
        {
            DbgHelp.MINIDUMP_HANDLE_DATA_STREAM handleData;
            IntPtr streamPointer;
            uint streamSize;

            if (!this.ReadStream<DbgHelp.MINIDUMP_HANDLE_DATA_STREAM>(DbgHelp.MINIDUMP_STREAM_TYPE.HandleDataStream, out handleData, out streamPointer, out streamSize, safeMemoryMappedViewHandle))
            {
                //Handle it somehow
            }

            // Advance the stream pointer past the header
            streamPointer = streamPointer + (int)handleData.SizeOfHeader;


            // Now read the handles
            if (handleData.SizeOfDescriptor == Marshal.SizeOf(typeof(DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR)))
            {
                DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR[] handles = ReadArray<DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR>(streamPointer, (int)handleData.NumberOfDescriptors, safeMemoryMappedViewHandle);

                foreach (var h in handles) {
                    Console.WriteLine("Handle found");
                    Console.WriteLine($"handle : {h.Handle}");
                    Console.WriteLine($"pointerCount: {h.PointerCount}");
                    Console.WriteLine($"RVA: {h.TypeNameRva}");
                    Console.WriteLine();

                }
            }
            else if (handleData.SizeOfDescriptor == Marshal.SizeOf(typeof(DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR_2)))
            {
                DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR_2[] handles = ReadArray<DbgHelp.MINIDUMP_HANDLE_DESCRIPTOR_2>(streamPointer, (int)handleData.NumberOfDescriptors, safeMemoryMappedViewHandle);

                foreach (var h in handles)
                {
                    Console.WriteLine("Handle found");
                    Console.WriteLine($"handle : {h.Handle}");
                    Console.WriteLine($"pointerCount: {h.PointerCount}");
                    Console.WriteLine($"RVA: {h.TypeNameRva}");
                    Console.WriteLine();

                }

            }
            else
                throw new Exception("Unexpected 'SizeOfDescriptor' when reading HandleDataStream. The unexpected value was: '" + handleData.SizeOfDescriptor + "'");


        }


        /// <summary>
        /// Reads the specified number of value types from memory starting at the address, and writes them into an array.
        /// </summary>
        /// <typeparam name="T">The value type to read</typeparam>
        /// <param name="absoluteStreamReadAddress">The absolute (not offset) location in the stream from which to start reading.</param>
        /// <param name="count">The number of value types to read from the input array and to write to the output array.</param>
        /// <returns>A populated array of the value type.</returns>
        protected internal unsafe T[] ReadArray<T>(IntPtr absoluteStreamReadAddress, int count, SafeMemoryMappedViewHandle safeMemoryMappedViewHandle) where T : struct
        {
            T[] readItems = new T[count];

            try
            {
                byte* baseOfView = null;

                safeMemoryMappedViewHandle.AcquirePointer(ref baseOfView);

                ulong offset = (ulong)absoluteStreamReadAddress - (ulong)baseOfView;

                safeMemoryMappedViewHandle.ReadArray<T>(offset, readItems, 0, count);
            }
            finally
            {
                safeMemoryMappedViewHandle.ReleasePointer();
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


        private static string GetDumpFileName(uint pid)
        {
            var local = Path.Combine(DUMPS_DIR, $"dump-{pid}.mdmp");
            return Path.GetFullPath(local);
        }

        private static void RecCheckDirectory()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(DUMPS_DIR);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
        }


    }

}

