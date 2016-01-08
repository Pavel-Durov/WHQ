using Assignments.Core.Model.Unified;
using Assignments.Core.Model.Unified.Thread;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignments.Core.Extentions
{
    public static class UnifiedToStringExtention
    {
        public static string AsString(this UnifiedThread thread)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendWithNewLine($"OSThreadId : {thread.OSThreadId}");
            sb.AppendWithNewLine($"IsManagedThreadManaged: {thread.IsManagedThread }");
            sb.AppendWithNewLine($"Detail: {thread.Detail}");
            sb.AppendWithNewLine($"EngineThreadId: {thread.EngineThreadId}");

            return sb.ToString();
        }

        #region Blocking objects

        public static string AsString(this List<UnifiedBlockingObject> blockingObjects, char prefix)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in blockingObjects)
            {
                sb.Append(item.AsString(prefix));
            }
            return sb.ToString();
        }

        public static string AsString(this UnifiedBlockingObject blockingObject, char prefix)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendWithNewLine($"{prefix}ManagedObjectAddress : {blockingObject.ManagedObjectAddress}");
            if (!String.IsNullOrEmpty(blockingObject.KernelObjectName))
            {
                sb.AppendWithNewLine($"{prefix}KernelObjectName: {blockingObject.KernelObjectName}");
            }
            sb.AppendWithNewLine($"{prefix}RecursionCount: {blockingObject.RecursionCount}");

            //TODO: Complete the info
            return sb.ToString();
        }

        #endregion
        
        
        #region StackTrace

        public static String AsString(this List<UnifiedStackFrame> list, char prefix)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var frame in list)
            {
                sb.AppendWithNewLine(frame.AsString(prefix));
            }

            return sb.ToString();
        }

        public static String AsString(this UnifiedStackFrame frame, char prefix)
        {
            StringBuilder result = new StringBuilder();

            if (frame.Type == UnifiedStackFrameType.Special)
            {
                result.AppendWithNewLine(String.Format("{0}{1,-10}", prefix, "Special"));
                return result.ToString();
            }
            if (String.IsNullOrEmpty(frame.SourceFileName))
            {
                result.AppendWithNewLine(String.Format("{0}{1,-10} {2,-20:x16} {3}!{4}+0x{5:x}",
                    prefix,frame.Type, frame.InstructionPointer,
                    frame.Module, frame.Method, frame.OffsetInMethod));
            }
            else
            {
                result.AppendWithNewLine(String.Format("{0}{1,-10} {2,-20:x16} {3}!{4} [{5}:{6},{7}]",
                    prefix,frame.Type, frame.InstructionPointer,
                    frame.Module, frame.Method, frame.SourceFileName,
                    frame.SourceLineNumber, frame.SourceColumnNumber));
            }

            return result.ToString();
        }


        #endregion
    }
}
