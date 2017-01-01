using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHQ.Core.Providers.ClrMd.Model;

namespace WHQ.Providers.ClrMd.Model
{
    public class ClrThread
    {
        public List<BlockingObject> BlockingObjects { get; internal set; }
        public uint OSThreadId { get; internal set; }
        public IList<ClrStackFrame> StackTrace { get; }
    }
}
