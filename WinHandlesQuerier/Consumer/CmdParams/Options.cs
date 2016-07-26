using WinHandlesQuerier.Core.Infra;
using CommandLine;
using CommandLine.Text;
using System.Text;
using WinHandlesQuerier.Core.Extentions;

namespace WinHandlesQuerier.CmdParams
{
    public class Options
    {
        [Option('d', "Dump", HelpText = "Input dump file absolute path.")]
        public string DumpFile { get; set; }

        [Option('p', "Live", HelpText = "Enter Live process pid.")]
        public int LivePid { get; set; }

        [Option('h', "Help", HelpText = "List of Command Line parameters")]
        public bool Help { get; set; } = false;


        [ParserState]
        public IParserState LastParserState { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            
            sb.AppendWithNewLine($"-h       - List of Command Line parameters");
            
            sb.AppendWithNewLine($"-p       - Connect to a Live Process");
            sb.AppendWithNewLine("Usage: -p [PID]");

            sb.AppendWithNewLine($"-d       - Load Dump file");
            sb.AppendWithNewLine("Usage: -d [FILE]");
            return sb.ToString();
        }
    }
}
