using System;
using System.Text;
using Assignments.Core.Extentions;
using Assignments.Core.Model.Unified;
using System.Collections.Generic;
using Assignments.Core.Model.Unified.Thread;

namespace Assignments.Core.Handlers
{
    public class PrintHandler
    {

        public static void Print(List<UnifiedThread> collection, bool log = false)
        {
            foreach (var item in collection)
            {
                if (log)
                {
                    PrintToLog(item);
                }
                else
                {
                    Print(item);
                }
            }
        }

        private static void PrintToLog(UnifiedThread item)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Thread info");
            sb.Append(item.AsString());

            DumpToLogAndConsole(sb);

            if (item.BlockingObjects?.Count > 0)
            {
                sb.AppendWithNewLine($"BlockingObjects : ");
                sb.Append($"{item.BlockingObjects.AsString('\t')}");

                DumpToLogAndConsole(sb);
            }

            sb.Append("- StackTrace");
            sb.Append(item.StackTrace.AsString('\t'));
            DumpToLogAndConsole(sb);
        }

        private static void DumpToLogAndConsole(StringBuilder sb)
        {
            Console.WriteLine(sb);
            LogHandler.Log(sb.ToString());
            sb.Clear();
        }

        public static void Print(UnifiedThread item)
        {
            Console.WriteLine("Thread info");
            Console.WriteLine(item.AsString());

            if (item.BlockingObjects?.Count > 0)
            {
                Console.WriteLine("- BlockingObjects");
                Console.WriteLine($"{item.BlockingObjects.AsString('\t')}");
            }

            //item.StackTrace
            Console.WriteLine("- StackTrace");

            Console.WriteLine(item.StackTrace.AsString('\t'));
        }
    }
}




