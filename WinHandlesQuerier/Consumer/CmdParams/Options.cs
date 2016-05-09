using CommandLine;
using CommandLine.Text;


namespace Consumer.CmdParams
{
    public class Options
    {
        public const int INVALID_PID = 0;

        [Option('d', "dump", HelpText = "Input dump file absolute path.")]
        public string DumpFile { get; set; }

        [Option('p', "live", DefaultValue = 0, HelpText = "Enter Live process pid.")]
        public int LivePid { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }
    }
}
