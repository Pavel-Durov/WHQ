using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinNativeApi.WinNT;

namespace Assignments.Core.Model
{
    internal class UnifiedThreadContext
    {
        public UnifiedThreadContext(CONTEXT context): this(false)
        {
            Context = context;
        }

        public UnifiedThreadContext(CONTEXT_AMD64 context) : this(true)
        {
            Context_amd64 = context;
            
        }

        public UnifiedThreadContext(bool is64bit)
        {
            Is64Bit = is64bit;
        }

        public bool Is64Bit { get; set; }

        public CONTEXT Context { get; private set; }
        public CONTEXT_AMD64 Context_amd64 { get; private set; }

    }
}
