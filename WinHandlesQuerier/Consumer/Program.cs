#define LIVE_PID_DEBUG

using System;
using WinHandlesQuerier.CmdParams;
using WinHandlesQuerier.Core.Infra;
using WinHandlesQuerier.Core.Handlers;
using WinHandlesQuerier.ProcessStrategies;
using Microsoft.Win32;
using CommandLine;
using WinHandlesQuerier.Handlers;

namespace WinHandlesQuerier
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
            ProcessStrategy _processStrategy = null;

#if LIVE_PID_DEBUG
            var pid = (int)Registry.CurrentUser.GetValue("my-ruthles-pid-key");
            _processStrategy = new LiveProcessStrategy((uint)pid);
#else

            if (options.DumpFile != null)
            {
                _processStrategy = new DumpFileProcessStrategy(options.DumpFile);
            }
            else if (options.LivePid != Constants.INVALID_PID)
            {
                _processStrategy = new LiveProcessStrategy((uint)options.LivePid);
            }
#endif
            var result = _processStrategy.Run().Result;
            if (result != null)
            {
                PrintHandler.Print(result, true);
            }

        }

        private static void PrintOptions(Options options)
        {
            Console.WriteLine(options.ToString());
        }

    }
}
