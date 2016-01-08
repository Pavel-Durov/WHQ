using System;
using System.Text;
using Assignments.Core.Model.StackFrames;
using Assignments.Core.Extentions;

using Assignments.Core.Model.StackFrames.UnManaged;
using Assignments.Core.Model.Unified;
using System.Collections.Generic;

namespace Assignments.Core.Handlers
{
    public class PrintHandler
    {
        
        public static void Print(List<UnifiedResult> collection)
        {
            foreach (var item in collection)
            {
                Print(item);
            }
        }

        public static void Print(UnifiedResult item)
        {
            Console.WriteLine("Thread info");
            Console.WriteLine(item.Thread.AsString());

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




