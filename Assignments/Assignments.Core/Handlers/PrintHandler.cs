using System;
using System.Text;
using Assignments.Core.Model.StackFrames;
using Assignments.Core.Extentions;

using Assignments.Core.Model.StackFrames.UnManaged;

namespace Assignments.Core.Handlers
{
    public class PrintHandler
    {
        public static void Print(AnalyzedThreadStack threadStack)
        {
            //TODO: Consider moving this implementation from ToString Overide to some outside handler 
            StringBuilder result = new StringBuilder();

            //Appending thread data
            result.AppendWithNewLine(threadStack.Thread.AsString());

            
            if (threadStack.HasBlockingObjects)
            {
                //Appending Blcking objects ie Extentions method
                result.Append(threadStack.Thread.BlockingObjects.AsString());
            }

            WriteToCoonsole(result);

            result.AppendWithNewLine(threadStack.WctThreadInfo.AsString());

            ///Appending Managed Stack frame list string
            result.AppendWithNewLine(threadStack.ManagedStackList.AsString());

            WriteToCoonsole(result);

            ///Appending UnManaged Stack frame list string
            result.AppendWithNewLine(threadStack.NativeStackList.AsString<WinApiStackFrame>());

            WriteToCoonsole(result);

            //Console.WriteLine(result.ToString());
        }

        private static void WriteToCoonsole(StringBuilder result)
        {
            Console.WriteLine(result.ToString());
            result.Clear();
        }
    }
}
