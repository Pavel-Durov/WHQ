
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core;
using System.IO;
using Assignments.Core.Handlers;
using Microsoft.Diagnostics.Runtime;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args != null && args.Length == 2 && !String.IsNullOrEmpty(args[0]))
            {
                if (args[0] == "-dump" && !String.IsNullOrEmpty(args[1]))
                {
                    using (DataTarget target = DataTarget.LoadCrashDump(args[1]))
                    {
                        Global.DumpFile.DoAnaytics(target, args[1]);
                    }
                }
                else if(args[0] == "-pid")
                {
                    int parsed = 0;
                    if(int.TryParse(args[0], out parsed))
                    {
                        Global.LiveProcess.Run(parsed);
                    }
                }
            }


            Console.ReadKey();
        }
    }
}
