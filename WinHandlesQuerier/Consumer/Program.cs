//#define LIVE_PID_DEBUG

using System;
using Consumer.CmdParams;
using WinHandlesQuerier.Core.Infra;
using WinHandlesQuerier.Core.Handlers;
using Consumer.ProcessStrategies;
using Microsoft.Win32;
using CommandLine;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();

            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                if (options.Help)
                {
                    PrintOptions(options);
                }
                else
                {
                    HandleProcess(args, options);
                }
            }
            
            Console.ReadKey();
        }

        private static void HandleProcess(string[] args, Options options)
        {
            Console.WriteLine($"Is64BitProcess : {Environment.Is64BitProcess}");

#if LIVE_PID_DEBUG
            var pid = (int)Registry.CurrentUser.GetValue("my-ruthles-pid-key");
            _processStrategy = new LiveProcessStrategy((uint)pid);
#else
            ProcessStrategy _processStrategy = null;

            if (options.DumpFile != null)
            {
                _processStrategy = new DumpFileProcessStrategy(options.DumpFile);
            }
            else if (options.LivePid != Constants.INVALID_PID)
            {
                _processStrategy = new LiveProcessStrategy((uint)options.LivePid);
            }

            var result = _processStrategy.Run().Result;
            PrintHandler.Print(result, true);
#endif
        }

        private static void PrintOptions(Options options)
        {
            Console.WriteLine(options.ToString());
        }

    }
}
