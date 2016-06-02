using WinHandlesQuerier.Core.msos;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interop;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using WinNativeApi.WinNT;

namespace WinHandlesQuerier.Core.Model.Unified
{
    public class UnifiedStackFrame
    {
        public CONTEXT_AMD64 ThreadContext { get; set; }

        public UnifiedStackFrameType Type { get; set; }
        public UnifiedBlockingObject BlockObject { get; set; }

        public string Module { get; set; }
        public string Method { get; set; }

        public ulong OffsetInMethod { get; set; }
        public ulong FrameOffset { get; private set; } // %EBP - Base Pointer 
        public ulong InstructionPointer { get; set; } // %EIP - Instruction Pointer 
        public ulong StackPointer { get; set; } // %ESP - Stack Pointer 

        public string SourceFileName { get; set; }
        public uint SourceLineNumber { get; set; }
        public uint SourceLineNumberEnd { get; set; }
        public uint SourceColumnNumber { get; set; }
        public uint SourceColumnNumberEnd { get; set; }

        public string SourceAndLine
        {
            get
            {
                if (String.IsNullOrEmpty(SourceFileName))
                    return null;
                return String.Format("{0}:{1},{2}", SourceFileName, SourceLineNumber, SourceColumnNumber);
            }
        }

        public bool HasSource
        {
            get { return !String.IsNullOrEmpty(SourceFileName); }
        }

        public UnifiedStackFrame LinkedStackFrame { get; set; } //Used for linking managed frame to native frame
        public List<byte[]> NativeParams { get; set; }
        public List<UnifiedHandle> Handles { get; set; }

        public UnifiedStackFrame(DEBUG_STACK_FRAME nativeFrame, IDebugSymbols2 debugSymbols)
        {
            FrameOffset = nativeFrame.FrameOffset;

            Type = UnifiedStackFrameType.Native;
            InstructionPointer = nativeFrame.InstructionOffset;
            StackPointer = nativeFrame.StackOffset;


            uint moduleIndex;
            ulong dummy;
            if (0 != debugSymbols.GetModuleByOffset(InstructionPointer, 0, out moduleIndex, out dummy))
            {
                //Some frames might not have modules associated with them, in which case this
                //will fail, and of course there is no function associated either. This happens
                //often with CLR JIT-compiled code.
                Module = "<Unknown>";
                Method = "<Unknown>";
                return;
            }

            StringBuilder methodName = new StringBuilder(1024);
            ulong displacement;
            uint dummy2;
            Util.VerifyHr(debugSymbols.GetNameByOffset(InstructionPointer, methodName, methodName.Capacity, out dummy2, out displacement));

            string[] parts = methodName.ToString().Split('!');
            Module = parts[0];
            if (parts.Length > 1)
            {
                Method = parts[1];
            }
            OffsetInMethod = displacement;

            uint sourceLine;
            ulong delta;
            StringBuilder sourceFile = new StringBuilder(1024);
            if (0 == debugSymbols.GetLineByOffset(InstructionPointer, out sourceLine, sourceFile, sourceFile.Capacity, out dummy2, out delta))
            {
                SourceFileName = sourceFile.ToString();
                SourceLineNumber = sourceLine;
                SourceLineNumberEnd = sourceLine;
            }
        }

        public UnifiedStackFrame(ClrStackFrame frame, SourceLocation sourceLocation)
        {
            if (frame.Kind == ClrStackFrameType.ManagedMethod)
                Type = UnifiedStackFrameType.Managed;
            if (frame.Kind == ClrStackFrameType.Runtime)
                Type = UnifiedStackFrameType.Special;

            InstructionPointer = frame.InstructionPointer;
            StackPointer = frame.StackPointer;

            if (frame.Method == null)
                return;

            Method = frame.Method.GetFullSignature();
            if (frame.Method.Type != null)
                Module = Path.GetFileNameWithoutExtension(frame.Method.Type.Module.Name);

            OffsetInMethod = InstructionPointer - frame.Method.NativeCode;

            if (sourceLocation == null)
                return;

            SourceFileName = sourceLocation.FilePath;
            SourceLineNumber = (uint)sourceLocation.LineNumber;
            SourceLineNumberEnd = (uint)sourceLocation.LineNumberEnd;
            SourceColumnNumber = (uint)sourceLocation.ColStart;
            SourceColumnNumberEnd = (uint)sourceLocation.ColEnd;
        }
    }

    public enum UnifiedStackFrameType
    {
        Managed,
        Native,
        Special
    }

}
