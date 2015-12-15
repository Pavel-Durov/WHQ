using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignments.Core.Model.Stack
{
    public class WinApiMultiWaitStackFrame : WinApiStackFrame
    {
        public uint[] ByteParams { get; internal set; }
        
        public uint HandlesCunt { get; internal set; }
        public uint WaitallFlag { get; internal set; }
        
    }
}
