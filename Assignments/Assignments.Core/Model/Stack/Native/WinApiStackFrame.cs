using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.Extentions;
using Assignments.Core.msos;
using Assignments.Core.Handlers;

namespace Assignments.Core.Model.Stack
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();


            sb.AppendWithNewLine(String.Format("{0,-10} {1,-20:x16} {2}!{3}+0x{4:x}",
                    Frame.Type, Frame.InstructionPointer,
                    Frame.Module, Frame.Method, Frame.OffsetInMethod));

            sb.AppendWithNewLine($"HandleAddress : {HandleAddress}");
            sb.AppendWithNewLine($"Timeout : {Timeout}");

           
            sb.AppendWithNewLine($"Params: { Params.AsString()}");
            
            return sb.ToString();
        }



    }
}
