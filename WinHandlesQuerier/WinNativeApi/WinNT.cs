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
   // [StructLayout(LayoutKind.Explicit, Pack = 16)]
    public unsafe struct CONTEXT_AMD64
    {
        //
        // Register parameter home addresses.
        //
        // N.B. These fields are for convience - they could be used to extend the
        //      context record in the future.
        //
        
        public DWORD64 P1Home;
        public DWORD64 P2Home;
        public DWORD64 P3Home;
        public DWORD64 P4Home;
        public DWORD64 P5Home;
        public DWORD64 P6Home;

        //
        // Control flags.
        //
        public DWORD ContextFlags;
        public DWORD MxCsr;

        //
        // Segment Registers and processor flags.
        //
        public WORD SegCs;
        public WORD SegDs;
        public WORD SegEs;
        public WORD SegFs;
        public WORD SegGs;
        public WORD SegSs;
        public DWORD EFlags;

        //
        // Debug registers
        //
        public DWORD64 Dr0;
        public DWORD64 Dr1;
        public DWORD64 Dr2;
        public DWORD64 Dr3;
        public DWORD64 Dr6;
        public DWORD64 Dr7;

        //
        // Integer registers.
        //
        public DWORD64 Rax;
        public DWORD64 Rcx;
        public DWORD64 Rdx;
        public DWORD64 Rbx;
        public DWORD64 Rsp;
        public DWORD64 Rbp;
        public DWORD64 Rsi;
        public DWORD64 Rdi;
        public DWORD64 R8;
        public DWORD64 R9;
        public DWORD64 R10;
        public DWORD64 R11;
        public DWORD64 R12;
        public DWORD64 R13;
        public DWORD64 R14;
        public DWORD64 R15;

        //
        // Program counter.
        //
        public DWORD64 Rip;

        //
        // Floating point state.
        //
        public DUMMYUNIONNAME dummyUnion;

        //
        // Vector registers.
        //
        M128A* VectorRegister;
        public DWORD64 VectorControl;

        //
        // Special debug control registers.
        //
        public DWORD64 DebugControl;
        public DWORD64 LastBranchToRip;
        public DWORD64 LastBranchFromRip;
        public DWORD64 LastExceptionToRip;
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
        [FieldOffset(1)]
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
