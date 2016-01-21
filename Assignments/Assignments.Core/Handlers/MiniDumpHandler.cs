using Assignments.Core.WinApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

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
                Analyze(handle, pid, fs.SafeFileHandle, DbgHelp.MINIDUMP_TYPE.MiniDumpWithHandleData, fullfileName);
            }
        }

        private void Analyze(IntPtr handle, uint processId, SafeHandle fileHandle, DbgHelp.MINIDUMP_TYPE dumpType, string fullfileName)
        {


            DbgHelp.MINIDUMP_EXCEPTION_INFORMATION info = new DbgHelp.MINIDUMP_EXCEPTION_INFORMATION();
            DbgHelp.MINIDUMP_MODULE_CALLBACK callback = new DbgHelp.MINIDUMP_MODULE_CALLBACK();
            
            //Craeting mini dump file...
            bool result = DbgHelp.MiniDumpWriteDump(handle, processId, fileHandle, dumpType, ref info, IntPtr.Zero, ref callback);

            if (result)
            {
                Kernel32.SECURITY_ATTRIBUTES securityAttributes = new Kernel32.SECURITY_ATTRIBUTES();
                //20971520
                IntPtr fileMaphandle = Kernel32.CreateFileMapping(INVALID_HANDLE_VALUE, ref securityAttributes, Kernel32.FileMapProtection.PageReadWrite, 0, 1024, "test");

                if (fileMaphandle != IntPtr.Zero)
                {
                    IntPtr openFileMapping = Kernel32.OpenFileMapping(Kernel32.FILE_MAP_ALL_ACCESS, false, "test");

                    if (openFileMapping != IntPtr.Zero)
                    {


                        DbgHelp.MINIDUMP_DIRECTORY directory = new DbgHelp.MINIDUMP_DIRECTORY();

                        var streamPointer = IntPtr.Zero;
                        uint streamSize = 0;

                        var error = Marshal.GetLastWin32Error();
                        uint size = 1024;
                        var viewHandle = Kernel32.MapViewOfFile(fileMaphandle, DbgHelp.FileMapAccess.FileMapAllAccess, 0, 0, size);
                        //MapViewOfFile

                        // baseOfView is the IntPtr we got when we created the memory mapped file
                        var readDumpResult = DbgHelp.MiniDumpReadDumpStream(
                                        viewHandle,
                                        (uint)DbgHelp.MINIDUMP_STREAM_TYPE.ModuleListStream,
                                        ref directory, ref streamPointer, ref streamSize);

                        if (readDumpResult)
                        {

                        }
                        else {
                            var errorCode = Marshal.GetLastWin32Error();
                        }

                    }
                    else
                    {
                        //TODO: Handle...
                    }

                }
            }
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
