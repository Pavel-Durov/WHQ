using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHQ.Core.Providers.ClrMd.Model
{
    public class SourceLocation
    {
        public uint ColEnd { get; internal set; }
        public uint ColStart { get; internal set; }
        public string FilePath { get; internal set; }
        public uint LineNumber { get; internal set; }
        public uint LineNumberEnd { get; internal set; }
    }
}
