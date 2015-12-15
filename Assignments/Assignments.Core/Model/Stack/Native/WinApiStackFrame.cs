using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignments.Core.Model.Stack
{
    public class WinApiStackFrame
    {
        public uint HandleAddress { get; internal set; }
        /// <summary>
        /// Timeout in milliseconds
        /// </summary>
        public uint Timeout { get; internal set; }
    }
}
