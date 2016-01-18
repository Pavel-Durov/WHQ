using Assignments.Core.WinApi;
using System;
using System.Collections.Generic;
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
        //MiniDump callback: http://www.debuginfo.com/articles/effminidumps.html
        //Minidump: http://www.debuginfo.com/examples/handledump.html
        //Example: http://blogs.msdn.com/b/dondu/archive/2010/10/24/writing-minidumps-in-c.aspx
        public static void Run(uint pid)
        {
            var handle = Kernel32.OpenProcess(Kernel32.ProcessAccessFlags.All, false, pid);

            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "myDump.mdmp");

            Console.WriteLine($"Writing dump to {fileName}");

            Debug.WriteLine(fileName);
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Write))
            {
                //Write(fs.SafeFileHandle, DbgHelp.Option.WithFullMemory, (uint) pid);
                Analyze(handle, pid, fs.SafeFileHandle, DbgHelp.MINIDUMP_TYPE.MiniDumpWithHandleData);
            }

        }

        public static void Analyze(IntPtr handle, uint processId, SafeHandle fileHandle, DbgHelp.MINIDUMP_TYPE dumpType)
        {
            //Craeting mini dump file...
            DbgHelp.MINIDUMP_EXCEPTION_INFORMATION info = new DbgHelp.MINIDUMP_EXCEPTION_INFORMATION();

            DbgHelp.MINIDUMP_MODULE_CALLBACK callback = new DbgHelp.MINIDUMP_MODULE_CALLBACK();

            DbgHelp.MiniDumpWriteDump(handle, processId, fileHandle, dumpType, ref info, IntPtr.Zero, ref callback);


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
