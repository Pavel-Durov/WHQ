using Assignments.Core.msos;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignments.Core.Extentions
{
    public static class ToStringExtention
    {


        private static String BytesAsHexString(this List<byte[]> parms)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < parms.Count; i++)
            {
                int byteValue = BitConverter.ToInt32(parms[i], 0);

                var msg = String.Format("p{0}= 0x{1:x}", i, byteValue);
                sb.Append(msg);
            }

            return sb.ToString();
        }

        public static String GetAsString(this UnifiedStackFrame frame)
        {
            StringBuilder result = new StringBuilder();

            if (frame.Type == UnifiedStackFrameType.Special)
            {
                result.AppendWithNewLine(String.Format("{0,-10}", "Special"));
                return result.ToString();
            }
            if (String.IsNullOrEmpty(frame.SourceFileName))
            {
                result.AppendWithNewLine(String.Format("{0,-10} {1,-20:x16} {2}!{3}+0x{4:x}",
                    frame.Type, frame.InstructionPointer,
                    frame.Module, frame.Method, frame.OffsetInMethod));
            }
            else
            {
                result.AppendWithNewLine(String.Format("{0,-10} {1,-20:x16} {2}!{3} [{4}:{5},{6}]",
                    frame.Type, frame.InstructionPointer,
                    frame.Module, frame.Method, frame.SourceFileName,
                    frame.SourceLineNumber, frame.SourceColumnNumber));
            }

            return result.ToString();
        }

        public static String GetAsString(this IList<BlockingObject> blockingObjects)
        {
            StringBuilder result = new StringBuilder();

            if (blockingObjects != null && blockingObjects?.Count > 0)
            {
                foreach (var bObj in blockingObjects)
                {
                    result.AppendWithNewLine(bObj.GetAsString());
                }
            }
            return result.ToString();
        }

        public static String GetAsString(this BlockingObject bObj)
        {
            StringBuilder result = new StringBuilder();

            if (bObj != null)
            {
                result.AppendWithNewLine($"Associated Object : {bObj.Object}");

                if (bObj.HasSingleOwner && bObj.Taken)
                {
                    result.AppendWithNewLine($"Single Owner : {bObj.Owner}");
                }
                else
                {
                    int ownerCounter = 0;
                    result.AppendWithNewLine("-- Owners -- ");
                    result.AppendWithNewLine($"{ThreadsIds(bObj.Waiters)}");

                    foreach (var owner in bObj.Owners)
                    {
                        result.AppendWithNewLine($"({++ownerCounter}) Owner: {owner?.OSThreadId}");
                    }
                }

                result.AppendWithNewLine($"Block Reason : {bObj.Reason}");
                result.AppendWithNewLine($"Taken : {0}{bObj.Taken}");
                result.AppendWithNewLine(" -- Witers List -- ");
            
                result.AppendWithNewLine(ThreadsIds(bObj.Waiters));                
            }
            
            return result.ToString();
        }

        private static string ThreadsIds(IList<ClrThread> list)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < list.Count; i++)
            {
                result.AppendWithNewLine($"{i}) Owner: {list[i]?.OSThreadId}");
            }

            return result.ToString();
        }
    }
}
