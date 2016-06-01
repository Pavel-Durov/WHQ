using System.Runtime.InteropServices;
namespace WinNativeApi.WinNT
{
    #region Structs

    //https://msdn.microsoft.com/en-us/library/windows/desktop/aa383751(v=vs.85).aspx
    using DWORD64 = System.UInt64;
    using DWORD = System.Int32;
    using WORD = System.SByte;
    using ULONGLONG = System.UInt64;
    using LONGLONG = System.Int64;

    //https://social.msdn.microsoft.com/Forums/vstudio/en-US/f5af96bc-88ad-4b56-98a3-c6dc5114cb8a/getthreadcontext?forum=vcgeneral
    [StructLayout(LayoutKind.Explicit, Pack = 16)]
    public unsafe struct CONTEXT_AMD64
    {
        //
        // Register parameter home addresses.
        //
        // N.B. These fields are for convience - they could be used to extend the
        //      context record in the future.
        //
        [FieldOffset(0)]
        public DWORD64 P1Home;
        [FieldOffset(1)]
        public DWORD64 P2Home;
        [FieldOffset(3)]
        public DWORD64 P3Home;
            [FieldOffset(4)]
        public DWORD64 P4Home;
        [FieldOffset(5)]
        public DWORD64 P5Home;
        [FieldOffset(6)]
        public DWORD64 P6Home;

        //
        // Control flags.
        //
        [FieldOffset(7)]
        public DWORD ContextFlags;
        [FieldOffset(8)]
        public DWORD MxCsr;

        //
        // Segment Registers and processor flags.
        //
        [FieldOffset(9)]
        public WORD SegCs;
        [FieldOffset(10)]
        public WORD SegDs;
        [FieldOffset(11)]
        public WORD SegEs;
        [FieldOffset(12)]
        public WORD SegFs;
        [FieldOffset(13)]
        public WORD SegGs;
        [FieldOffset(14)]
        public WORD SegSs;
        [FieldOffset(15)]
        public DWORD EFlags;

        //
        // Debug registers
        //
        [FieldOffset(16)]
        public DWORD64 Dr0;
        [FieldOffset(17)]
        public DWORD64 Dr1;
        [FieldOffset(18)]
        public DWORD64 Dr2;
        [FieldOffset(19)]
        public DWORD64 Dr3;
        [FieldOffset(20)]
        public DWORD64 Dr6;
        [FieldOffset(21)]
        public DWORD64 Dr7;

        //
        // Integer registers.
        //
        [FieldOffset(22)]
        public DWORD64 Rax;
        [FieldOffset(23)]
        public DWORD64 Rcx;
        [FieldOffset(24)]
        public DWORD64 Rdx;
        [FieldOffset(25)]
        public DWORD64 Rbx;
        [FieldOffset(26)]
        public DWORD64 Rsp;
        [FieldOffset(27)]
        public DWORD64 Rbp;
        [FieldOffset(28)]
        public DWORD64 Rsi;
        [FieldOffset(29)]
        public DWORD64 Rdi;
        [FieldOffset(30)]
        public DWORD64 R8;
        [FieldOffset(31)]
        public DWORD64 R9;
        [FieldOffset(32)]
        public DWORD64 R10;
        [FieldOffset(33)]
        public DWORD64 R11;
        [FieldOffset(34)]
        public DWORD64 R12;
        [FieldOffset(35)]
        public DWORD64 R13;
        [FieldOffset(36)]
        public DWORD64 R14;
        [FieldOffset(37)]
        public DWORD64 R15;

        //
        // Program counter.
        //
        [FieldOffset(38)]
        public DWORD64 Rip;

        //
        // Floating point state.
        //
        [FieldOffset(39)]

        public DUMMYUNIONNAME dummyUnion;

        //
        // Vector registers.
        //
        [FieldOffset(40)]
        M128A* VectorRegister;
        [FieldOffset(41)]
        public DWORD64 VectorControl;

        //
        // Special debug control registers.
        //
        [FieldOffset(42)]
        public DWORD64 DebugControl;
        [FieldOffset(43)]
        public DWORD64 LastBranchToRip;
        [FieldOffset(44)]
        public DWORD64 LastBranchFromRip;
        [FieldOffset(45)]
        public DWORD64 LastExceptionToRip;
        [FieldOffset(46)]
        public DWORD64 LastExceptionFromRip;
    }

    struct XMM_SAVE_AREA32
    {

    }

    public unsafe struct DUMMY
    {
        M128A* Header;
        M128A* Legacy;
        M128A Xmm0;
        M128A Xmm1;
        M128A Xmm2;
        M128A Xmm3;
        M128A Xmm4;
        M128A Xmm5;
        M128A Xmm6;
        M128A Xmm7;
        M128A Xmm8;
        M128A Xmm9;
        M128A Xmm10;
        M128A Xmm11;
        M128A Xmm12;
        M128A Xmm13;
        M128A Xmm14;
        M128A Xmm15;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct DUMMYUNIONNAME
    {
        [FieldOffset(0)]
        XMM_SAVE_AREA32 FltSave;
        [FieldOffset(0)]
        DUMMY Dummy;
    }

    struct M128A
    {
        ULONGLONG Low;
        LONGLONG High;
    };

    #endregion
}

public enum CONTEXT_FLAGS : uint
{
    CONTEXT_i386 = 0x10000,
    CONTEXT_i486 = 0x10000,   //  same as i386
    CONTEXT_CONTROL = CONTEXT_i386 | 0x01, // SS:SP, CS:IP, FLAGS, BP
    CONTEXT_INTEGER = CONTEXT_i386 | 0x02, // AX, BX, CX, DX, SI, DI
    CONTEXT_SEGMENTS = CONTEXT_i386 | 0x04, // DS, ES, FS, GS
    CONTEXT_FLOATING_POINT = CONTEXT_i386 | 0x08, // 387 state
    CONTEXT_DEBUG_REGISTERS = CONTEXT_i386 | 0x10, // DB 0-3,6,7
    CONTEXT_EXTENDED_REGISTERS = CONTEXT_i386 | 0x20, // cpu specific extensions
    CONTEXT_FULL = CONTEXT_CONTROL | CONTEXT_INTEGER | CONTEXT_SEGMENTS,
    CONTEXT_ALL = CONTEXT_CONTROL | CONTEXT_INTEGER | CONTEXT_SEGMENTS | CONTEXT_FLOATING_POINT | CONTEXT_DEBUG_REGISTERS | CONTEXT_EXTENDED_REGISTERS
}


[StructLayout(LayoutKind.Sequential)]
public struct FLOATING_SAVE_AREA
{
    public uint ControlWord;
    public uint StatusWord;
    public uint TagWord;
    public uint ErrorOffset;
    public uint ErrorSelector;
    public uint DataOffset;
    public uint DataSelector;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
    public byte[] RegisterArea;
    public uint Cr0NpxState;
}

[StructLayout(LayoutKind.Sequential)]
public struct CONTEXT
{
    public uint ContextFlags; //set this to an appropriate value 
                              // Retrieved by CONTEXT_DEBUG_REGISTERS 
    public uint Dr0;
    public uint Dr1;
    public uint Dr2;
    public uint Dr3;
    public uint Dr6;
    public uint Dr7;
    // Retrieved by CONTEXT_FLOATING_POINT 
    public FLOATING_SAVE_AREA FloatSave;
    // Retrieved by CONTEXT_SEGMENTS 
    public uint SegGs;
    public uint SegFs;
    public uint SegEs;
    public uint SegDs;
    // Retrieved by CONTEXT_INTEGER 
    public uint Edi;
    public uint Esi;
    public uint Ebx;
    public uint Edx;
    public uint Ecx;
    public uint Eax;
    // Retrieved by CONTEXT_CONTROL 
    public uint Ebp;
    public uint Eip;
    public uint SegCs;
    public uint EFlags;
    public uint Esp;
    public uint SegSs;
    // Retrieved by CONTEXT_EXTENDED_REGISTERS 
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
    public byte[] ExtendedRegisters;
}



/*
typedef struct DECLSPEC_ALIGN(16) _CONTEXT {

    //
    // Register parameter home addresses.
    //
    // N.B. These fields are for convience - they could be used to extend the
    //      context record in the future.
    //

    DWORD64 P1Home;
    DWORD64 P2Home;
    DWORD64 P3Home;
    DWORD64 P4Home;
    DWORD64 P5Home;
    DWORD64 P6Home;

    //
    // Control flags.
    //

    DWORD ContextFlags;
    DWORD MxCsr;

    //
    // Segment Registers and processor flags.
    //

    WORD   SegCs;
    WORD   SegDs;
    WORD   SegEs;
    WORD   SegFs;
    WORD   SegGs;
    WORD   SegSs;
    DWORD EFlags;

    //
    // Debug registers
    //

    DWORD64 Dr0;
    DWORD64 Dr1;
    DWORD64 Dr2;
    DWORD64 Dr3;
    DWORD64 Dr6;
    DWORD64 Dr7;

    //
    // Integer registers.
    //

    DWORD64 Rax;
    DWORD64 Rcx;
    DWORD64 Rdx;
    DWORD64 Rbx;
    DWORD64 Rsp;
    DWORD64 Rbp;
    DWORD64 Rsi;
    DWORD64 Rdi;
    DWORD64 R8;
    DWORD64 R9;
    DWORD64 R10;
    DWORD64 R11;
    DWORD64 R12;
    DWORD64 R13;
    DWORD64 R14;
    DWORD64 R15;

    //
    // Program counter.
    //

    DWORD64 Rip;

    //
    // Floating point state.
    //

    union {
        XMM_SAVE_AREA32 FltSave;
        struct {
            M128A Header[2];
            M128A Legacy[8];
            M128A Xmm0;
            M128A Xmm1;
            M128A Xmm2;
            M128A Xmm3;
            M128A Xmm4;
            M128A Xmm5;
            M128A Xmm6;
            M128A Xmm7;
            M128A Xmm8;
            M128A Xmm9;
            M128A Xmm10;
            M128A Xmm11;
            M128A Xmm12;
            M128A Xmm13;
            M128A Xmm14;
            M128A Xmm15;
        } DUMMYSTRUCTNAME;
    } DUMMYUNIONNAME;

    //
    // Vector registers.
    //

    M128A VectorRegister[26];
    DWORD64 VectorControl;

    //
    // Special debug control registers.
    //

    DWORD64 DebugControl;
    DWORD64 LastBranchToRip;
    DWORD64 LastBranchFromRip;
    DWORD64 LastExceptionToRip;
    DWORD64 LastExceptionFromRip;
} CONTEXT, *PCONTEXT;

*/
