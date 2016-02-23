using CommandLine;
using CommandLine.Text;


namespace Consumer.CmdParams
{
    public class Options
    {
        public const int INVALID_PID = -1;

        [Option('d', "dump", HelpText = "Input dump file absolete path.")]
        public string DumpFile { get; set; }

        [Option('p', "live", DefaultValue = -1, HelpText = "Enter Live process pid.")]

        public int LivePid { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }
    }
}
