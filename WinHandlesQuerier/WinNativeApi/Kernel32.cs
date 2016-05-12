using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using HANDLE = System.IntPtr;

namespace Kernel32
{
    public class Functions
    {
        public const Int32 INFINITE = -1;
        public const Int32 WAIT_ABANDONED = 0x80;
        public const Int32 WAIT_OBJECT_0 = 0x00;
        public const Int32 WAIT_TIMEOUT = 0x102;
        public const Int32 WAIT_FAILED = -1;

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetFileInformationByHandleEx(HANDLE hFile, FILE_INFO_BY_HANDLE_CLASS infoClass, out FILE_ID_BOTH_DIR_INFO dirInfo, uint dwBufferSize);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern HANDLE CreateEvent(SECURITY_ATTRIBUTES lpSecurityAttributes, bool isManualReset, bool initialState, string name);

        [DllImport("Kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool Beep(uint dwFreq, uint dwDuration);

        [DllImport("Kernel32.dll")]
        public static extern uint WaitForMultipleObjects(uint nCount, IntPtr[] lpHandles, bool bWaitAll, uint dwMilliseconds);

        [DllImport("Kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        public static extern HANDLE CreateEvent(HANDLE lpEventAttributes, [In, MarshalAs(UnmanagedType.Bool)] bool bManualReset, [In, MarshalAs(UnmanagedType.Bool)] bool bIntialState, [In, MarshalAs(UnmanagedType.BStr)] string lpName);

        [DllImport("Kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(HANDLE hObject);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern Int32 WaitForSingleObject(IntPtr Handle, uint Wait);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualQuery(SafeMemoryMappedViewHandle address, ref MEMORY_BASIC_INFORMATION buffer, IntPtr sizeOfBuffer);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeMemoryMappedViewHandle MapViewOfFile(
            SafeMemoryMappedFileHandle hFileMappingObject,
            Kernel32.FileMapAccess dwDesiredAccess,
            uint dwFileOffsetHigh,
            uint dwFileOffsetLow,
            IntPtr dwNumberOfBytesToMap);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, uint processId);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool SetPrivilege(HANDLE token, ProcessAccessFlags Privilege, bool EnablePrivilege);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenFileMapping(
          uint dwDesiredAccess,
          bool bInheritHandle,
          string lpName);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFileMapping(
               IntPtr hFile,
               ref SECURITY_ATTRIBUTES attributes,
               FileMapProtection flProtect,
               uint dwMaximumSizeHigh,
               uint dwMaximumSizeLow,
               string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DuplicateHandle(IntPtr hSourceProcessHandle,
   IntPtr hSourceHandle, IntPtr hTargetProcessHandle, out IntPtr lpTargetHandle,
   uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, DuplicateOptions options);


        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateMutex(IntPtr lpMutexAttributes, bool bInitialOwner,
  string lpName);


        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void InitializeCriticalSection(out IntPtr
    lpCriticalSection);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void EnterCriticalSection(ref IntPtr
   lpCriticalSection);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void LeaveCriticalSection(ref IntPtr
           lpCriticalSection);

        [DllImport("kernel32.dll")]
        public static extern bool TryEnterCriticalSection(ref IntPtr
   lpCriticalSection);
        /// <summary>
        /// This value can be returned by CreateMutex() and is found in
        /// C++ in the error.h header file.
        /// </summary>
        public const int ERROR_ALREADY_EXISTS = 183;
        public const uint SYNCHRONIZE = 0x00100000;

        [DllImport("kernel32.dll")]
        public static extern bool ReleaseMutex(IntPtr hMutex);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenMutex(uint dwDesiredAccess, bool bInheritHandle, string lpName);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool SetSecurityDescriptorDacl(ref SECURITY_DESCRIPTOR securityDescriptor, bool daclPresent, IntPtr dacl, bool daclDefaulted);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool InitializeSecurityDescriptor(out SECURITY_DESCRIPTOR securityDescriptor, uint dwRevision);


    }



    public enum FileMapAccessType : uint
    {
        Copy = 0x01,
        Write = 0x02,
        Read = 0x04,
        AllAccess = 0x08,
        Execute = 0x20,
    }

    [Flags]
    public enum DuplicateOptions : uint
    {
        DUPLICATE_CLOSE_SOURCE = (0x00000001),// Closes the source handle. This occurs regardless of any error status returned.
        DUPLICATE_SAME_ACCESS = (0x00000002), //Ignores the dwDesiredAccess parameter. The duplicate handle has the same access as the source handle.
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct FILE_ID_BOTH_DIR_INFO
    {
        public uint NextEntryOffset;
        public uint FileIndex;
        public long CreationTime;
        public long LastAccessTime;
        public long LastWriteTime;
        public long ChangeTime;
        public long EndOfFile;
        public long AllocationSize;
        public uint FileAttributes;
        public uint FileNameLength;
        public uint EaSize;
        public char ShortNameLength;
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string ShortName;
        public long FileId;
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 1)]
        public string FileName;
    }

    public enum FILE_INFO_BY_HANDLE_CLASS
    {
        FileBasicInfo = 0,
        FileStandardInfo = 1,
        FileNameInfo = 2,
        FileRenameInfo = 3,
        FileDispositionInfo = 4,
        FileAllocationInfo = 5,
        FileEndOfFileInfo = 6,
        FileStreamInfo = 7,
        FileCompressionInfo = 8,
        FileAttributeTagInfo = 9,
        FileIdBothDirectoryInfo = 10,// 0x0A
        FileIdBothDirectoryRestartInfo = 11, // 0xB
        FileIoPriorityHintInfo = 12, // 0xC
        FileRemoteProtocolInfo = 13, // 0xD
        FileFullDirectoryInfo = 14, // 0xE
        FileFullDirectoryRestartInfo = 15, // 0xF
        FileStorageInfo = 16, // 0x10
        FileAlignmentInfo = 17, // 0x11
        FileIdInfo = 18, // 0x12
        FileIdExtdDirectoryInfo = 19, // 0x13
        FileIdExtdDirectoryRestartInfo = 20, // 0x14
        MaximumFileInfoByHandlesClass
    }

    public struct VS_FIXEDFILEINFO
    {
        public UInt32 dwSignature;
        public UInt32 dwStrucVersion;
        public UInt32 dwFileVersionMS;
        public UInt32 dwFileVersionLS;
        public UInt32 dwProductVersionMS;
        public UInt32 dwProductVersionLS;
        public UInt32 dwFileFlagsMask;
        public UInt32 dwFileFlags;
        public UInt32 dwFileOS;
        public UInt32 dwFileType;
        public UInt32 dwFileSubtype;
        public UInt32 dwFileDateMS;
        public UInt32 dwFileDateLS;
    };


    [StructLayout(LayoutKind.Sequential)]
    public struct SECURITY_ATTRIBUTES
    {
        public int nLength;
        public unsafe byte* lpSecurityDescriptor;
        public int bInheritHandle;
    }



    [Flags]
    public enum ProcessAccessFlags : uint
    {
        All = 0x001F0FFF,
        Terminate = 0x00000001,
        CreateThread = 0x00000002,
        VirtualMemoryOperation = 0x00000008,
        VirtualMemoryRead = 0x00000010,
        VirtualMemoryWrite = 0x00000020,
        DuplicateHandle = 0x00000040,
        CreateProcess = 0x000000080,
        SetQuota = 0x00000100,
        SetInformation = 0x00000200,
        QueryInformation = 0x00000400,
        QueryLimitedInformation = 0x00001000,
        Synchronize = 0x00100000
    }

    public struct MEMORY_BASIC_INFORMATION
    {
        public IntPtr BaseAddress;
        public IntPtr AllocationBase;
        public uint AllocationProtect;
        public UIntPtr RegionSize;
        public uint State;
        public uint Protect;
        public uint Type;
    }



    [Flags]
    public enum FileMapAccess : uint
    {
        FileMapCopy = 0x0001,
        FileMapWrite = 0x0002,
        FileMapRead = 0x0004,
        FileMapAllAccess = 0x001f,
        FileMapExecute = 0x0020,
    }

    [Flags]
    public enum FileMapProtection : uint
    {
        PageReadonly = 0x02,
        PageReadWrite = 0x04,
        PageWriteCopy = 0x08,
        PageExecuteRead = 0x20,
        PageExecuteReadWrite = 0x40,
        SectionCommit = 0x8000000,
        SectionImage = 0x1000000,
        SectionNoCache = 0x10000000,
        SectionReserve = 0x4000000,
    }

    /// <summary>
    /// Security enumeration from:
    /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dllproc/base/synchronization_object_security_and_access_rights.asp
    /// </summary>
    [Flags]
    public enum SyncObjectAccess : uint
    {
        DELETE = 0x00010000,
        READ_CONTROL = 0x00020000,
        WRITE_DAC = 0x00040000,
        WRITE_OWNER = 0x00080000,
        SYNCHRONIZE = 0x00100000,
        EVENT_ALL_ACCESS = 0x001F0003,
        EVENT_MODIFY_STATE = 0x00000002,
        MUTEX_ALL_ACCESS = 0x001F0001,
        MUTEX_MODIFY_STATE = 0x00000001,
        SEMAPHORE_ALL_ACCESS = 0x001F0003,
        SEMAPHORE_MODIFY_STATE = 0x00000002,
        TIMER_ALL_ACCESS = 0x001F0003,
        TIMER_MODIFY_STATE = 0x00000002,
        TIMER_QUERY_STATE = 0x00000001
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct SECURITY_DESCRIPTOR
    {
        public byte revision;
        public byte size;
        public short control;
        public IntPtr owner;
        public IntPtr group;
        public IntPtr sacl;
        public IntPtr dacl;
    }
}
