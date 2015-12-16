using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.Extentions;

namespace Assignments.Core.Model.Stack
{
    public class WinApiStackFrame
    {
        public uint HandleAddress { get; internal set; }
        /// <summary>
        /// Timeout in milliseconds
        /// </summary>
        public uint Timeout { get; internal set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendWithNewLine($"HandleAddress : {HandleAddress}");
            sb.AppendWithNewLine($"Timeout : {Timeout}");

            return sb.ToString();
        }
    }
}
