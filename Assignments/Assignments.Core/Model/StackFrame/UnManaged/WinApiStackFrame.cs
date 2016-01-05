using System;
using System.Collections.Generic;
using System.Text;
using Assignments.Core.msos;
using Assignments.Core.Model.Unified;

namespace Assignments.Core.Model.StackFrames.UnManaged
{
    public class WinApiStackFrame
    {
        public UnifiedStackFrame Frame{get; set;}
        public uint HandleAddress { get; internal set; }
        public List<byte[]> Params { get; set; }
        /// <summary>
        /// Timeout in milliseconds
        /// </summary>
        public uint Timeout { get; internal set; }
    }
}
