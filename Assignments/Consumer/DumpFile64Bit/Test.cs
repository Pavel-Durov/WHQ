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
        public static void Run(int pid)
        {
            var handle = Kernel32.OpenProcess((long)Kernel32.PROCESS_PRIVILEGE.WRITE_DAC, false , pid);

            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MiniDumpDemo_Mainline.mdmp");

            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Write))

            {

                Write(fs.SafeFileHandle, DbgHelp.Option.WithFullMemory);

            }

        }

        public static bool Write(SafeHandle fileHandle, DbgHelp.Option options, DbgHelp.ExceptionInfo exceptionInfo)
        {
            Process currentProcess = Process.GetCurrentProcess();
            IntPtr currentProcessHandle = currentProcess.Handle;
            uint currentProcessId = (uint)currentProcess.Id;

            DbgHelp.MiniDumpExceptionInformation exp;
            exp.ThreadId = DbgHelp.GetCurrentThreadId();

            exp.ClientPointers = false;
            exp.ExceptionPointers = IntPtr.Zero;
            if (exceptionInfo == DbgHelp.ExceptionInfo.Present)
            {
                exp.ExceptionPointers = System.Runtime.InteropServices.Marshal.GetExceptionPointers();
            }

            bool bRet = false;

            if (exp.ExceptionPointers == IntPtr.Zero)
            {
                bRet = DbgHelp.MiniDumpWriteDump(currentProcessHandle, currentProcessId, fileHandle, (uint)options, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            }

            else
            {
                bRet = DbgHelp.MiniDumpWriteDump(currentProcessHandle, currentProcessId, fileHandle, (uint)options, ref exp, IntPtr.Zero, IntPtr.Zero);
            }

            return bRet;
        }



        public static bool Write(SafeHandle fileHandle, DbgHelp.Option dumpType)
        {
            return Write(fileHandle, dumpType, DbgHelp.ExceptionInfo.None);
        }

    }
}
