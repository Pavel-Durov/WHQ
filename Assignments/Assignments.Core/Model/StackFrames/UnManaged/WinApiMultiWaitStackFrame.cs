using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.Extentions;

namespace Assignments.Core.Model.StackFrames.UnManaged
{
    public class WinApiMultiWaitStackFrame : WinApiStackFrame
    {
        public const string FUNCTION_NAME = "WaitForMultipleObjects";

        public uint HandlesCunt { get; internal set; }
        public uint WaitallFlag { get; internal set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(base.ToString());

            sb.AppendWithNewLine($"WaitallFlag : {WaitallFlag}");
            sb.AppendWithNewLine($"HandlesCunt : {HandlesCunt}");

            sb.AppendWithNewLine($"Params: { Params.AsString()}");
            return sb.ToString();

        }
    }
}
