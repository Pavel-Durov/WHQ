using System;
using System.Text;
using WinHandlesQuerier.Core.Extentions;
using WinHandlesQuerier.Core.Model.Unified;
using System.Collections.Generic;
using WinHandlesQuerier.Core.Model.Unified.Thread;

namespace WinHandlesQuerier.Core.Handlers
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

            PrintSummary(collection, log);
        }

        private static void PrintSummary(List<UnifiedThread> collection, bool log = false)
        {
            StringBuilder sb = new StringBuilder();

            Dictionary<string, int> temp = new Dictionary<string, int>();

            sb.AppendLine(":: SUMMARY ::");
            foreach (var item in collection)
            {
                if (item.BlockingObjects == null)
                    continue;

                foreach (var block in item.BlockingObjects)
                {
                    var key = block.WaitReason.ToString();
                    if (temp.ContainsKey(block.WaitReason.ToString()))
                    {
                        temp[key]++;
                    }
                    else
                    {
                        temp[key] = 1;
                    }
                }
            }

            //Console.WriteLine($"BlockingObjects Total: {total}");
            foreach (var item in temp)
            {
                sb.AppendLine($"{item.Key} : {item.Value}");
            }

            if (log)
            {
                DumpToLogAndConsole(sb);
            }
            else
            {
                Console.WriteLine(sb.ToString());
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




