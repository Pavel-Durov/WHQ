using System;
using System.Text;
using WinHandlesQuerier.Core.Extentions;
using System.Collections.Generic;
using WinHandlesQuerier.Core.Model.Unified.Thread;
using WinHandlesQuerier.Core.Model;

namespace WinHandlesQuerier.Handlers
{
    public class PrintHandler
    {
        public static void Print(ProcessAnalysisResult analysis, bool log = false)
        {
            foreach (var unifiedThread in analysis.Threads)
            {
                var print = GetData(unifiedThread);
                PrintToLog(print);
                PrintToConsole(print);
            }

            var sammary = GetSummary(analysis);
            PrintToLog(sammary);
            PrintToConsole(sammary);
        }

        private static StringBuilder GetData(UnifiedThread threadInfo)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(threadInfo.AsString());

            if (threadInfo.BlockingObjects?.Count > 0)
            {
                sb.Append(threadInfo.BlockingObjects.AsString('\t'));
            }

            sb.Append(threadInfo.StackTrace.AsString('\t'));

            return sb;
        }

        private static StringBuilder GetSummary(ProcessAnalysisResult analysis)
        {
            StringBuilder sb = new StringBuilder();

            Dictionary<string, int> temp = new Dictionary<string, int>();

            sb.AppendLine(":: SUMMARY ::");
            foreach (var item in analysis.Threads)
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

            foreach (var item in temp)
            {
                sb.AppendLine($"{item.Key} : {item.Value}");
            }

            sb.AppendLine($"Elapsed Time: {analysis.ElapsedMilliseconds}");
            return sb;
        }

        private static void PrintToLog(StringBuilder sb)
        {
            LogHandler.Log(sb.ToString());
        }

        private static void PrintToLog(string str)
        {
            LogHandler.Log(str.ToString());
        }

        public static void PrintToConsole(StringBuilder sb)
        {
            Console.WriteLine(sb);
        }

        public static void PrintToConsole(string str)
        {
            Console.WriteLine(str);
        }
    }
}




