using Assignments.Core.WinApi;
using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace Assignments.Core.Handlers
{
    public class MemoryMapFileHandler
    {
        public static SafeMemoryMappedViewHandle MapFile(FileStream fileStream, string fileName)
        {
            MemoryMappedFile mappedFile = MemoryMappedFile.CreateFromFile(fileStream, Path.GetFileName(fileName), 0, MemoryMappedFileAccess.Read, null, HandleInheritability.None, false);


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

    }
}
