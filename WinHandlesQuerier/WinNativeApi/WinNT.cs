using System.Runtime.InteropServices;
namespace WinNativeApi.WinNT
{
    #region Structs

    //https://msdn.microsoft.com/en-us/library/windows/desktop/aa383751(v=vs.85).aspx
    using DWORD64 = System.UInt64;
    using DWORD = System.Int32;
    using WORD = System.UInt16;
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
        [FieldOffset(8)]
        public DWORD64 P2Home;
        [FieldOffset(16)]
        public DWORD64 P3Home;
        [FieldOffset(24)]
        public DWORD64 P4Home;
        [FieldOffset(32)]
        public DWORD64 P5Home;
        [FieldOffset(40)]
        public DWORD64 P6Home;

        //
        // Control flags.
        //
        [FieldOffset(48)]
        public DWORD ContextFlags;
        [FieldOffset(52)]
        public DWORD MxCsr;

        //
        // Segment Registers and processor flags.
        //
        [FieldOffset(56)]
        public WORD SegCs;
        [FieldOffset(58)]
        public WORD SegDs;
        [FieldOffset(60)]
        public WORD SegEs;
        [FieldOffset(62)]
        public WORD SegFs;
        [FieldOffset(64)]
        public WORD SegGs;
        [FieldOffset(66)]
        public WORD SegSs;
        [FieldOffset(68)]
        public DWORD EFlags;

        //
        // Debug registers
        //
        [FieldOffset(72)]
        public DWORD64 Dr0;
        [FieldOffset(80)]
        public DWORD64 Dr1;
        [FieldOffset(88)]
        public DWORD64 Dr2;
        [FieldOffset(96)]
        public DWORD64 Dr3;
        [FieldOffset(104)]
        public DWORD64 Dr6;
        [FieldOffset(112)]
        public DWORD64 Dr7;

        //
        // Integer registers.
        //
        [FieldOffset(120)]
        public DWORD64 Rax;
        [FieldOffset(128)]
        public DWORD64 Rcx;
        [FieldOffset(136)]
        public DWORD64 Rdx;
        [FieldOffset(144)]
        public DWORD64 Rbx;
        [FieldOffset(152)]
        public DWORD64 Rsp;
        [FieldOffset(160)]
        public DWORD64 Rbp;
        [FieldOffset(168)]
        public DWORD64 Rsi;
        [FieldOffset(176)]
        public DWORD64 Rdi;
        [FieldOffset(184)]
        public DWORD64 R8;
        [FieldOffset(192)]
        public DWORD64 R9;
        [FieldOffset(200)]
        public DWORD64 R10;
        [FieldOffset(208)]
        public DWORD64 R11;
        [FieldOffset(216)]
        public DWORD64 R12;
        [FieldOffset(224)]
        public DWORD64 R13;
        [FieldOffset(232)]
        public DWORD64 R14;
        [FieldOffset(240)]
        public DWORD64 R15;

        //
        // Program counter.
        //
        [FieldOffset(248)]
        public DWORD64 Rip;

        //
        // Floating point state.
        //
        [FieldOffset(256)]
        public DUMMYUNIONNAME dummyUnion;
        [FieldOffset(288)]
        M128A* Header;
        [FieldOffset(416)]
        M128A* Legacy;
        [FieldOffset(432)]
        M128A Xmm0;
        [FieldOffset(448)]
        M128A Xmm1;
        [FieldOffset(464)]
        M128A Xmm2;
        [FieldOffset(480)]
        M128A Xmm3;
        [FieldOffset(496)]
        M128A Xmm4;
        [FieldOffset(512)]
        M128A Xmm5;
        [FieldOffset(512)]
        M128A Xmm6;
        [FieldOffset(528)]
        M128A Xmm7;
        [FieldOffset(544)]
        M128A Xmm8;
        [FieldOffset(560)]
        M128A Xmm9;
        [FieldOffset(576)]
        M128A Xmm10;
        [FieldOffset(592)]
        M128A Xmm11;
        [FieldOffset(608)]
        M128A Xmm12;
        [FieldOffset(624)]
        M128A Xmm13;
        [FieldOffset(640)]
        M128A Xmm14;
        [FieldOffset(656)]
        M128A Xmm15;
        //
        // Vector registers.
        //

        [FieldOffset(768)]
        M128A* VectorRegister;
        [FieldOffset(1184)]
        public DWORD64 VectorControl;

        //
        // Special debug control registers.
        //
        [FieldOffset(1192)]
        public DWORD64 DebugControl;
        [FieldOffset(1200)]
        public DWORD64 LastBranchToRip;
        [FieldOffset(1208)]
        public DWORD64 LastBranchFromRip;
        [FieldOffset(1216)]
        public DWORD64 LastExceptionToRip;
        [FieldOffset(1224)]
        public DWORD64 LastExceptionFromRip;
    }

    struct XMM_SAVE_AREA32
    {
        public byte[] DummyContent;
    }


    [StructLayout(LayoutKind.Explicit)]
    public struct DUMMYUNIONNAME
    {
        //[FieldOffset(0)]
        //XMM_SAVE_AREA32 FltSave;
        //[FieldOffset(1)]
        //DUMMY Dummy;
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


    Struct CONTEXT Total Size: 1232

Data Types:
-----------
DWORD64: 8 bytes
DWORD: 4 bytes
WORD: 2 bytes
ULONGLONG: 8 bytes
LONGLONG: 8 bytes
M128A: 16 bytes
XMM_SAVE_AREA32: 512 bytes

Member Offsets:
---------------
P1Home: 0
P2Home: 8
P3Home: 16
P4Home: 24
P5Home: 32
P6Home: 40
ContextFlags: 48
MxCsr: 52
SegCs: 56
SegDs: 58
SegEs: 60
SegFs: 62
SegGs: 64
SegSs: 66
EFlags: 68

Dr0: 72
Dr1: 80
Dr2: 88
Dr3: 96
Dr6: 104
Dr7: 112

Rax: 120
Rcx: 128
Rdx: 136
Rbx: 144
Rsp: 152
Rbp: 160
Rsi: 168
Rdi: 176
R8: 184
R9: 192
R10: 200
R11: 208
R12: 216
R13: 224
R14: 232
R15: 240
Rip: 248
FltSave: 256
Header: 256
Legacy: 288
Xmm0: 416
Xmm1: 432
Xmm2: 448
Xmm3: 464
Xmm4: 480
Xmm5: 496
Xmm6: 512
Xmm7: 528
Xmm8: 544
Xmm9: 560
Xmm10: 576
Xmm11: 592
Xmm12: 608
Xmm13: 624
Xmm14: 640
Xmm15: 656
VectorRegister: 768
VectorControl: 1184
DebugControl: 1192
LastBranchToRip: 1200
LastBranchFromRip: 1208
LastExceptionToRip: 1216
LastExceptionFromRip: 1224

*/
