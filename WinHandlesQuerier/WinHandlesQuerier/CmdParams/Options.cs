using CommandLine;

namespace WinHandlesQuerier.CmdParams
{
    [Verb("dump", HelpText = "Get handles data of a process using dump file as source.")]
    public class DumpVerb : CommonVerb
    {
        [Option('p', "[FILE]", HelpText = "Absolete path to dump file.")]
        public string DumpFile { get; set; }
    }

    [Verb("live", HelpText = "Get handles data of a live process.")]
    public class LiveVerb : CommonVerb
    {
        [Option('p', "[PID]", HelpText = "PID of a live Process.")]
        public int LivePid { get; set; }
    }

    public class CommonVerb
    {
        [Option('b', "Blocking Objects", HelpText = "Get list of blocking objects.", Default = false)]
        public bool BlockingObjects { get; set; }

        [Option('s', "Stack Trace", HelpText = "List threads and their stack frames.", Default = false)]
        public bool StackTrace { get; set; }

        [Option('h', "Total handles", HelpText = "Summary of handles and their types ", Default = false)]
        public bool HandlesSummary { get; set; }

        [Option('t', "Threads list", HelpText = "List of process threads", Default = false)]
        public bool Threads { get; set; }

        [Option('a', "All", HelpText = "List all available data", Default = false)]
        public bool All { get; set; }

    }
}
