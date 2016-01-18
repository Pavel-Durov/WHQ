using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Assignments.Core.WinApi
{
    public class DbgHelp
    {
        [DllImport("Dbghelp.dll")]
        public static extern bool MiniDumpWriteDump(IntPtr hProcess, uint ProcessId, SafeHandle hFile, MINIDUMP_TYPE DumpType, 
            ref MINIDUMP_EXCEPTION_INFORMATION ExceptionParam, IntPtr UserStreamParam, ref MINIDUMP_MODULE_CALLBACK CallbackParam);


        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct MINIDUMP_EXCEPTION_INFORMATION
        {
            public uint ThreadId;
            public IntPtr ExceptionPointers;
            public int ClientPointers;
        }

        public enum MINIDUMP_TYPE : uint
        {
            MiniDumpNormal = 0x00000000,
            MiniDumpWithDataSegs = 0x00000001,
            MiniDumpWithFullMemory = 0x00000002,
            MiniDumpWithHandleData = 0x00000004,
            MiniDumpFilterMemory = 0x00000008,
            MiniDumpScanMemory = 0x00000010,
            MiniDumpWithUnloadedModules = 0x00000020,
            MiniDumpWithIndirectlyReferencedMemory = 0x00000040,
            MiniDumpFilterModulePaths = 0x00000080,
            MiniDumpWithProcessThreadData = 0x00000100,
            MiniDumpWithPrivateReadWriteMemory = 0x00000200,
            MiniDumpWithoutOptionalData = 0x00000400,
            MiniDumpWithFullMemoryInfo = 0x00000800,
            MiniDumpWithThreadInfo = 0x00001000,
            MiniDumpWithCodeSegs = 0x00002000,
            MiniDumpWithoutManagedState = 0x00004000,
        }


   


        // Taken almost verbatim from http://blog.kalmbach-software.de/2008/12/13/writing-minidumps-in-c/

        [Flags]
        public enum Option : uint
        {
            // From dbghelp.h:
            Normal = 0x00000000,
            WithDataSegs = 0x00000001,
            WithFullMemory = 0x00000002,
            WithHandleData = 0x00000004,
            FilterMemory = 0x00000008,
            ScanMemory = 0x00000010,
            WithUnloadedModules = 0x00000020,
            WithIndirectlyReferencedMemory = 0x00000040,
            FilterModulePaths = 0x00000080,
            WithProcessThreadData = 0x00000100,
            WithPrivateReadWriteMemory = 0x00000200,
            WithoutOptionalData = 0x00000400,
            WithFullMemoryInfo = 0x00000800,
            WithThreadInfo = 0x00001000,
            WithCodeSegs = 0x00002000,
            WithoutAuxiliaryState = 0x00004000,
            WithFullAuxiliaryState = 0x00008000,
            WithPrivateWriteCopyMemory = 0x00010000,
            IgnoreInaccessibleMemory = 0x00020000,
            ValidTypeFlags = 0x0003ffff,
        };


        [StructLayout(LayoutKind.Sequential)]
        public struct MINIDUMP_MODULE_CALLBACK
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            String FullPath;
            UInt64 BaseOfImage;
            UInt32 SizeOfImage;
            UInt32 CheckSum;
            UInt32 TimeDateStamp;
            VS_FIXEDFILEINFO VersionInfo;
            IntPtr CvRecord;
            UInt32 SizeOfCvRecord;
            IntPtr MiscRecord;
            UInt32 SizeOfMiscRecord;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct VS_FIXEDFILEINFO
        {
            UInt32 dwSignature;
            UInt32 dwStrucVersion;
            UInt32 dwFileVersionMS;
            UInt32 dwFileVersionLS;
            UInt32 dwProductVersionMS;
            UInt32 dwProductVersionLS;
            UInt32 dwFileFlagsMask;
            UInt32 dwFileFlags;
            UInt32 dwFileOS;
            UInt32 dwFileType;
            UInt32 dwFileSubtype;
            UInt32 dwFileDateMS;
            UInt32 dwFileDateLS;
        }

        //typedef struct _MINIDUMP_EXCEPTION_INFORMATION 
        //    DWORD ThreadId;
        //    PEXCEPTION_POINTERS ExceptionPointers;
        //    BOOL ClientPointers;
        //} MINIDUMP_EXCEPTION_INFORMATION, *PMINIDUMP_EXCEPTION_INFORMATION;
        [StructLayout(LayoutKind.Sequential, Pack = 4)]  // Pack=4 is important! So it works also for x64!
        public struct MiniDumpExceptionInformation
        {
            public uint ThreadId;
            public IntPtr ExceptionPointers;
            [MarshalAs(UnmanagedType.Bool)]
            public bool ClientPointers;
        }

        public enum ExceptionInfo
        {
            None,
            Present
        }





        // Overload requiring MiniDumpExceptionInformation

        //[DllImport("dbghelp.dll", EntryPoint = "MiniDumpWriteDump", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]

        //public static extern bool MiniDumpWriteDump(IntPtr hProcess, uint processId, SafeHandle hFile, uint dumpType, ref MiniDumpExceptionInformation expParam, IntPtr userStreamParam, IntPtr callbackParam);



        //// Overload supporting MiniDumpExceptionInformation == NULL

        //[DllImport("dbghelp.dll", EntryPoint = "MiniDumpWriteDump", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]

        //public static extern bool MiniDumpWriteDump(IntPtr hProcess, uint processId, SafeHandle hFile, uint dumpType, IntPtr expParam, IntPtr userStreamParam, IntPtr callbackParam);



        //[DllImport("kernel32.dll", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
        //public static extern uint GetCurrentThreadId();

    }
}
