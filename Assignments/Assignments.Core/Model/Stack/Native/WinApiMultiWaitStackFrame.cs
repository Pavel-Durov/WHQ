using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.Extentions;

namespace Assignments.Core.Model.Stack
{
    public class WinApiMultiWaitStackFrame : WinApiStackFrame
    {
        public uint[] ByteParams { get; internal set; }
        public uint HandlesCunt { get; internal set; }
        public uint WaitallFlag { get; internal set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(base.ToString());

            sb.AppendWithNewLine($"WaitallFlag : {WaitallFlag}");
            sb.AppendWithNewLine($"HandlesCunt : {HandlesCunt}");

            for (int i = 0; i < ByteParams.Length; i++)
            {
                sb.AppendWithNewLine($"param {1} : {ByteParams[i]}");
            }

            return sb.ToString();

        }
    }
}
