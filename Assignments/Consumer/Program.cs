#define LIVE_PID_DEBUG

using System;
using Consumer.CmdParams;
using Microsoft.Win32;


namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {

#if DEBUG && LIVE_PID_DEBUG
            var pid = (int)Registry.CurrentUser.GetValue("my-ruthles-pid-key");
            Global.LiveProcess.Run((uint)pid);
#else

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
#endif
            Console.ReadKey();
        }

    }
}
