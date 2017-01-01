using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHQ.Core.msos;
using WinNativeApi.WinNT;

namespace WHQ.Core.Model
{
    public class UnifiedThreadContext
    {
        public UnifiedThreadContext(CONTEXT_X86 context, ThreadInfo threadInfo) : this(false)
        {
            Context = context;
        }

        public UnifiedThreadContext(CONTEXT_AMD64 context, ThreadInfo threadInfo) : this(true)
        {
            Context_amd64 = context;
            OSThreadId = threadInfo.OSThreadId;
        }

        public UnifiedThreadContext(bool is64bit)
        {
            Is64Bit = is64bit;
        }

        public uint OSThreadId { get; private set; }
        public bool Is64Bit { get; private set; }

        public CONTEXT_X86 Context { get; private set; }
        public CONTEXT_AMD64 Context_amd64 { get; private set; }

    }
}
