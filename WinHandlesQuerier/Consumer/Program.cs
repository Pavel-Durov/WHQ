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

namespace WinHandlesQuerier
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Is64BitProcess : {Environment.Is64BitProcess}");

            ProcessStrategy processStrategy = null;
#if LIVE_PID_DEBUG
            var pid = (int)Registry.CurrentUser.GetValue("my-ruthles-pid-key");
            processStrategy = new LiveProcessStrategy((uint)pid);
#else

            processStrategy = (ProcessStrategy)Parser.Default.ParseArguments<Options.DumpVerb, Options.LiveVerb>(args)
               .MapResult(
                  (Options.DumpVerb opts) => DumpFileOptions(opts),
                  (Options.LiveVerb opts) => LiveProcessOptions(opts),
                  errs => 1);
#endif
           

            var result = processStrategy.Run().Result;

            if (result != null)
            {
                PrintHandler.Print(result, true);
            }

            Console.ReadKey();
        }

        private static object LiveProcessOptions(Options.LiveVerb options)
        {
            ProcessStrategy result = null;

            if (options.LivePid != Constants.INVALID_PID)
            {
                result = new LiveProcessStrategy((uint)options.LivePid);
            }

            return result;
        }

        private static object DumpFileOptions(Options.DumpVerb options)
        {
            ProcessStrategy result = null;

            if (options.DumpFile != null)
            {
                result = new DumpFileProcessStrategy(options.DumpFile);
            }

            return result;
        }
    }
}
