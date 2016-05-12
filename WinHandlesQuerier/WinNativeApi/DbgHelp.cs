using System;
using System.Runtime.InteropServices;

namespace DbgHelp
{
    public class Functions
    {
        public const int ERR_ELEMENT_NOT_FOUND = 1168;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);


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

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MINIDUMP_MODULE
    {
        public UInt64 BaseOfImage;
        public UInt32 SizeOfImage;
        public UInt32 CheckSum;
        public UInt32 TimeDateStamp;
        public uint ModuleNameRva;
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
        public uint TypeNameRva;
        public uint ObjectNameRva;
        public UInt32 Attributes;
        public UInt32 GrantedAccess;
        public UInt32 HandleCount;
        public UInt32 PointerCount;
        public uint ObjectInfoRva;
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
        public uint TypeNameRva;
        public uint ObjectNameRva;
        public UInt32 Attributes;
        public UInt32 GrantedAccess;
        public UInt32 HandleCount;
        public UInt32 PointerCount;
    }



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

   
}
