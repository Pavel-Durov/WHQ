//#define LIVE_PID_DEBUG

using System;
using WinHandlesQuerier.CmdParams;
using WinHandlesQuerier.Core.Infra;
using WinHandlesQuerier.ProcessStrategies;
using CommandLine;
using WinHandlesQuerier.Handlers;
using System.Collections.Generic;
using WinHandlesQuerier.Core.Model;

namespace WinHandlesQuerier
{
    public class Program
    {
        public static void Main(string[] args)
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
                result = new LiveProcessStrategy(options.LivePid);
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
