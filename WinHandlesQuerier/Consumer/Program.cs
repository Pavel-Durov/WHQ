//#define LIVE_PID_DEBUG

using System;
using Consumer.CmdParams;
using Assignments.Core.Infra;
using Consumer.ProccessStrategies;
using WinHandlesQuerier.Core.Handlers;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Is64BitProcess : {Environment.Is64BitProcess}");

#if LIVE_PID_DEBUG
            var pid = (int)Registry.CurrentUser.GetValue("my-ruthles-pid-key");
            Global.LiveProcess.Run((uint)pid);
#else

            var options = new Options();

            ProcessStrategy _processStrategy = null;

            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                if (options.DumpFile != null)
                {
                    _processStrategy = new DumpFileProcessStrategy(options.DumpFile);
                    
                }
                else if(options.LivePid != Constants.INVALID_PID)
                {
                    _processStrategy = new LiveProcessStrategy((uint)options.LivePid);
                }

                var result = _processStrategy.Run();
                PrintHandler.Print(result, true);

                Console.ReadKey(); 
            }
#endif
            Console.ReadKey();
        }

    }
}
