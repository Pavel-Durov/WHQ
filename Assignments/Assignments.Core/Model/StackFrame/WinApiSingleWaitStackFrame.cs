using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignments.Core.Model.StackFrame
{
    public class WinApiSingleWaitStackFrame
    {
        public uint HandleAddress { get; internal set; }
        public uint TimeoutMilliseconds { get; internal set; }
    }
}
