using System.Runtime.InteropServices;
using DWORD64 = System.UInt64;
using DWORD = System.Int32;
using WORD = System.UInt16;
using ULONGLONG = System.UInt64;
using LONGLONG = System.Int64;

namespace WinNativeApi.WinNT
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CONTEXT_X86
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

    /// <summary>
    /// https://social.msdn.microsoft.com/Forums/vstudio/en-US/f5af96bc-88ad-4b56-98a3-c6dc5114cb8a/getthreadcontext?forum=vcgeneral
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
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

    


}
