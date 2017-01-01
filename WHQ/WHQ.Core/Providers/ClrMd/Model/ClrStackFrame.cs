using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHQ.Core.Providers.ClrMd.Model
{
    /// <summary>
    /// The type of frame the ClrStackFrame represents.
    /// </summary>
    public enum ClrStackFrameType
    {
        /// <summary>
        /// Indicates this stack frame is unknown
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// Indicates this stack frame is a standard managed method.
        /// </summary>
        ManagedMethod = 0,

        /// <summary>
        /// Indicates this stack frame is a special stack marker that the Clr runtime leaves on the stack.
        /// Note that the ClrStackFrame may still have a ClrMethod associated with the marker.
        /// </summary>
        Runtime = 1
    }


    public class ClrStackFrame
    {
        public ulong InstructionPointer { get; internal set; }
        public ClrStackFrameType Kind { get; internal set; }
        public ClrMethod Method { get; internal set; }
        public ulong StackPointer { get; internal set; }

        internal object GetFileAndLineNumber()
        {
            throw new NotImplementedException();
        }
    }
}
