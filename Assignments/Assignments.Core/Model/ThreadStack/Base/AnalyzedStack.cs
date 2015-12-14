using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.msos;
using Microsoft.Diagnostics.Runtime;

namespace Assignments.Core.Model.ThreadStack
{
    public abstract class AnalyzedStack
    {
        public AnalyzedStack()
        {
            _stack = new List<UnifiedStackFrame>();
        }
        internal abstract void Fill(IEnumerable<UnifiedStackFrame> stackTrace, ClrRuntime runtime, ClrThread thread = null);

        protected List<UnifiedStackFrame> _stack;


        internal void Add(UnifiedStackFrame frame)
        {
            _stack.Add(frame);
        }


        //public void Print(UnifiedStackFrame frame)
        //{
        //    if (frame.Type == UnifiedStackFrameType.Special)
        //    {
        //        Console.WriteLine("{0,-10}", "Special");
        //        return;
        //    }
        //    if (String.IsNullOrEmpty(frame.SourceFileName))
        //    {
        //        Console.WriteLine("{0,-10} {1,-20:x16} {2}!{3}+0x{4:x}",
        //            frame.Type, frame.InstructionPointer,
        //            frame.Module, frame.Method, frame.OffsetInMethod);
        //    }
        //    else
        //    {
        //        Console.WriteLine("{0,-10} {1,-20:x16} {2}!{3} [{4}:{5},{6}]",
        //            frame.Type, frame.InstructionPointer,
        //            frame.Module, frame.Method, frame.SourceFileName,
        //            frame.SourceLineNumber, frame.SourceColumnNumber);
        //    }
        //}
    }
}
