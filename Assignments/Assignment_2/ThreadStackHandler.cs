using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Runtime.Interop;
using System.Runtime.InteropServices;
using System.IO;

namespace Assignment_2.Handlers
{
    class ThreadStackHandler
    {
        public static void Handle(ClrThread thread)
        {
        }


        uint _numThreads;
        IDebugClient _debugClient;
        private ClrRuntime _runtime;

        public IEnumerable<ThreadInfo> Threads { get; private set; }

        internal void Handle(IDebugClient debugClient, ClrThread thread, ClrRuntime runtime)
        {

            _debugClient = debugClient;
            _runtime = runtime;


            uint _numThreads = 0;
            Util.VerifyHr(((IDebugSystemObjects)_debugClient).GetNumberThreads(out _numThreads));

            var threads = new List<ThreadInfo>();

            ThreadInfo specific_info = null;

            for (uint threadIdx = 0; threadIdx < _numThreads; ++threadIdx)
            {
                specific_info = GetThreadInfo(threadIdx);
                threads.Add(specific_info);
            }
            Threads = threads;


            var managedStack = GetManagedStackTrace(thread);
            var unmanagedStack = GetNativeStackTrace(specific_info.EngineThreadId);

            PrintToConsole(managedStack, unmanagedStack);
        }

        private void PrintToConsole(List<UnifiedStackFrame> managedStack, List<UnifiedStackFrame> unmanagedStack)
        {
            PrintStackTrace(managedStack);
            PrintStackTrace(unmanagedStack);
            TheMostProgressiveFeature(unmanagedStack);
        }

        private void TheMostProgressiveFeature(List<UnifiedStackFrame> unmanagedStack)
        {
            foreach (var unifiedStackFrame in unmanagedStack)
            {
                TheFeature(unifiedStackFrame);
            }
        }

        private void TheFeature(UnifiedStackFrame stackFrame)
        {
            var offset = stackFrame.FrameOffset; //Base Pointer - % EBP
            byte[] paramBuffer = new byte[4];
            int bytesRead = 0;
            offset += 4;

            for (int i = 0; i < 4; i++)
            {
                offset += 4;
                if (_runtime.ReadMemory(offset, paramBuffer, 4, out bytesRead))
                {
                    // TODO: print the 32-bit buffer content
                    string hexData = BitConverter.ToString(paramBuffer);
                    Console.Write("p{0}={1} ", i, hexData);

                }
            }

            Console.WriteLine();
        }


        public void PrintStackTrace(IEnumerable<UnifiedStackFrame> stackTrace)
        {
            foreach (var frame in stackTrace)
            {
                if (frame.Type == UnifiedStackFrameType.Special)
                {
                    Console.WriteLine("{0,-10}", "Special");
                    continue;
                }
                if (String.IsNullOrEmpty(frame.SourceFileName))
                {
                    Console.WriteLine("{0,-10} {1,-20:x16} {2}!{3}+0x{4:x}",
                        frame.Type, frame.InstructionPointer,
                        frame.Module, frame.Method, frame.OffsetInMethod);
                }
                else
                {
                    Console.WriteLine("{0,-10} {1,-20:x16} {2}!{3} [{4}:{5},{6}]",
                        frame.Type, frame.InstructionPointer,
                        frame.Module, frame.Method, frame.SourceFileName,
                        frame.SourceLineNumber, frame.SourceColumnNumber);
                }
            }
        }


        private ThreadInfo GetThreadInfo(uint threadIndex)
        {
            uint[] engineThreadIds = new uint[1];
            uint[] osThreadIds = new uint[1];
            Util.VerifyHr(((IDebugSystemObjects)_debugClient).GetThreadIdsByIndex(threadIndex, 1, engineThreadIds, osThreadIds));
            ClrThread managedThread = _runtime.Threads.FirstOrDefault(thread => thread.OSThreadId == osThreadIds[0]);
            return new ThreadInfo
            {
                Index = threadIndex,
                EngineThreadId = engineThreadIds[0],
                OSThreadId = osThreadIds[0],
                ManagedThread = managedThread
            };
        }


        public List<UnifiedStackFrame> GetStackTrace(uint threadIndex)
        {
            ThreadInfo threadInfo = GetThreadInfo(threadIndex);
            List<UnifiedStackFrame> unifiedStackTrace = new List<UnifiedStackFrame>();
            List<UnifiedStackFrame> nativeStackTrace = GetNativeStackTrace(threadInfo.EngineThreadId);
            if (threadInfo.IsManagedThread)
            {
                List<UnifiedStackFrame> managedStackTrace = GetManagedStackTrace(threadInfo.ManagedThread);
                int managedFrame = 0;
                for (int nativeFrame = 0; nativeFrame < nativeStackTrace.Count; ++nativeFrame)
                {
                    bool found = false;
                    for (int temp = managedFrame; temp < managedStackTrace.Count; ++temp)
                    {
                        if (nativeStackTrace[nativeFrame].InstructionPointer == managedStackTrace[temp].InstructionPointer)
                        {
                            managedStackTrace[temp].LinkedStackFrame = nativeStackTrace[nativeFrame];
                            unifiedStackTrace.Add(managedStackTrace[temp]);
                            managedFrame = temp + 1;
                            found = true;
                            break;
                        }
                        else if (managedFrame > 0)
                        {
                            // We have already seen at least one managed frame, and we're about
                            // to skip a managed frame because we didn't find a matching native
                            // frame. In this case, add the managed frame into the stack anyway.
                            unifiedStackTrace.Add(managedStackTrace[temp]);
                            managedFrame = temp + 1;
                            found = true;
                            break;
                        }
                    }
                    // We didn't find a matching managed frame, so add the native frame directly.
                    if (!found)
                        unifiedStackTrace.Add(nativeStackTrace[nativeFrame]);
                }
            }
            else
            {
                return nativeStackTrace;
            }
            return unifiedStackTrace;
        }

        private List<UnifiedStackFrame> GetManagedStackTrace(ClrThread thread)
        {
            return (from frame in thread.StackTrace
                    let sourceLocation = GetFileAndLineNumberSafe(frame)
                    select new UnifiedStackFrame(frame, sourceLocation)
                    ).ToList();
        }

        //\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\
        //Taken from msos.SymbolCache class
        //\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\
        private HashSet<string> _failedToLoadSymbols = new HashSet<string>();

        public SourceLocation GetFileAndLineNumberSafe(ClrStackFrame frame)
        {
            if (frame.Method == null || frame.Method.Type == null || frame.Method.Type.Module == null)
                return null;

            string moduleName = frame.Method.Type.Module.Name;
            if (_failedToLoadSymbols.Contains(moduleName))
                return null;

            // ClrStackFrame.GetFileAndLineNumber throws occasionally when something is wrong with the module.
            try
            {
                var location = frame.GetFileAndLineNumber();
                if (location == null)
                    _failedToLoadSymbols.Add(moduleName);

                return location;
            }
            catch (ClrDiagnosticsException)
            {
                _failedToLoadSymbols.Add(moduleName);
                return null;
            }
            catch (InvalidOperationException)
            {
                _failedToLoadSymbols.Add(moduleName);
                return null;
            }

        }
        //\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\
        //\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\


        private List<UnifiedStackFrame> GetNativeStackTrace(uint engineThreadId)
        {
            Util.VerifyHr(((IDebugSystemObjects)_debugClient).SetCurrentThreadId(engineThreadId));

            DEBUG_STACK_FRAME[] stackFrames = new DEBUG_STACK_FRAME[200];
            uint framesFilled;
            Util.VerifyHr(((IDebugControl)_debugClient).GetStackTrace(0, 0, 0, stackFrames, stackFrames.Length, out framesFilled));

            List<UnifiedStackFrame> stackTrace = new List<UnifiedStackFrame>();
            for (uint i = 0; i < framesFilled; ++i)
            {
                stackTrace.Add(new UnifiedStackFrame(stackFrames[i], (IDebugSymbols2)_debugClient));
            }
            return stackTrace;
        }



        //Helpers
        class Util
        {
            public static void VerifyHr(int hr)
            {
                if (hr != 0)
                    Marshal.ThrowExceptionForHR(hr);
            }
        }

        public class ThreadInfo
        {
            public uint Index { get; set; }
            public uint EngineThreadId { get; set; }
            public uint OSThreadId { get; set; }
            public ClrThread ManagedThread { get; set; }
            public string Detail { get; set; }

            public bool IsManagedThread { get { return ManagedThread != null; } }
        }



        public class UnifiedStackFrame
        {
            public UnifiedStackFrameType Type { get; set; }

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
}
