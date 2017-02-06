using System;
using System.Runtime.InteropServices;

namespace DbgHelp
{
    public class Functions
    {
        public const int ERR_ELEMENT_NOT_FOUND = 1168;


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool MINIDUMP_CALLBACK_ROUTINE(
            [In] IntPtr CallbackParam,
            [In] ref MINIDUMP_CALLBACK_INPUT CallbackInput,
            [In, Out] ref MINIDUMP_CALLBACK_OUTPUT CallbackOutput
            );

        [DllImport("dbghelp.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MiniDumpWriteDump(
            IntPtr hProcess,
            uint ProcessId,
            IntPtr hFile,
            MINIDUMP_TYPE DumpType,
            [In] ref MINIDUMP_EXCEPTION_INFORMATION ExceptionParam,
            [In] ref MINIDUMP_USER_STREAM_INFORMATION UserStreamParam,
            [In] ref MINIDUMP_CALLBACK_INFORMATION CallbackParam
            );

        [DllImport("DbgHelp.dll", SetLastError = true)]
        public extern static bool MiniDumpWriteDump(
            IntPtr hProcess,
            UInt32 ProcessId, // DWORD is a 32 bit unsigned integer
            SafeHandle hFile,
            DbgHelp.MINIDUMP_TYPE DumpType,
            IntPtr exceptionParam,
            IntPtr userStream,
            IntPtr callback);

        [DllImport("Dbghelp.dll", SetLastError = true)]
        public static extern bool MiniDumpReadDumpStream(IntPtr BaseOfDump,
            DbgHelp.MINIDUMP_STREAM_TYPE StreamNumber,
            ref MINIDUMP_DIRECTORY Dir,
            ref IntPtr StreamPointer,
            ref uint StreamSize);
    }

    #region Structs
    
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MINIDUMP_EXCEPTION_INFORMATION
    {
        public uint ThreadId;
        public IntPtr ExceptionPointers;
        [MarshalAs(UnmanagedType.Bool)]
        public bool ClientPointers;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MINIDUMP_USER_STREAM
    {
        public MINIDUMP_STREAM_TYPE Type;
        public uint BufferSize;
        public IntPtr Buffer;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MINIDUMP_USER_STREAM_INFORMATION
    {
        public MINIDUMP_USER_STREAM_INFORMATION(params MINIDUMP_USER_STREAM[] streams)
        {
            UserStreamCount = (uint)streams.Length;
            int sizeOfStream = Marshal.SizeOf(typeof(MINIDUMP_USER_STREAM));
            UserStreamArray = Marshal.AllocHGlobal((int)(UserStreamCount * sizeOfStream));
            for (int i = 0; i < streams.Length; ++i)
            {
                Marshal.StructureToPtr(streams[i], UserStreamArray + (i * sizeOfStream), false);
            }
        }

        public void Delete()
        {
            Marshal.FreeHGlobal(UserStreamArray);
            UserStreamCount = 0;
            UserStreamArray = IntPtr.Zero;
        }

        public uint UserStreamCount;
        public IntPtr UserStreamArray;
    }

    public enum MINIDUMP_CALLBACK_TYPE : uint
    {
        ModuleCallback,
        ThreadCallback,
        ThreadExCallback,
        IncludeThreadCallback,
        IncludeModuleCallback,
        MemoryCallback,
        CancelCallback,
        WriteKernelMinidumpCallback,
        KernelMinidumpStatusCallback,
        RemoveMemoryCallback,
        IncludeVmRegionCallback,
        IoStartCallback,
        IoWriteAllCallback,
        IoFinishCallback,
        ReadMemoryFailureCallback,
        SecondaryFlagsCallback
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct MINIDUMP_THREAD_CALLBACK
    {
        public uint ThreadId;
        public IntPtr ThreadHandle;
#if X64
        public fixed byte Context[1232];
#else
        public fixed byte Context[716];
#endif
        public uint SizeOfContext;
        public ulong StackBase;
        public ulong StackEnd;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MINIDUMP_THREAD_EX_CALLBACK
    {
        public MINIDUMP_THREAD_CALLBACK BasePart;
        public ulong BackingStoreBase;
        public ulong BackingStoreEnd;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct VS_FIXEDFILEINFO
    {
        public uint dwSignature;
        public uint dwStrucVersion;
        public uint dwFileVersionMS;
        public uint dwFileVersionLS;
        public uint dwProductVersionMS;
        public uint dwProductVersionLS;
        public uint dwFileFlagsMask;
        public uint dwFileFlags;
        public uint dwFileOS;
        public uint dwFileType;
        public uint dwFileSubtype;
        public uint dwFileDateMS;
        public uint dwFileDateLS;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MINIDUMP_MODULE_CALLBACK
    {
        public IntPtr FullPath; // This is a PCWSTR
        public ulong BaseOfImage;
        public uint SizeOfImage;
        public uint CheckSum;
        public uint TimeDateStamp;
        public VS_FIXEDFILEINFO VersionInfo;
        public IntPtr CvRecord;
        public uint SizeOfCvRecord;
        public IntPtr MiscRecord;
        public uint SizeOfMiscRecord;
    }

    public struct MINIDUMP_INCLUDE_THREAD_CALLBACK
    {
        public uint ThreadId;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MINIDUMP_INCLUDE_MODULE_CALLBACK
    {
        public ulong BaseOfImage;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MINIDUMP_IO_CALLBACK
    {
        public IntPtr Handle;
        public ulong Offset;
        public IntPtr Buffer;
        public ulong BufferBytes;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MINIDUMP_READ_MEMORY_FAILURE_CALLBACK
    {
        public ulong Offset;
        public uint Bytes;
        public int FailureStatus; // HRESULT
    }

    [Flags]
    public enum MINIDUMP_SECONDARY_FLAGS : uint
    {
        MiniSecondaryWithoutPowerInfo = 0x00000001
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct MINIDUMP_CALLBACK_INPUT
    {
#if X64
        const int CallbackTypeOffset = 4 + 8;
#else
        const int CallbackTypeOffset = 4 + 4;
#endif
        const int UnionOffset = CallbackTypeOffset + 4;

        [FieldOffset(0)]
        public uint ProcessId;
        [FieldOffset(4)]
        public IntPtr ProcessHandle;
        [FieldOffset(CallbackTypeOffset)]
        public MINIDUMP_CALLBACK_TYPE CallbackType;

        [FieldOffset(UnionOffset)]
        public int Status; // HRESULT
        [FieldOffset(UnionOffset)]
        public MINIDUMP_THREAD_CALLBACK Thread;
        [FieldOffset(UnionOffset)]
        public MINIDUMP_THREAD_EX_CALLBACK ThreadEx;
        [FieldOffset(UnionOffset)]
        public MINIDUMP_MODULE_CALLBACK Module;
        [FieldOffset(UnionOffset)]
        public MINIDUMP_INCLUDE_THREAD_CALLBACK IncludeThread;
        [FieldOffset(UnionOffset)]
        public MINIDUMP_INCLUDE_MODULE_CALLBACK IncludeModule;
        [FieldOffset(UnionOffset)]
        public MINIDUMP_IO_CALLBACK Io;
        [FieldOffset(UnionOffset)]
        public MINIDUMP_READ_MEMORY_FAILURE_CALLBACK ReadMemoryFailure;
        [FieldOffset(UnionOffset)]
        public MINIDUMP_SECONDARY_FLAGS SecondaryFlags;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MINIDUMP_MEMORY_INFO
    {
        public ulong BaseAddress;
        public ulong AllocationBase;
        public uint AllocationProtect;
        public uint __alignment1;
        public ulong RegionSize;
        public STATE State;
        public PROTECT Protect;
        public TYPE Type;
        public uint __alignment2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MemoryCallbackOutput
    {
        public ulong MemoryBase;
        public uint MemorySize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct CancelCallbackOutput
    {
        [MarshalAs(UnmanagedType.Bool)]
        public bool CheckCancel;
        [MarshalAs(UnmanagedType.Bool)]
        public bool Cancel;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MemoryInfoCallbackOutput
    {
        public MINIDUMP_MEMORY_INFO VmRegion;
        [MarshalAs(UnmanagedType.Bool)]
        public bool Continue;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 4)]
    public struct MINIDUMP_CALLBACK_OUTPUT
    {
        [FieldOffset(0)]
        public MODULE_WRITE_FLAGS ModuleWriteFlags;
        [FieldOffset(0)]
        public THREAD_WRITE_FLAGS ThreadWriteFlags;
        [FieldOffset(0)]
        public uint SecondaryFlags;
        [FieldOffset(0)]
        public MemoryCallbackOutput Memory;
        [FieldOffset(0)]
        public CancelCallbackOutput Cancel;
        [FieldOffset(0)]
        public IntPtr Handle;
        [FieldOffset(0)]
        public MemoryInfoCallbackOutput MemoryInfo;
        [FieldOffset(0)]
        public int Status; // HRESULT
    }

    public struct MINIDUMP_CALLBACK_INFORMATION
    {
        public Functions.MINIDUMP_CALLBACK_ROUTINE CallbackRoutine;
        public IntPtr CallbackParam;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MINIDUMP_MODULE
    {
        public UInt64 BaseOfImage;
        public UInt32 SizeOfImage;
        public UInt32 CheckSum;
        public UInt32 TimeDateStamp;
        public Int32 ModuleNameRva;
        public Kernel32.VS_FIXEDFILEINFO VersionInfo;
        public MINIDUMP_LOCATION_DESCRIPTOR CvRecord;
        public MINIDUMP_LOCATION_DESCRIPTOR MiscRecord;
        public UInt64 Reserved0;
        public UInt64 Reserved1;
    }

    public struct MINIDUMP_MODULE_LIST
    {
        public UInt32 NumberOfModules;
        public IntPtr Modules;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MINIDUMP_SYSTEM_INFO
    {
        public ushort ProcessorArchitecture;
        public ushort ProcessorLevel;
        public ushort ProcessorRevision;
        public byte NumberOfProcessors;
        public byte ProductType;
        public UInt32 MajorVersion;
        public UInt32 MinorVersion;
        public UInt32 BuildNumber;
        public UInt32 PlatformId;
        public uint CSDVersionRva;
        public ushort SuiteMask;
        public ushort Reserved2;
        public CPU_INFORMATION Cpu;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct CPU_INFORMATION
    {
        // OtherCpuInfo
        [FieldOffset(0)]
        public fixed ulong ProcessorFeatures[2];
        // X86CpuInfo, Official VendorId is 3 * 32bit long's (EAX, EBX and ECX).
        // It actually stores a 12 byte ASCII string though, so it's easier for us to treat it as a 12 byte array instead.
        [FieldOffset(0)]
        public fixed byte VendorId[12];
        [FieldOffset(12)]
        public UInt32 VersionInformation;
        [FieldOffset(16)]
        public UInt32 FeatureInformation;
        [FieldOffset(20)]
        public UInt32 AMDExtendedCpuFeatures;
    }

    public unsafe struct MINIDUMP_STRING
    {
        public UInt32 Length;
        public char* Buffer;
    }

    public struct MINIDUMP_HANDLE_DATA_STREAM
    {
        public UInt32 SizeOfHeader;
        public UInt32 SizeOfDescriptor;
        public UInt32 NumberOfDescriptors;
        public UInt32 Reserved;
    }

    public struct MINIDUMP_HANDLE_DESCRIPTOR_2
    {
        public UInt64 Handle;
        public Int32 TypeNameRva;
        public Int32 ObjectNameRva;
        public UInt32 Attributes;
        public UInt32 GrantedAccess;
        public UInt32 HandleCount;
        public UInt32 PointerCount;
        public Int32 ObjectInfoRva;
        public UInt32 Reserved0;
    }

    public struct MINIDUMP_HANDLE_OBJECT_INFORMATION
    {
        public uint NextInfoRva;
        public MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE InfoType;
        public UInt32 SizeOfInfo;
    }

    public struct MINIDUMP_HANDLE_DESCRIPTOR
    {
        public UInt64 Handle;
        public Int32 TypeNameRva;
        public Int32 ObjectNameRva;
        public UInt32 Attributes;
        public UInt32 GrantedAccess;
        public UInt32 HandleCount;
        public UInt32 PointerCount;
    }

    public struct MiniDumpExceptionInformation
    {
        public uint ThreadId;
        public IntPtr ExceptionPointers;
        [MarshalAs(UnmanagedType.Bool)]
        public bool ClientPointers;
    }

    public struct MINIDUMP_DIRECTORY
    {
        public UInt32 StreamType;
        public MINIDUMP_LOCATION_DESCRIPTOR Location;
    }

    public struct MINIDUMP_LOCATION_DESCRIPTOR
    {
        public UInt32 DataSize;
        public uint Rva;
    }

    #endregion

    #region Enums


    public enum STATE : uint
    {
        MEM_COMMIT = 0x1000,
        MEM_FREE = 0x10000,
        MEM_RESERVE = 0x2000
    }

    public enum TYPE : uint
    {
        MEM_IMAGE = 0x1000000,
        MEM_MAPPED = 0x40000,
        MEM_PRIVATE = 0x20000
    }

    [Flags]
    public enum PROTECT : uint
    {
        PAGE_EXECUTE = 0x10,
        PAGE_EXECUTE_READ = 0x20,
        PAGE_EXECUTE_READWRITE = 0x40,
        PAGE_EXECUTE_WRITECOPY = 0x80,
        PAGE_NOACCESS = 0x01,
        PAGE_READONLY = 0x02,
        PAGE_READWRITE = 0x04,
        PAGE_WRITECOPY = 0x08,
        PAGE_TARGETS_INVALID = 0x40000000,
        PAGE_TARGETS_NO_UPDATE = 0x40000000,

        PAGE_GUARD = 0x100,
        PAGE_NOCACHE = 0x200,
        PAGE_WRITECOMBINE = 0x400
    }

    [Flags]
    public enum THREAD_WRITE_FLAGS : uint
    {
        ThreadWriteThread = 0x0001,
        ThreadWriteStack = 0x0002,
        ThreadWriteContext = 0x0004,
        ThreadWriteBackingStore = 0x0008,
        ThreadWriteInstructionWindow = 0x0010,
        ThreadWriteThreadData = 0x0020,
        ThreadWriteThreadInfo = 0x0040
    }

    [Flags]
    public  enum MODULE_WRITE_FLAGS : uint
    {
        ModuleWriteModule = 0x0001,
        ModuleWriteDataSeg = 0x0002,
        ModuleWriteMiscRecord = 0x0004,
        ModuleWriteCvRecord = 0x0008,
        ModuleReferencedByMemory = 0x0010,
        ModuleWriteTlsData = 0x0020,
        ModuleWriteCodeSegs = 0x0040
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

    public enum MINIDUMP_STREAM_TYPE : uint
    {
        UnusedStream = 0,
        ReservedStream0 = 1,
        ReservedStream1 = 2,
        ThreadListStream = 3,
        ModuleListStream = 4,
        MemoryListStream = 5,
        ExceptionStream = 6,
        SystemInfoStream = 7,
        ThreadExListStream = 8,
        Memory64ListStream = 9,
        CommentStreamA = 10,
        CommentStreamW = 11,
        HandleDataStream = 12,
        FunctionTableStream = 13,
        UnloadedModuleListStream = 14,
        MiscInfoStream = 15,
        MemoryInfoListStream = 16,
        ThreadInfoListStream = 17,
        HandleOperationListStream = 18,
        LastReservedStream = 0xffff
    }

    public enum MiniDumpProcessorArchitecture
    {
        PROCESSOR_ARCHITECTURE_INTEL = 0,
        PROCESSOR_ARCHITECTURE_IA64 = 6,
        PROCESSOR_ARCHITECTURE_AMD64 = 9,
        PROCESSOR_ARCHITECTURE_UNKNOWN = 0xfff
    }

    public enum MiniDumpProductType
    {
        VER_NT_WORKSTATION = 0x0000001,
        VER_NT_DOMAIN_CONTROLLER = 0x0000002,
        VER_NT_SERVER = 0x0000003
    }

    public enum MiniDumpPlatform
    {
        VER_PLATFORM_WIN32s = 0,
        VER_PLATFORM_WIN32_WINDOWS = 1,
        VER_PLATFORM_WIN32_NT = 2
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

    // Per-handle object information varies according to
    // the OS, the OS version, the processor type and
    // so on.  The minidump gives a minidump identifier
    // to each possible data format for identification
    // purposes but does not control nor describe the actual data.
    public enum MINIDUMP_HANDLE_OBJECT_INFORMATION_TYPE : uint
    {
        MiniHandleObjectInformationNone,
        MiniThreadInformation1,
        MiniMutantInformation1,
        MiniMutantInformation2,
        MiniProcessInformation1,
        MiniProcessInformation2,
        MiniEventInformation1,
        MiniSectionInformation1,
        MiniHandleObjectInformationTypeMax
    }

    public enum Option : uint
    {
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

    #endregion
}
