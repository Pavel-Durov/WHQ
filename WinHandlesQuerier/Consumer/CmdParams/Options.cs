using WinHandlesQuerier.Core.Infra;
using CommandLine;

using CommandLine.Text;
using System.Text;
using WinHandlesQuerier.Core.Extentions;
using System;

namespace WinHandlesQuerier.CmdParams
{
    public class Options 
    {
        [Verb("dump", HelpText = "Get handles data of a process using dump file as source.")]
        public class DumpVerb
        {

            [Option('p', "Path", HelpText = "Get handles data of a process using dump file as source.")]
            public string DumpFile { get; set; }

            //[Option('f', "force", SetName = "mode-f",
            //    HelpText = "Allow adding otherwise ignored files.")]
            //public bool Force { get; set; }

            [Value(0)]
            public string FileName { get; set; }
        }

        [Verb("live", HelpText = "Get handles data of a live process.")]
        public class LiveVerb
        {

            [Option('p', "Live", HelpText = "Get handles data of a lve process.")]
            public int LivePid { get; set; }


            //[Option('f', "force", SetName = "mode-f",
            //    HelpText = "Allow adding otherwise ignored files.")]
            //public bool Force { get; set; }

            [Value(0)]
            public string FileName { get; set; }
        }

        //[Option('d', "Dump", HelpText = "Get handles data of a process using dump file as source.")]
        //public string DumpFile { get; set; }

        //[Option('p', "Live", HelpText = "Get handles data of a lve process.")]
        //public int LivePid { get; set; }

        //[HelpOption]
        //public string GetUsage()
        //{
        //    return HelpText.AutoBuild(this,
        //      (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        //}
    }
}
