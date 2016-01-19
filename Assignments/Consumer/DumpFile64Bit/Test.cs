using Assignments.Core.WinApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Consumer.DumpFile64Bit
{
    class Test
    {
        const string DUMPS_DIR = "Dums";

        //MiniDump callback: http://www.debuginfo.com/articles/effminidumps.html
        //Minidump: http://www.debuginfo.com/examples/handledump.html
        //Example: http://blogs.msdn.com/b/dondu/archive/2010/10/24/writing-minidumps-in-c.aspx
        private static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        public static void Run(uint pid)
        {
            var handle = Kernel32.OpenProcess(Kernel32.ProcessAccessFlags.All, false, pid);

            RecCheckDirectory();

            string fullfileName = GetDumpFileName(pid);

            using (FileStream fs = new FileStream(fullfileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Write))
            {
                //Write(fs.SafeFileHandle, DbgHelp.Option.WithFullMemory, (uint) pid);
                Analyze(handle, pid, fs.SafeFileHandle, DbgHelp.MINIDUMP_TYPE.MiniDumpWithHandleData, fullfileName);
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

        //https://gregsplaceontheweb.wordpress.com/2014/04/08/reading-minidump-files-part-3-of-4-reading-stream-data-returned-from-minidumpreaddumpstream/
        public static void Analyze(IntPtr handle, uint processId, SafeHandle fileHandle, DbgHelp.MINIDUMP_TYPE dumpType, string fullfileName)
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
                        uint size =  1024;
                        var viewHandle = DbgHelp.MapViewOfFile(fileMaphandle, DbgHelp.FileMapAccess.FileMapAllAccess, 0, 0, size);
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
                else
                {
                    throw new Exception("CreateFileMapping", new Win32Exception(Marshal.GetLastWin32Error()));
                }
            }
            else
            {
                //TODO: Handle....
            }
        }






        public static bool Write(SafeHandle fileHandle, DbgHelp.Option options, DbgHelp.ExceptionInfo exceptionInfo, uint currentProcessId)
        {
            //Process currentProcess = Process.GetCurrentProcess();
            //IntPtr currentProcessHandle = currentProcess.Handle;

            //DbgHelp.MiniDumpExceptionInformation exp;
            //exp.ThreadId = DbgHelp.GetCurrentThreadId();

            //exp.ClientPointers = false;
            //exp.ExceptionPointers = IntPtr.Zero;
            //if (exceptionInfo == DbgHelp.ExceptionInfo.Present)
            //{
            //    exp.ExceptionPointers = System.Runtime.InteropServices.Marshal.GetExceptionPointers();
            //}

            //bool bRet = false;

            //if (exp.ExceptionPointers == IntPtr.Zero)
            //{
            //    bRet = DbgHelp.MiniDumpWriteDump(currentProcessHandle, currentProcessId, fileHandle, (uint)options, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            //}

            //else
            //{
            //    bRet = DbgHelp.MiniDumpWriteDump(currentProcessHandle, currentProcessId, fileHandle, (uint)options, ref exp, IntPtr.Zero, IntPtr.Zero);
            //}

            //return bRet;
            return false;
        }



        public static bool Write(SafeHandle fileHandle, DbgHelp.Option dumpType, uint pid)
        {
            return Write(fileHandle, dumpType, DbgHelp.ExceptionInfo.None, pid);
        }

    }
}
