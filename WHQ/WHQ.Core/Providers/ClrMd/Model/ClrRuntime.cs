using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHQ.Providers.ClrMd.Model
{
    public class ClrRuntime
    {
        public List<ClrThread> Threads { get; internal set; }

        internal bool ReadMemory(ulong startAddress, byte[] readedBytes, int size, out int sum)
        {
            throw new NotImplementedException();
        }
    }
}
