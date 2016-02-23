
using System;
using Consumer.CmdParams;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();

            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                if (options.DumpFile != null)
                {
                    Global.DumpFile.DoAnaytics(options.DumpFile);
                }
                else if(options.LivePid != Options.INVALID_PID)
                {
                    Global.LiveProcess.Run(options.LivePid);
                }
            }

            Console.ReadKey();
        }
    }
}
