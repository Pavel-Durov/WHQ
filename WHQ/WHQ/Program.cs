//#define LIVE_PID_DEBUG

using System;
using WHQ.CmdParams;
using WHQ.Core.Infra;
using WHQ.ProcessStrategies;
using CommandLine;
using WHQ.Handlers;
using System.Collections.Generic;
using WHQ.Core.Model;

namespace WHQ
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

            var parseResult = Parser.Default.ParseArguments<DumpVerb, LiveVerb>(args);

            processStrategy = parseResult.MapResult<DumpVerb, LiveVerb, ProcessStrategy>(
                  (DumpVerb opts) => DumpFileSource(opts),
                  (LiveVerb opts) => LiveProcessSource(opts),
                  (IEnumerable<Error> opts) => null);
#endif
            if (processStrategy != null)
            {
                var result = processStrategy.Run().Result;

                if (result.Error == null)
                {
                    Console.WriteLine($"Is64BitProcess : {Environment.Is64BitProcess}");

                    SetPrintOptions(processStrategy.Options);
                    PrintHandler.Print(result, true);
                }
                else
                {
                    Console.WriteLine(result.Error.Description);
                }
            }

            Console.ReadKey();
        }

        private static void SetPrintOptions(CommonVerb options)
        {
            if (options.All)
            {
                PrintHandler.Options = PrintHandler.PrintOptions.All;
            }
            else
            {
                if (options.StackTrace)
                {
                    PrintHandler.Options = PrintHandler.Options | PrintHandler.PrintOptions.StackTrace;
                }

                if (options.BlockingObjects)
                {
                    PrintHandler.Options = PrintHandler.Options | PrintHandler.PrintOptions.BlockingObjects;
                }

                if (options.HandlesSummary)
                {
                    PrintHandler.Options = PrintHandler.Options | PrintHandler.PrintOptions.HandlesSummary;
                }

                if (options.Threads)
                {
                    PrintHandler.Options = PrintHandler.Options | PrintHandler.PrintOptions.Threads;
                }
            }
        }

        private static ProcessStrategy LiveProcessSource(LiveVerb options)
        {
            ProcessStrategy result = null;

            if (options.LivePid != Constants.INVALID_PID)
            {
                result = new LiveProcessStrategy((uint)options.LivePid);
                result.Options = (LiveVerb)options;
            }

            return result;
        }

        private static ProcessStrategy DumpFileSource(DumpVerb options)
        {
            ProcessStrategy result = null;

            if (!String.IsNullOrEmpty(options.DumpFile))
            {
                result = new DumpFileProcessStrategy(options.DumpFile);
                result.Options = (CommonVerb)options;
            }

            return result;
        }
    }
}
