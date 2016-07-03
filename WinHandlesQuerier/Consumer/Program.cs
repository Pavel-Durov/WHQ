//#define LIVE_PID_DEBUG

using System;
using Consumer.CmdParams;
using WinHandlesQuerier.Core.Infra;
using WinHandlesQuerier.Core.Handlers;
using Consumer.ProcessStrategies;
using Microsoft.Win32;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Is64BitProcess : {Environment.Is64BitProcess}");

            ProcessStrategy _processStrategy = null;

#if LIVE_PID_DEBUG
            var pid = (int)Registry.CurrentUser.GetValue("my-ruthles-pid-key");
            _processStrategy = new LiveProcessStrategy((uint)pid);
#else
            var options = new Options();

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
            }
#endif

            var result = _processStrategy.Run();
            PrintHandler.Print(result, true);

            Console.ReadKey();
        }

    }
}
