using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WHQ.Core.Providers.ClrMd.Model
{
    public interface IDebugSymbols2
    {
        int GetModuleByOffset(ulong instructionPointer, int v, out uint moduleIndex, out ulong dummy);
        int GetNameByOffset(ulong instructionPointer, StringBuilder methodName, int capacity, out uint dummy2, out ulong displacement);
        int GetLineByOffset(ulong instructionPointer, out uint sourceLine, StringBuilder sourceFile, int capacity, out uint dummy2, out ulong delta);
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DEBUG_STACK_FRAME
    {
        public UInt64 InstructionOffset;
        public UInt64 ReturnOffset;
        public UInt64 FrameOffset;
        public UInt64 StackOffset;
        public UInt64 FuncTableEntry;
        public fixed UInt64 Params[4];
        public fixed UInt64 Reserved[6];
        public UInt32 Virtual;
        public UInt32 FrameNumber;
    }
}
