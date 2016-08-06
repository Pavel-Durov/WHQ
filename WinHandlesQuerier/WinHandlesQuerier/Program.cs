//#define LIVE_PID_DEBUG

using System;
using WinHandlesQuerier.CmdParams;
using WinHandlesQuerier.Core.Infra;
using WinHandlesQuerier.Core.Handlers;
using WinHandlesQuerier.ProcessStrategies;
using Microsoft.Win32;
using CommandLine;
using WinHandlesQuerier.Handlers;
using System.Collections.Generic;
using System.Diagnostics;

namespace WinHandlesQuerier
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessStrategy processStrategy = null;
#if LIVE_PID_DEBUG
            var pid = (int)Registry.CurrentUser.GetValue("my-ruthles-pid-key");
            processStrategy = new LiveProcessStrategy((uint)pid);
#else

            var parseResult = Parser.Default.ParseArguments<Options.DumpVerb, Options.LiveVerb>(args);

            processStrategy = parseResult.MapResult<Options.DumpVerb, Options.LiveVerb, ProcessStrategy>(
                  (Options.DumpVerb opts) => DumpFileSource(opts),
                  (Options.LiveVerb opts) => LiveProcessSource(opts),
                  (IEnumerable<Error> opts) => null);
#endif
            if (processStrategy != null)
            {
                var result = processStrategy.Run().Result;

                if (result.Error == null)
                {
                    Console.WriteLine($"Is64BitProcess : {Environment.Is64BitProcess}");

                    PrintHandler.Print(result, true);
                }
                else
                {
                    Console.WriteLine(result.Error.Description);
                }
            }

            Console.ReadKey();
        }

        private static ProcessStrategy LiveProcessSource(Options.LiveVerb options)
        {
            ProcessStrategy result = null;

            if (options.LivePid != Constants.INVALID_PID)
            {
                result = new LiveProcessStrategy((uint)options.LivePid);
            }

            return result;
        }

        private static ProcessStrategy DumpFileSource(Options.DumpVerb options)
        {
            ProcessStrategy result = null;

            if (!String.IsNullOrEmpty(options.DumpFile))
            {
                result = new DumpFileProcessStrategy(options.DumpFile);
            }

            return result;
        }
    }
}
