using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignments.Core.Model.StackFrame
{
    public class WinApiMultiWaitStackFrame
    {
        public uint[] ByteParams { get; internal set; }
        public uint HandleAddress { get; internal set; }
        public uint HandlesCunt { get; internal set; }
        public uint WaitallFlag { get; internal set; }
        public uint WaitTimeout { get; internal set; }
    }
}
