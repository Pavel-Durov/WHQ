using System;
using System.Text;
using WinHandlesQuerier.Core.Extentions;
using System.Collections.Generic;
using WinHandlesQuerier.Core.Model.Unified.Thread;

namespace WinHandlesQuerier.Core.Handlers
{
    public class PrintHandler
    {
        public static void Print(List<UnifiedThread> unifiedThreadCollection, bool log = false)
        {
            foreach (var unifiedThread in unifiedThreadCollection)
            {
                PrintToLog(GetData(unifiedThread));
                PrintToConsole(GetData(unifiedThread));
            }

            PrintToLog(GetSummary(unifiedThreadCollection));
            PrintToConsole(GetSummary(unifiedThreadCollection));
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

        private static StringBuilder GetSummary(List<UnifiedThread> collection)
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

            foreach (var item in temp)
            {
                sb.AppendLine($"{item.Key} : {item.Value}");
            }

            return sb;
        }

        private static void PrintToLog(StringBuilder sb)
        {
            LogHandler.Log(sb.ToString());
        }

        public static void PrintToConsole(StringBuilder sb)
        {
            Console.WriteLine(sb);
        }
    }
}




