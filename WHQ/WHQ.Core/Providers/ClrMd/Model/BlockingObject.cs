using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHQ.Providers.ClrMd.Model;

namespace WHQ.Core.Providers.ClrMd.Model
{
    public class BlockingObject
    {
        public ulong Object { get; internal set; }
        public List<ClrThread> Owners { get; internal set; }
        public int Reason { get; internal set; }
        public int RecursionCount { get; internal set; }
        public List<ClrThread> Waiters { get; internal set; }
    }
}
