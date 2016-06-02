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
        public UnifiedThreadContext(CONTEXT context)
        {
            _context = context;
        }

        public UnifiedThreadContext(CONTEXT_AMD64 context)
        {
            _context_amd64 = context;
        }

        CONTEXT _context;
        CONTEXT_AMD64 _context_amd64;
    }
}
