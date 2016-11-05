using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using Kernel32;

namespace WHQ.Core.Handlers
{
    public class MemoryMapFileHandler
    {
        public static SafeMemoryMappedViewHandle MapFile(FileStream fileStream, string fileName)
        {
            MemoryMappedFile mappedFile = MemoryMappedFile.CreateFromFile(fileStream, Path.GetFileName(fileName), 0, MemoryMappedFileAccess.Read, null, HandleInheritability.None, false);


            SafeMemoryMappedViewHandle mappedFileView = Functions.MapViewOfFile(mappedFile.SafeMemoryMappedFileHandle, FileMapAccess.FileMapRead, 0, 0, IntPtr.Zero);

            MEMORY_BASIC_INFORMATION memoryInfo = default(MEMORY_BASIC_INFORMATION);
            
            if (Functions.VirtualQuery(mappedFileView, ref memoryInfo, (IntPtr)Marshal.SizeOf(memoryInfo)) == IntPtr.Zero)
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
