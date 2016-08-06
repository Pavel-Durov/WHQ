using CommandLine;

namespace WinHandlesQuerier.CmdParams
{
    public class Options 
    {
        [Verb("dump", HelpText = "Get handles data of a process using dump file as source.")]
        public class DumpVerb
        {
            [Option('p', "[FILE]", HelpText = "Absolete path to dump file.")]
            public string DumpFile { get; set; }
        }

        [Verb("live", HelpText = "Get handles data of a live process.")]
        public class LiveVerb
        {
            [Option('p', "[PID]", HelpText = "PID of a live Process.")]
            public int LivePid { get; set; }
        }
    }
}
